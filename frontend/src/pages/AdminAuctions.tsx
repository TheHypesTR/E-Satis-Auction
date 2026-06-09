/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import type { Dispatch, ReactNode, SetStateAction } from 'react';
import { Check, Eye, Gavel, Plus, Search, X } from 'lucide-react';
import { adminApi } from '../services/adminApi';
import { auctionBidStatusLabel, auctionStatusLabel, formatMoney, getApiErrorMessage } from '../services/apiUtils';
import type { AdminAuctionBidDto, AdminProductListingSummaryDto } from '../types/admin';
import type { AuctionDetailDto, AuctionSummaryDto } from '../types/commerce';

type AuctionForm = {
  productListingId: string;
  sellerUserId: string;
  startingPrice: string;
  minimumBidIncrement: string;
  startsAt: string;
  endsAt: string;
  quantity: string;
  currency: string;
};

type TransitionKind = 'schedule' | 'relist';
type TransitionForm = { auctionId: string; kind: TransitionKind; startsAt: string; endsAt: string };
type ImmediateAction = 'activate' | 'finalize' | 'cancel';

const DEFAULT_CURRENCY = 'TRY';

function toDateTimeLocalValue(value?: string | Date | null): string {
  if (!value) return '';
  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return '';
  const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
  return local.toISOString().slice(0, 16);
}

function defaultDateTimeLocal(offsetMinutes: number): string {
  return toDateTimeLocalValue(new Date(Date.now() + offsetMinutes * 60000));
}

function toBackendIso(value: string): string {
  return new Date(value).toISOString();
}

function createEmptyForm(): AuctionForm {
  return {
    productListingId: '',
    sellerUserId: '',
    startingPrice: '',
    minimumBidIncrement: '10',
    startsAt: defaultDateTimeLocal(10),
    endsAt: defaultDateTimeLocal(70),
    quantity: '1',
    currency: DEFAULT_CURRENCY,
  };
}

function hasValidDateRange(startsAt: string, endsAt: string): boolean {
  if (!startsAt || !endsAt) return false;
  return new Date(endsAt).getTime() > new Date(startsAt).getTime();
}

