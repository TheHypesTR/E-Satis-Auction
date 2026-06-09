/* eslint-disable react-hooks/set-state-in-effect, react-hooks/exhaustive-deps */
import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { AlertCircle, ArrowLeft, CheckCircle2, Clock, Gavel, Package, XCircle } from 'lucide-react';
import { commerceApi } from '../../services/commerceApi';
import { auctionStatusLabel, formatMoney, getApiErrorMessage, isActiveAuction, isAuctionPaymentPending, makeIdempotencyKey, requireAuth } from '../../services/apiUtils';
import type { AuctionBidDto, AuctionDetailDto, PaymentInitiationDto } from '../../types/commerce';

export default function UserAuctionDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [auction, setAuction] = useState<AuctionDetailDto | null>(null);
  const [bids, setBids] = useState<AuctionBidDto[]>([]);
  const [payment, setPayment] = useState<PaymentInitiationDto | null>(null);
  const [amount, setAmount] = useState('');
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');

  const load = async () => {
    if (!id) return;
    setError('');
    try {
      const [detail, bidList, winnerInfo] = await Promise.all([commerceApi.getAuction(id), commerceApi.getAuctionBids(id, 20), commerceApi.getAuctionWinner(id).catch(() => null)]);
      setAuction(detail); setBids(bidList); if (winnerInfo?.winningUserId) setMessage('Kazanan kullanıcı: ' + winnerInfo.winningUserId); setAmount(String(detail.minimumNextBid));
    } catch (err) { setError(getApiErrorMessage(err, 'Açık artırma detayı yüklenemedi.')); }
    finally { setLoading(false); }
  };

  useEffect(() => { void load(); }, [id]);

  const placeBid = async () => {
    if (!auction || !id || !requireAuth(navigate)) return;
    const numeric = Number(amount);
    if (!Number.isFinite(numeric) || numeric < auction.minimumNextBid) { setError(`Minimum teklif ${formatMoney(auction.minimumNextBid, auction.currency)} olmalıdır.`); return; }
    setActionLoading(true); setError(''); setMessage('');
    try {
      await commerceApi.placeBid(id, numeric, makeIdempotencyKey('bid'));
      setMessage('Teklifiniz alındı. Son 5 dakikada verilen geçerli teklifler süreyi 5 dakika uzatabilir.');
      await load();
    } catch (err) { setError(getApiErrorMessage(err, 'Teklif verilemedi.')); }
    finally { setActionLoading(false); }
  };

  const initiateWinnerPayment = async () => {
    if (!id || !requireAuth(navigate)) return;
    setActionLoading(true); setError(''); setMessage('');
    try {
      const result = await commerceApi.initiateAuctionPayment(id, makeIdempotencyKey('auction_payment'));
      setPayment(result);
      setMessage('Kazanan ödeme denemesi başlatıldı. Stok ödeme süresince sipariş akışına aktarılır.');
    } catch (err) { setError(getApiErrorMessage(err, 'Kazanan ödeme akışı başlatılamadı. Kazanan kullanıcı değilseniz backend bu işlemi engeller.')); }
    finally { setActionLoading(false); }
  };

  const confirmPayment = async () => {
    if (!payment) return;
    setActionLoading(true);
    try {
      await commerceApi.confirmPayment(payment.payment.id, makeIdempotencyKey('auction_confirm'));
      navigate('/user/order-success', { state: { order: payment.order, payment: payment.payment, mode: 'auctionPaymentConfirm' } });
    } catch (err) { setError(getApiErrorMessage(err, 'Açık artırma ödeme simülasyonu onaylanamadı.')); }
    finally { setActionLoading(false); }
  };

  const failPayment = async () => {
    if (!payment) return;
    setActionLoading(true);
    try {
      await commerceApi.failPayment(payment.payment.id, makeIdempotencyKey('auction_fail'), 'Demo açık artırma ödeme fail.');
      setMessage('Ödeme başarısız işaretlendi. Backend auction payment fail/expire akışında stok release davranışını uygular.');
      await load();
    } catch (err) { setError(getApiErrorMessage(err, 'Açık artırma ödeme fail sonucu işlenemedi.')); }
    finally { setActionLoading(false); }
  };

  if (loading) return <Centered icon={Package} text="Yükleniyor..." />;
  if (!auction) return <Centered icon={AlertCircle} text={error || 'Açık artırma bulunamadı.'} />;
  const active = isActiveAuction(auction.status);
  const paymentPending = isAuctionPaymentPending(auction.status);

  return <div className="user-page"><button className="btn btn-ghost" style={{ marginBottom: 28, gap: 6 }} onClick={() => navigate(-1)}><ArrowLeft size={15} /> Geri</button><div className="user-detail-grid"><div><div className="user-detail-img" style={{ background: 'linear-gradient(135deg, #7c3aed 0%, #f59e0b 100%)' }}><Gavel size={84} color="rgba(255,255,255,0.55)" /></div><div className="glass-card" style={{ marginTop: 20, padding: 18 }}><h3 style={{ marginBottom: 12 }}>Son Teklifler</h3>{bids.length === 0 ? <p style={{ color: 'var(--text-muted)' }}>Henüz teklif yok.</p> : bids.map(b => <div key={b.id} style={{ display: 'flex', justifyContent: 'space-between', padding: '10px 0', borderBottom: '1px solid var(--glass-border)' }}><span style={{ color: 'var(--text-muted)' }}>{new Date(b.createdAt).toLocaleTimeString('tr-TR')}</span><strong>{formatMoney(b.amount, auction.currency)}</strong></div>)}</div></div><div><span className="badge badge-amber" style={{ marginBottom: 12 }}>{auctionStatusLabel[String(auction.status)] ?? String(auction.status)}</span><h1 style={{ fontSize: '1.8rem', fontWeight: 700, marginBottom: 8 }}>{auction.productName}</h1><code style={{ color: 'var(--text-muted)' }}>{auction.sku}</code><div className="glow-divider" style={{ margin: '24px 0' }} /><div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16, marginBottom: 24 }}><Metric label="Güncel fiyat" value={formatMoney(auction.currentPrice, auction.currency)} /><Metric label="Minimum sonraki teklif" value={formatMoney(auction.minimumNextBid, auction.currency)} /><Metric label="Bitiş" value={new Date(auction.endsAt).toLocaleString('tr-TR')} /><Metric label="Anti-snipe" value="Son 5 dk teklif gelirse +5 dk" /></div>{error && <div className="error-banner" style={{ marginBottom: 16 }}><AlertCircle size={15} /> {error}</div>}{message && <div style={{ marginBottom: 16, padding: '12px 16px', color: '#6ee7b7', background: 'rgba(16,185,129,0.08)', borderRadius: 10, border: '1px solid rgba(16,185,129,0.15)' }}>{message}</div>}{active && <div className="glass-card" style={{ padding: 18, marginBottom: 18 }}><label className="form-label">Teklif Tutarı</label><div style={{ display: 'flex', gap: 10 }}><input className="form-input" type="number" value={amount} onChange={e => setAmount(e.target.value)} min={auction.minimumNextBid} /><button className="btn btn-primary" onClick={placeBid} disabled={actionLoading}>Teklif Ver</button></div><p style={{ color: 'var(--text-muted)', fontSize: '0.78rem', marginTop: 10 }}>Frontend minimum teklif ön kontrolü yapar; asıl kontrol ve idempotency backend’dedir.</p></div>}{paymentPending && <div className="glass-card" style={{ padding: 18 }}><h3 style={{ marginBottom: 10 }}>Kazanan Ödeme Akışı</h3><p style={{ color: 'var(--text-muted)', fontSize: '0.88rem', marginBottom: 14 }}>Kazanan dışındaki kullanıcılar backend tarafından engellenir. Buton deneme amaçlıdır.</p>{!payment ? <button className="btn btn-primary" onClick={initiateWinnerPayment} disabled={actionLoading}><Clock size={16} /> Kazanan Ödemesini Başlat</button> : <div style={{ display: 'flex', gap: 10, flexWrap: 'wrap' }}><button className="btn btn-primary" onClick={confirmPayment} disabled={actionLoading}><CheckCircle2 size={16} /> Demo Confirm</button><button className="btn btn-ghost" style={{ color: '#f87171', borderColor: 'rgba(248,113,113,0.3)' }} onClick={failPayment} disabled={actionLoading}><XCircle size={16} /> Demo Fail</button></div>}</div>}</div></div></div>;
}
function Metric({ label, value }: { label: string; value: string }) { return <div style={{ padding: 14, border: '1px solid var(--glass-border)', borderRadius: 12 }}><div style={{ color: 'var(--text-muted)', fontSize: '0.76rem', marginBottom: 6 }}>{label}</div><strong style={{ color: 'var(--text-primary)' }}>{value}</strong></div>; }
function Centered({ icon: Icon, text }: { icon: typeof Package; text: string }) { return <div className="user-page" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: 400 }}><div style={{ textAlign: 'center', color: 'var(--text-muted)' }}><Icon size={44} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div></div>; }




