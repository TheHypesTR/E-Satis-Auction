import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { MapPin, CreditCard, CheckCircle2, ChevronRight, Lock, AlertCircle, Clock, XCircle } from 'lucide-react';
import { useCart } from '../../context/CartContext';
import { commerceApi } from '../../services/commerceApi';
import { formatMoney, getApiErrorMessage, makeIdempotencyKey } from '../../services/apiUtils';
import type { CartPricePreviewDto, PaymentInitiationDto } from '../../types/commerce';

type Step = 'address' | 'payment' | 'review' | 'simulate';
const steps: { key: Step; label: string; icon: typeof MapPin }[] = [
  { key: 'address', label: 'Teslimat Adresi', icon: MapPin },
  { key: 'payment', label: 'Simüle Kart', icon: CreditCard },
  { key: 'review', label: 'Önizleme', icon: CheckCircle2 },
  { key: 'simulate', label: 'Ödeme Sonucu', icon: Clock },
];

interface AddressForm { fullName: string; phone: string; city: string; district: string; address: string; zip: string; }
interface CardForm { cardHolder: string; cardNumber: string; expiry: string; cvv: string; }

function formatCardNumber(raw: string) { return raw.replace(/\D/g, '').slice(0, 16).replace(/(.{4})/g, '$1 ').trim(); }
function formatExpiry(raw: string) { const digits = raw.replace(/\D/g, '').slice(0, 4); return digits.length > 2 ? `${digits.slice(0, 2)}/${digits.slice(2)}` : digits; }
function cardBrand(num: string): string { const d = num.replace(/\s/g, ''); if (/^4/.test(d)) return 'Visa'; if (/^5[1-5]/.test(d) || /^2[2-7]/.test(d)) return 'Mastercard'; if (/^3[47]/.test(d)) return 'Amex'; return ''; }