export default function AdminAuctions() {
  const [auctions, setAuctions] = useState<AuctionSummaryDto[]>([]);
  const [listings, setListings] = useState<AdminProductListingSummaryDto[]>([]);
  const [selected, setSelected] = useState<AuctionDetailDto | null>(null);
  const [bids, setBids] = useState<AdminAuctionBidDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [search, setSearch] = useState('');
  const [showAdd, setShowAdd] = useState(false);
  const [transition, setTransition] = useState<TransitionForm | null>(null);
  const [form, setForm] = useState<AuctionForm>(() => createEmptyForm());

  const load = () => {
    setLoading(true);
    Promise.all([adminApi.auctions({ pageSize: 100 }), adminApi.listings({ pageSize: 100 })])
      .then(([auctionRows, listingRows]) => { setAuctions(auctionRows); setListings(listingRows); })
      .catch(err => setError(getApiErrorMessage(err, 'Açık artırmalar yüklenemedi.')))
      .finally(() => setLoading(false));
  };

  useEffect(load, []);

  const filtered = auctions.filter(a =>
    a.productName.toLowerCase().includes(search.toLowerCase()) ||
    a.sku.toLowerCase().includes(search.toLowerCase()),
  );

  const validateForm = (mode: 'create' | 'update') => {
    const startingPrice = Number(form.startingPrice);
    const minimumBidIncrement = Number(form.minimumBidIncrement);
    const quantity = Number(form.quantity);

    if (mode === 'create' && !form.productListingId) return 'ProductListing seçilmelidir.';
    if (!Number.isFinite(startingPrice) || startingPrice <= 0) return 'Başlangıç fiyatı 0’dan büyük olmalıdır.';
    if (!Number.isFinite(minimumBidIncrement) || minimumBidIncrement <= 0) return 'Minimum artış 0’dan büyük olmalıdır.';
    if (!Number.isFinite(quantity) || quantity <= 0) return 'Adet 0’dan büyük olmalıdır.';
    if (!form.currency.trim() || form.currency.trim().length !== 3) return 'Currency 3 karakter olmalıdır.';
    if (!hasValidDateRange(form.startsAt, form.endsAt)) return 'Bitiş tarihi başlangıç tarihinden sonra olmalıdır.';
    return '';
  };

  const createPayload = () => ({
    productListingId: form.productListingId,
    sellerUserId: form.sellerUserId.trim() || null,
    startingPrice: Number(form.startingPrice),
    minimumBidIncrement: Number(form.minimumBidIncrement),
    startsAt: toBackendIso(form.startsAt),
    endsAt: toBackendIso(form.endsAt),
    quantity: Number(form.quantity),
    currency: form.currency.trim().toUpperCase(),
  });

  const updatePayload = () => {
    const payload = createPayload();
    return {
      sellerUserId: payload.sellerUserId,
      startingPrice: payload.startingPrice,
      minimumBidIncrement: payload.minimumBidIncrement,
      startsAt: payload.startsAt,
      endsAt: payload.endsAt,
      quantity: payload.quantity,
      currency: payload.currency,
    };
  };

  const create = async () => {
    const validationError = validateForm('create');
    if (validationError) { setError(validationError); return; }
    setSaving(true); setError(''); setMessage('');
    try {
      await adminApi.createAuction(createPayload());
      setMessage('Auction oluşturuldu. Schedule/activate akışında stok rezervasyonu backend tarafından yönetilir.');
      setShowAdd(false);
      setForm(createEmptyForm());
      load();
    } catch (err) { setError(getApiErrorMessage(err, 'Auction oluşturulamadı.')); }
    finally { setSaving(false); }
  };

  const open = async (id: string) => {
    setError(''); setMessage('');
    try {
      const [detail, bidList] = await Promise.all([
        adminApi.auction(id),
        adminApi.auctionBids(id, { pageSize: 50 }).catch(() => []),
      ]);
      setSelected(detail);
      setBids(bidList);
      setForm({
        productListingId: detail.productListingId,
        sellerUserId: detail.sellerUserId ?? '',
        startingPrice: String(detail.startingPrice),
        minimumBidIncrement: String(detail.minimumBidIncrement),
        startsAt: toDateTimeLocalValue(detail.startsAt),
        endsAt: toDateTimeLocalValue(detail.endsAt),
        quantity: String(detail.quantity),
        currency: detail.currency,
      });
    } catch (err) { setError(getApiErrorMessage(err, 'Auction detayı alınamadı.')); }
  };

  const update = async () => {
    if (!selected) return;
    const validationError = validateForm('update');
    if (validationError) { setError(validationError); return; }
    setSaving(true); setError(''); setMessage('');
    try {
      const detail = await adminApi.updateAuction(selected.id, updatePayload());
      setSelected(detail);
      setMessage('Auction güncellendi. ProductListing backend update DTO’sunda değiştirilmez.');
      load();
    } catch (err) { setError(getApiErrorMessage(err, 'Auction güncellenemedi.')); }
    finally { setSaving(false); }
  };

  const runImmediateAction = async (id: string, kind: ImmediateAction) => {
    setSaving(true); setError(''); setMessage('');
    try {
      if (kind === 'activate') await adminApi.activateAuction(id);
      if (kind === 'finalize') await adminApi.finalizeAuction(id);
      if (kind === 'cancel') await adminApi.cancelAuction(id);
      setMessage(`Auction ${kind} işlemi tamamlandı.`);
      setSelected(null);
      load();
    } catch (err) { setError(getApiErrorMessage(err, 'Auction işlemi başarısız.')); }
    finally { setSaving(false); }
  };

  const openTransition = (auction: AuctionSummaryDto | AuctionDetailDto, kind: TransitionKind) => {
    const useExistingDates = kind === 'schedule';
    setTransition({
      auctionId: auction.id,
      kind,
      startsAt: useExistingDates ? toDateTimeLocalValue(auction.startsAt) || defaultDateTimeLocal(10) : defaultDateTimeLocal(10),
      endsAt: useExistingDates ? toDateTimeLocalValue(auction.endsAt) || defaultDateTimeLocal(70) : defaultDateTimeLocal(70),
    });
  };

  const submitTransition = async () => {
    if (!transition) return;
    if (!hasValidDateRange(transition.startsAt, transition.endsAt)) { setError('Schedule/relist için bitiş tarihi başlangıç tarihinden sonra olmalıdır.'); return; }
    setSaving(true); setError(''); setMessage('');
    const payload = { startsAt: toBackendIso(transition.startsAt), endsAt: toBackendIso(transition.endsAt) };
    try {
      if (transition.kind === 'schedule') await adminApi.scheduleAuction(transition.auctionId, payload);
      if (transition.kind === 'relist') await adminApi.relistAuction(transition.auctionId, payload);
      setMessage(`Auction ${transition.kind} işlemi adminin seçtiği tarih/saat ile tamamlandı.`);
      setTransition(null);
      setSelected(null);
      load();
    } catch (err) { setError(getApiErrorMessage(err, 'Auction tarih işlemi başarısız.')); }
    finally { setSaving(false); }
  };

  return <div><div className="page-header"><div><h1 className="page-title">Açık Artırma Yönetimi</h1><p className="page-subtitle">Create/schedule/activate/update/finalize/cancel/relist admin endpointleri.</p></div><button className="btn btn-primary" onClick={() => { setForm(createEmptyForm()); setShowAdd(true); }}><Plus size={16} /> Yeni Auction</button></div>{error && <div className="error-banner" style={{ marginBottom: 16 }}>{error}</div>}{message && <div style={{ color: '#6ee7b7', marginBottom: 16 }}>{message}</div>}<div className="data-table-wrapper"><div className="data-table-header"><div className="search-bar" style={{ minWidth: 300 }}><Search size={15} /><input placeholder="Auction ara..." value={search} onChange={e => setSearch(e.target.value)} /></div></div>{loading ? <Empty text="Açık artırmalar yükleniyor..." /> : filtered.length === 0 ? <Empty text="Açık artırma bulunamadı." /> : <table className="data-table"><thead><tr><th>Ürün</th><th>Güncel</th><th>Min Teklif</th><th>Bitiş</th><th>Durum</th><th style={{ textAlign: 'right' }}>İşlem</th></tr></thead><tbody>{filtered.map(a => <tr key={a.id}><td><strong>{a.productName}</strong><div style={{ color: 'var(--text-muted)', fontSize: '0.78rem' }}>{a.sku}</div></td><td>{formatMoney(a.currentPrice, a.currency)}</td><td>{formatMoney(a.minimumNextBid, a.currency)}</td><td>{new Date(a.endsAt).toLocaleString('tr-TR')}</td><td><span className="badge badge-purple">{auctionStatusLabel[String(a.status)] ?? String(a.status)}</span></td><td style={{ textAlign: 'right' }}><ActionButtons auction={a} saving={saving} open={open} openTransition={openTransition} runImmediateAction={runImmediateAction} /></td></tr>)}</tbody></table>}</div>{showAdd && <AuctionModal title="Yeni Auction" form={form} setForm={setForm} listings={listings} saving={saving} allowListingChange close={() => setShowAdd(false)} submit={create} submitText="Oluştur" />}{selected && <AuctionModal title="Auction Detayı / Güncelle" form={form} setForm={setForm} listings={listings} saving={saving} allowListingChange={false} close={() => setSelected(null)} submit={update} submitText="Güncelle" extra={<><div style={{ display: 'grid', gap: 6, marginTop: 12, color: 'var(--text-secondary)' }}><span>Winning user: {selected.winningUserId ?? '-'}</span><span>Winning amount: {selected.winningBidAmount ? formatMoney(selected.winningBidAmount, selected.currency) : '-'}</span><span>Order: {selected.purchaseOrderId ?? '-'}</span><span>PaymentAttempt: {selected.paymentAttemptId ?? '-'}</span><span>Platform revenue: {formatMoney(selected.platformRevenueAmount, selected.currency)}</span></div><h3 style={{ marginTop: 16, marginBottom: 10 }}>Teklifler</h3>{bids.length === 0 ? <p style={{ color: 'var(--text-muted)' }}>Bid yok veya endpoint erişilemedi.</p> : <table className="data-table"><thead><tr><th>Bidder</th><th>Tutar</th><th>Durum</th><th>Tarih</th></tr></thead><tbody>{bids.map(b => <tr key={b.id}><td>{b.bidderUserId}</td><td>{formatMoney(b.amount, selected.currency)}</td><td>{auctionBidStatusLabel[String(b.status)] ?? String(b.status)}</td><td>{new Date(b.createdAt).toLocaleString('tr-TR')}</td></tr>)}</tbody></table>}<div style={{ marginTop: 14 }}><ActionButtons auction={selected} saving={saving} open={open} openTransition={openTransition} runImmediateAction={runImmediateAction} hideDetail /></div></>} />}{transition && <TransitionModal transition={transition} setTransition={setTransition} saving={saving} close={() => setTransition(null)} submit={submitTransition} />}</div>;
}

function ActionButtons({ auction, saving, open, openTransition, runImmediateAction, hideDetail = false }: { auction: AuctionSummaryDto | AuctionDetailDto; saving: boolean; open: (id: string) => void; openTransition: (auction: AuctionSummaryDto | AuctionDetailDto, kind: TransitionKind) => void; runImmediateAction: (id: string, kind: ImmediateAction) => void; hideDetail?: boolean }) {
  return <div style={{ display: 'flex', gap: 6, justifyContent: 'flex-end', flexWrap: 'wrap' }}>{!hideDetail && <button className="btn btn-ghost" onClick={() => open(auction.id)}><Eye size={14} /> Detay</button>}<button className="btn btn-ghost" disabled={saving} onClick={() => openTransition(auction, 'schedule')}>Schedule</button><button className="btn btn-ghost" disabled={saving} onClick={() => runImmediateAction(auction.id, 'activate')}>Activate</button><button className="btn btn-ghost" disabled={saving} onClick={() => runImmediateAction(auction.id, 'finalize')}>Finalize</button><button className="btn btn-ghost" disabled={saving} onClick={() => openTransition(auction, 'relist')}>Relist</button><button className="btn btn-ghost" disabled={saving} style={{ color: '#f87171' }} onClick={() => runImmediateAction(auction.id, 'cancel')}>Cancel</button></div>;
}