export default function UserCheckout() {
  const navigate = useNavigate();
  const { items, clearCart } = useCart();
  const [step, setStep] = useState<Step>('address');
  const [loading, setLoading] = useState(false);
  const [preview, setPreview] = useState<CartPricePreviewDto | null>(null);
  const [payment, setPayment] = useState<PaymentInitiationDto | null>(null);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [addrForm, setAddrForm] = useState<AddressForm>({ fullName: '', phone: '', city: '', district: '', address: '', zip: '' });
  const [cardForm, setCardForm] = useState<CardForm>({ cardHolder: '', cardNumber: '', expiry: '', cvv: '' });
  const [cardError, setCardError] = useState('');

  useEffect(() => {
    if (items.length === 0) return;
    const item = items[0];
    commerceApi.updateCart(item.id, item.quantity)
      .then(() => commerceApi.getCartPreview())
      .then(setPreview)
      .catch(err => setError(getApiErrorMessage(err, 'Checkout fiyat önizlemesi alınamadı.')));
  }, [items]);

  const stepIndex = steps.findIndex(s => s.key === step);
  const brand = cardBrand(cardForm.cardNumber);
  const subtotal = preview?.subtotalAmount ?? items.reduce((s, i) => s + i.price * i.quantity, 0);
  const shipping = preview?.shippingAmount ?? 0;
  const discount = preview?.discountAmount ?? 0;
  const total = preview?.totalAmount ?? subtotal;
  const currency = preview?.currency ?? 'TRY';

  const validateAddress = () => Boolean(addrForm.fullName && addrForm.phone && addrForm.city && addrForm.district && addrForm.address);
  const validateCard = () => {
    const num = cardForm.cardNumber.replace(/\s/g, '');
    if (num.length < 16) { setCardError('Demo kart numarası 16 haneli olmalıdır.'); return false; }
    if (!cardForm.cardHolder.trim()) { setCardError('Kart üzerindeki isim gerekli.'); return false; }
    if (cardForm.expiry.length < 5) { setCardError('Son kullanma tarihi geçersiz.'); return false; }
    if (cardForm.cvv.length < 3) { setCardError('CVV geçersiz.'); return false; }
    setCardError(''); return true;
  };

  const handleNext = () => {
    if (step === 'address' && !validateAddress()) { setError('Teslimat bilgilerini doldurun.'); return; }
    if (step === 'payment' && !validateCard()) return;
    setError('');
    const idx = steps.findIndex(s => s.key === step);
    if (idx < 2) setStep(steps[idx + 1].key);
  };

  const initiate = async () => {
    if (items.length === 0) return;
    setLoading(true); setError(''); setMessage('');
    try {
      const item = items[0];
      await commerceApi.updateCart(item.id, item.quantity);
      const result = await commerceApi.initiatePayment(makeIdempotencyKey('checkout'));
      setPayment(result);
      setStep('simulate');
      setMessage('Ödeme denemesi başlatıldı. Stok 15 dakika için rezerve edildi; gerçek ödeme sağlayıcısı yerine demo confirm/fail butonları kullanılır.');
    } catch (err) {
      setError(getApiErrorMessage(err, 'Ödeme denemesi başlatılamadı.'));
    } finally { setLoading(false); }
  };

  const confirm = async () => {
    if (!payment) return;
    setLoading(true); setError('');
    try {
      await commerceApi.confirmPayment(payment.payment.id, makeIdempotencyKey('confirm'));
      clearCart();
      navigate('/user/order-success', { state: { order: payment.order, payment: payment.payment, mode: 'paymentConfirm' } });
    } catch (err) { setError(getApiErrorMessage(err, 'Ödeme simülasyonu onaylanamadı.')); }
    finally { setLoading(false); }
  };

  const fail = async () => {
    if (!payment) return;
    setLoading(true); setError('');
    try {
      await commerceApi.failPayment(payment.payment.id, makeIdempotencyKey('fail'), 'Demo kullanıcı başarısız ödeme sonucu seçti.');
      clearCart();
      setMessage('Ödeme başarısız olarak işaretlendi. Backend rezerve edilen stoğu geri bırakır.');
    } catch (err) { setError(getApiErrorMessage(err, 'Ödeme fail sonucu işlenemedi.')); }
    finally { setLoading(false); }
  };

  if (items.length === 0 && !payment) return <div className="user-page" style={{ textAlign: 'center', paddingTop: 80 }}><p style={{ color: 'var(--text-muted)' }}>Sepetiniz boş.</p><button className="btn btn-primary" style={{ marginTop: 20 }} onClick={() => navigate('/user/catalog')}>Alışverişe Git</button></div>;

  return (
    <div className="user-page">
      <div style={{ marginBottom: 36 }}><div className="user-section-label">Ödeme Akışı</div><h1 style={{ fontSize: '1.9rem', fontWeight: 700 }}>Simüle Ödeme ile Siparişi Tamamla</h1></div>
      <div className="user-stepper">
        {steps.map((s, i) => { const Icon = s.icon; const done = i < stepIndex; const current = i === stepIndex; return <div key={s.key} className="user-stepper-item"><div className={`user-stepper-circle ${done ? 'done' : current ? 'current' : ''}`}><Icon size={18} /></div><span className={`user-stepper-label ${current ? 'current' : done ? 'done' : ''}`}>{s.label}</span>{i < steps.length - 1 && <div className={`user-stepper-line ${i < stepIndex ? 'done' : ''}`} />}</div>; })}
      </div>
      <div className="user-checkout-grid">
        <div className="glass-card animate-fade-up" style={{ padding: 32 }}>
          {error && <div className="error-banner" style={{ marginBottom: 16 }}><AlertCircle size={15} /> {error}</div>}
          {message && <div style={{ marginBottom: 16, padding: '12px 16px', color: '#6ee7b7', background: 'rgba(16,185,129,0.08)', borderRadius: 10, border: '1px solid rgba(16,185,129,0.15)' }}>{message}</div>}
          {step === 'address' && <AddressStep addrForm={addrForm} setAddrForm={setAddrForm} />}
          {step === 'payment' && <PaymentStep cardForm={cardForm} setCardForm={setCardForm} brand={brand} cardError={cardError} />}
          {step === 'review' && <ReviewStep items={items} addrForm={addrForm} brand={brand} cardForm={cardForm} />}
          {step === 'simulate' && payment && <div><h2 style={{ fontSize: '1.2rem', fontWeight: 700, marginBottom: 18, display: 'flex', alignItems: 'center', gap: 10 }}><Clock size={20} color="#a78bfa" /> PaymentAttempt Oluştu</h2><p style={{ color: 'var(--text-secondary)', lineHeight: 1.7, marginBottom: 16 }}>PaymentAttempt: <code>{payment.payment.id}</code><br />Sipariş: <code>{payment.order.orderNumber}</code><br />Rezervasyon bitişi: {new Date(payment.payment.expiresAt).toLocaleString('tr-TR')}</p><div style={{ display: 'flex', gap: 12, flexWrap: 'wrap' }}><button className="btn btn-primary" onClick={confirm} disabled={loading}><CheckCircle2 size={16} /> Demo Confirm</button><button className="btn btn-ghost" style={{ color: '#f87171', borderColor: 'rgba(248,113,113,0.3)' }} onClick={fail} disabled={loading}><XCircle size={16} /> Demo Fail</button></div><p style={{ marginTop: 16, color: 'var(--text-muted)', fontSize: '0.82rem' }}>Confirm gerçek provider callback değildir; demo/sunum için simüle ödeme sonucudur.</p></div>}
          <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: 32, gap: 12 }}>
            <button className="btn btn-ghost" onClick={() => { const i = steps.findIndex(s => s.key === step); if (i > 0 && step !== 'simulate') setStep(steps[i - 1].key); else navigate('/user/cart'); }}>Geri</button>
            {step === 'address' || step === 'payment' ? <button className="btn btn-primary" style={{ gap: 6 }} onClick={handleNext}>Devam Et <ChevronRight size={16} /></button> : step === 'review' ? <button className="btn btn-primary" style={{ gap: 6, padding: '12px 28px' }} disabled={loading} onClick={initiate}>{loading ? 'Başlatılıyor...' : <><Lock size={15} /> PaymentAttempt Başlat</>}</button> : <button className="btn btn-ghost" onClick={() => navigate('/user/profile')}>Siparişlerime Git</button>}
          </div>
        </div>
        <div className="user-order-summary"><h3 style={{ fontSize: '1rem', fontWeight: 700, marginBottom: 16 }}>Backend Fiyat Önizleme</h3><div style={{ display: 'flex', flexDirection: 'column', gap: 10, marginBottom: 16 }}><div className="user-summary-row"><span>Ara Toplam</span><span>{formatMoney(subtotal, currency)}</span></div><div className="user-summary-row"><span>İndirim</span><span>-{formatMoney(discount, currency)}</span></div><div className="user-summary-row"><span>Kargo</span><span style={{ color: shipping === 0 ? '#6ee7b7' : undefined }}>{shipping === 0 ? 'Ücretsiz' : formatMoney(shipping, currency)}</span></div></div><div className="glow-divider" style={{ marginBottom: 16 }} /><div className="user-summary-row"><span style={{ fontWeight: 700 }}>Toplam</span><span style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1.2rem', background: 'linear-gradient(135deg, #a78bfa, #60a5fa)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent', backgroundClip: 'text' }}>{formatMoney(total, currency)}</span></div><div style={{ marginTop: 20, fontSize: '0.78rem', color: 'var(--text-muted)' }}>Sepet backend modelinde tek listing tutar. Frozen fiyat sipariş/payment oluştuğunda order snapshot'a yazılır.</div></div>
      </div>
    </div>
  );
}