function AuctionModal({ title, form, setForm, listings, saving, close, submit, submitText, allowListingChange, extra }: { title: string; form: AuctionForm; setForm: Dispatch<SetStateAction<AuctionForm>>; listings: AdminProductListingSummaryDto[]; saving: boolean; close: () => void; submit: () => void; submitText: string; allowListingChange: boolean; extra?: ReactNode }) {
  return <div className="modal-overlay" onClick={close}><div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 720, maxHeight: '90vh', overflowY: 'auto' }}><Header title={title} close={close} /><Select label="ProductListing" value={form.productListingId} disabled={!allowListingChange} onChange={value => setForm(prev => ({ ...prev, productListingId: value }))} options={listings.map(l => ({ value: l.id, label: `${l.productName} - ${formatMoney(l.price, l.currency)}` }))} />{!allowListingChange && <p style={{ color: 'var(--text-muted)', fontSize: '0.78rem', marginTop: -4, marginBottom: 10 }}>Backend update DTO’su ProductListing değiştirmez; gerekiyorsa yeni auction oluşturulmalıdır.</p>}<Input label="Seller User Id (opsiyonel)" value={form.sellerUserId} onChange={value => setForm(prev => ({ ...prev, sellerUserId: value }))} /><Input label="Başlangıç Fiyatı" type="number" min="0.01" step="0.01" value={form.startingPrice} onChange={value => setForm(prev => ({ ...prev, startingPrice: value }))} /><Input label="Minimum Artış" type="number" min="0.01" step="0.01" value={form.minimumBidIncrement} onChange={value => setForm(prev => ({ ...prev, minimumBidIncrement: value }))} /><DateInput label="Başlangıç" value={form.startsAt} onChange={value => setForm(prev => ({ ...prev, startsAt: value }))} /><DateInput label="Bitiş" value={form.endsAt} onChange={value => setForm(prev => ({ ...prev, endsAt: value }))} /><Input label="Adet" type="number" min="1" step="1" value={form.quantity} onChange={value => setForm(prev => ({ ...prev, quantity: value }))} /><Input label="Currency" value={form.currency} onChange={value => setForm(prev => ({ ...prev, currency: value.toUpperCase() }))} /><p style={{ color: 'var(--text-muted)', fontSize: '0.8rem' }}>ReservePrice backend DTO’da yoktur. Anti-snipe son 5 dakika +5 dakika backend domain kuralıdır. Bid spam/rate limit endpoint’i yok; fake edilmedi.</p>{extra}<div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12, marginTop: 20 }}><button className="btn btn-ghost" onClick={close}>İptal</button><button className="btn btn-primary" disabled={saving} onClick={submit}><Check size={16} /> {submitText}</button></div></div></div>;
}

function TransitionModal({ transition, setTransition, saving, close, submit }: { transition: TransitionForm; setTransition: Dispatch<SetStateAction<TransitionForm | null>>; saving: boolean; close: () => void; submit: () => void }) {
  const title = transition.kind === 'schedule' ? 'Auction Schedule' : 'Auction Relist';
  return <div className="modal-overlay" onClick={close}><div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 520 }}><Header title={title} close={close} /><DateInput label="Başlangıç" value={transition.startsAt} onChange={value => setTransition(prev => prev ? { ...prev, startsAt: value } : prev)} /><DateInput label="Bitiş" value={transition.endsAt} onChange={value => setTransition(prev => prev ? { ...prev, endsAt: value } : prev)} /><p style={{ color: 'var(--text-muted)', fontSize: '0.8rem' }}>Varsayılan tarih önerisi sadece başlangıç değeridir. Submit payload adminin elle seçtiği lokal tarih/saatin ISO karşılığı olarak gönderilir.</p><div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12, marginTop: 20 }}><button className="btn btn-ghost" onClick={close}>İptal</button><button className="btn btn-primary" disabled={saving} onClick={submit}><Check size={16} /> Gönder</button></div></div></div>;
}

function Empty({ text }: { text: string }) { return <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}><Gavel size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>; }
function Header({ title, close }: { title: string; close: () => void }) { return <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}><h2>{title}</h2><button className="btn btn-ghost" style={{ padding: 4 }} onClick={close}><X size={18} /></button></div>; }
function Input({ label, value, onChange, type = 'text', min, step }: { label: string; value: string; onChange: (value: string) => void; type?: string; min?: string; step?: string }) { return <div className="form-group" style={{ marginBottom: 10 }}><label className="form-label">{label}</label><input className="form-input" type={type} min={min} step={step} value={value} onChange={event => onChange(event.target.value)} /></div>; }
function DateInput({ label, value, onChange }: { label: string; value: string; onChange: (value: string) => void }) { return <div className="form-group" style={{ marginBottom: 10 }}><label className="form-label">{label}</label><input type="datetime-local" className="form-input" value={value} onChange={event => onChange(event.target.value)} /></div>; }
function Select({ label, value, onChange, options, disabled }: { label: string; value: string; onChange: (value: string) => void; options: Array<{ value: string; label: string }>; disabled?: boolean }) { return <div className="form-group" style={{ marginBottom: 10 }}><label className="form-label">{label}</label><select className="form-input" style={{ appearance: 'auto' }} value={value} disabled={disabled} onChange={event => onChange(event.target.value)}><option value="">Seçiniz</option>{options.map(option => <option key={option.value} value={option.value}>{option.label}</option>)}</select></div>; }