function AddressStep({ addrForm, setAddrForm }: { addrForm: AddressForm; setAddrForm: React.Dispatch<React.SetStateAction<AddressForm>> }) {
  return <div><h2 style={{ fontSize: '1.2rem', fontWeight: 700, marginBottom: 24, display: 'flex', alignItems: 'center', gap: 10 }}><MapPin size={20} color="#a78bfa" /> Teslimat Adresi</h2><div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}><Input label="Ad Soyad" value={addrForm.fullName} onChange={v => setAddrForm(p => ({ ...p, fullName: v }))} span /><Input label="Telefon" value={addrForm.phone} onChange={v => setAddrForm(p => ({ ...p, phone: v }))} /><Input label="Posta Kodu" value={addrForm.zip} onChange={v => setAddrForm(p => ({ ...p, zip: v }))} /><Input label="İl" value={addrForm.city} onChange={v => setAddrForm(p => ({ ...p, city: v }))} /><Input label="İlçe" value={addrForm.district} onChange={v => setAddrForm(p => ({ ...p, district: v }))} /><Input label="Açık Adres" value={addrForm.address} onChange={v => setAddrForm(p => ({ ...p, address: v }))} span /></div></div>;
}
function PaymentStep({ cardForm, setCardForm, brand, cardError }: { cardForm: CardForm; setCardForm: React.Dispatch<React.SetStateAction<CardForm>>; brand: string; cardError: string }) {
  return <div><h2 style={{ fontSize: '1.2rem', fontWeight: 700, marginBottom: 24, display: 'flex', alignItems: 'center', gap: 10 }}><CreditCard size={20} color="#a78bfa" /> Simüle Kart Bilgileri</h2><div className="user-card-preview"><div style={{ display: 'flex', justifyContent: 'space-between' }}><div style={{ fontSize: '0.75rem', opacity: 0.7, letterSpacing: '0.1em' }}>E-SATIS PAY DEMO</div><div style={{ fontWeight: 700 }}>{brand || 'DEMO'}</div></div><div className="user-card-number">{(cardForm.cardNumber || '0000 0000 0000 0000').padEnd(19, '0')}</div><div>{cardForm.cardHolder.toUpperCase() || 'DEMO KULLANICI'}</div></div>{cardError && <div className="error-banner" style={{ marginBottom: 16 }}><AlertCircle size={15} /> {cardError}</div>}<div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}><Input label="Kart Numarası" value={cardForm.cardNumber} onChange={v => setCardForm(p => ({ ...p, cardNumber: formatCardNumber(v) }))} span /><Input label="Kart Üzerindeki İsim" value={cardForm.cardHolder} onChange={v => setCardForm(p => ({ ...p, cardHolder: v }))} span /><Input label="Son Kullanma" value={cardForm.expiry} onChange={v => setCardForm(p => ({ ...p, expiry: formatExpiry(v) }))} /><Input label="CVV" value={cardForm.cvv} onChange={v => setCardForm(p => ({ ...p, cvv: v.replace(/\D/g, '').slice(0, 4) }))} /></div><p style={{ marginTop: 16, fontSize: '0.78rem', color: 'var(--text-muted)' }}>Bu form gerçek sanal POS entegrasyonu değildir; backend simüle PaymentAttempt state machine kullanır.</p></div>;
}
function ReviewStep({ items, addrForm, brand, cardForm }: { items: ReturnType<typeof useCart>['items']; addrForm: AddressForm; brand: string; cardForm: CardForm }) {
  return <div><h2 style={{ fontSize: '1.2rem', fontWeight: 700, marginBottom: 24, display: 'flex', alignItems: 'center', gap: 10 }}><CheckCircle2 size={20} color="#a78bfa" /> Sipariş Özeti</h2>{items.map(item => <div key={item.id} style={{ display: 'flex', justifyContent: 'space-between', padding: '12px 0', borderBottom: '1px solid var(--glass-border)' }}><div><div style={{ fontWeight: 500 }}>{item.name}</div><div style={{ fontSize: '0.78rem', color: 'var(--text-muted)' }}>{item.quantity} adet x {formatMoney(item.price)}</div></div><strong>{formatMoney(item.price * item.quantity)}</strong></div>)}<div style={{ background: 'rgba(0,0,0,0.2)', borderRadius: 12, padding: '16px 20px', marginTop: 24 }}><div style={{ color: 'var(--text-muted)', marginBottom: 8 }}>Teslimat Adresi</div><p>{addrForm.fullName}<br />{addrForm.address}, {addrForm.district}/{addrForm.city} {addrForm.zip}<br />{addrForm.phone}</p></div><div style={{ background: 'rgba(0,0,0,0.2)', borderRadius: 12, padding: '16px 20px', marginTop: 16 }}><div style={{ color: 'var(--text-muted)', marginBottom: 8 }}>Ödeme Yöntemi</div><p>{brand || 'Demo Kart'} - **** {cardForm.cardNumber.replace(/\s/g, '').slice(-4)}</p></div></div>;
}
function Input({ label, value, onChange, span }: { label: string; value: string; onChange: (value: string) => void; span?: boolean }) {
  return <div className="form-group" style={span ? { gridColumn: 'span 2' } : undefined}><label className="form-label">{label}</label><input className="form-input" value={value} onChange={e => onChange(e.target.value)} /></div>;
}
