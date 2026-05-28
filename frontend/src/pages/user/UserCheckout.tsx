import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { MapPin, CreditCard, CheckCircle2, ChevronRight, Lock, AlertCircle } from 'lucide-react';
import { useCart } from '../../context/CartContext';

type Step = 'address' | 'payment' | 'review';

const steps: { key: Step; label: string; icon: typeof MapPin }[] = [
  { key: 'address', label: 'Teslimat Adresi', icon: MapPin },
  { key: 'payment', label: 'Ödeme Bilgileri', icon: CreditCard },
  { key: 'review',  label: 'Sipariş Özeti',   icon: CheckCircle2 },
];

interface AddressForm {
  fullName: string;
  phone: string;
  city: string;
  district: string;
  address: string;
  zip: string;
}

interface CardForm {
  cardHolder: string;
  cardNumber: string;
  expiry: string;
  cvv: string;
}

function formatCardNumber(raw: string) {
  return raw.replace(/\D/g, '').slice(0, 16).replace(/(.{4})/g, '$1 ').trim();
}

function formatExpiry(raw: string) {
  const digits = raw.replace(/\D/g, '').slice(0, 4);
  if (digits.length > 2) return `${digits.slice(0, 2)}/${digits.slice(2)}`;
  return digits;
}

function cardBrand(num: string): string {
  const d = num.replace(/\s/g, '');
  if (/^4/.test(d)) return 'Visa';
  if (/^5[1-5]/.test(d) || /^2[2-7]/.test(d)) return 'Mastercard';
  if (/^3[47]/.test(d)) return 'Amex';
  return '';
}

export default function UserCheckout() {
  const navigate = useNavigate();
  const { items, totalPrice, clearCart } = useCart();

  const [step, setStep]   = useState<Step>('address');
  const [loading, setLoading] = useState(false);
  const [addrForm, setAddrForm] = useState<AddressForm>({
    fullName: '', phone: '', city: '', district: '', address: '', zip: '',
  });
  const [cardForm, setCardForm] = useState<CardForm>({
    cardHolder: '', cardNumber: '', expiry: '', cvv: '',
  });
  const [cardError, setCardError] = useState('');

  const shipping   = totalPrice >= 500 ? 0 : 49;
  const tax        = totalPrice * 0.18;
  const grandTotal = totalPrice + shipping + tax;

  const stepIndex   = steps.findIndex(s => s.key === step);
  const brand       = cardBrand(cardForm.cardNumber);

  const validateAddress = (): boolean =>
    !!(addrForm.fullName && addrForm.phone && addrForm.city && addrForm.district && addrForm.address);

  const validateCard = (): boolean => {
    const num = cardForm.cardNumber.replace(/\s/g, '');
    if (num.length < 16) { setCardError('Kart numarası 16 haneli olmalıdır.'); return false; }
    if (!cardForm.cardHolder.trim()) { setCardError('Kart üzerindeki isim gerekli.'); return false; }
    if (cardForm.expiry.length < 5) { setCardError('Son kullanma tarihi geçersiz.'); return false; }
    if (cardForm.cvv.length < 3) { setCardError('CVV geçersiz.'); return false; }
    setCardError('');
    return true;
  };

  const handleNext = () => {
    if (step === 'address' && !validateAddress()) return;
    if (step === 'payment' && !validateCard()) return;
    const idx = steps.findIndex(s => s.key === step);
    if (idx < steps.length - 1) setStep(steps[idx + 1].key);
  };

  const handleSubmit = () => {
    setLoading(true);
    setTimeout(() => {
      clearCart();
      navigate('/user/order-success');
    }, 1800);
  };

  if (items.length === 0) {
    return (
      <div className="user-page" style={{ textAlign: 'center', paddingTop: 80 }}>
        <p style={{ color: 'var(--text-muted)' }}>Sepetiniz boş.</p>
        <button className="btn btn-primary" style={{ marginTop: 20 }} onClick={() => navigate('/user/catalog')}>Alışverişe Git</button>
      </div>
    );
  }

  return (
    <div className="user-page">
      <div style={{ marginBottom: 36 }}>
        <div className="user-section-label">Ödeme Akışı</div>
        <h1 style={{ fontSize: '1.9rem', fontWeight: 700 }}>Siparişi Tamamla</h1>
      </div>

      {/* Stepper */}
      <div className="user-stepper">
        {steps.map((s, i) => {
          const Icon     = s.icon;
          const done     = i < stepIndex;
          const current  = i === stepIndex;
          return (
            <div key={s.key} className="user-stepper-item">
              <div
                className={`user-stepper-circle ${done ? 'done' : current ? 'current' : ''}`}
                onClick={() => done && setStep(s.key)}
                style={{ cursor: done ? 'pointer' : 'default' }}
              >
                {done ? <CheckCircle2 size={18} /> : <Icon size={18} />}
              </div>
              <span className={`user-stepper-label ${current ? 'current' : done ? 'done' : ''}`}>{s.label}</span>
              {i < steps.length - 1 && <div className={`user-stepper-line ${i < stepIndex ? 'done' : ''}`} />}
            </div>
          );
        })}
      </div>

      <div className="user-checkout-grid">
        {/* Left: step content */}
        <div className="glass-card animate-fade-up" style={{ padding: '32px' }}>

          {/* ── Step 1: Address ── */}
          {step === 'address' && (
            <div>
              <h2 style={{ fontSize: '1.2rem', fontWeight: 700, marginBottom: 24, display: 'flex', alignItems: 'center', gap: 10 }}>
                <MapPin size={20} color="#a78bfa" /> Teslimat Adresi
              </h2>
              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
                <div className="form-group" style={{ gridColumn: 'span 2' }}>
                  <label className="form-label">Ad Soyad</label>
                  <input className="form-input" placeholder="Ahmet Yılmaz" value={addrForm.fullName}
                    onChange={e => setAddrForm(p => ({ ...p, fullName: e.target.value }))} />
                </div>
                <div className="form-group">
                  <label className="form-label">Telefon</label>
                  <input className="form-input" placeholder="0532 000 00 00" value={addrForm.phone}
                    onChange={e => setAddrForm(p => ({ ...p, phone: e.target.value }))} />
                </div>
                <div className="form-group">
                  <label className="form-label">Posta Kodu</label>
                  <input className="form-input" placeholder="34000" value={addrForm.zip}
                    onChange={e => setAddrForm(p => ({ ...p, zip: e.target.value }))} />
                </div>
                <div className="form-group">
                  <label className="form-label">İl</label>
                  <input className="form-input" placeholder="İstanbul" value={addrForm.city}
                    onChange={e => setAddrForm(p => ({ ...p, city: e.target.value }))} />
                </div>
                <div className="form-group">
                  <label className="form-label">İlçe</label>
                  <input className="form-input" placeholder="Kadıköy" value={addrForm.district}
                    onChange={e => setAddrForm(p => ({ ...p, district: e.target.value }))} />
                </div>
                <div className="form-group" style={{ gridColumn: 'span 2' }}>
                  <label className="form-label">Açık Adres</label>
                  <input className="form-input" placeholder="Mahalle, Sokak, No..." value={addrForm.address}
                    onChange={e => setAddrForm(p => ({ ...p, address: e.target.value }))} />
                </div>
              </div>
            </div>
          )}

          {/* ── Step 2: Payment ── */}
          {step === 'payment' && (
            <div>
              <h2 style={{ fontSize: '1.2rem', fontWeight: 700, marginBottom: 24, display: 'flex', alignItems: 'center', gap: 10 }}>
                <CreditCard size={20} color="#a78bfa" /> Kart Bilgileri
              </h2>

              {/* Card preview */}
              <div className="user-card-preview">
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <div style={{ fontSize: '0.75rem', opacity: 0.7, letterSpacing: '0.1em' }}>E-SATİS PAY</div>
                  <div style={{ fontWeight: 700, fontSize: '0.9rem', opacity: 0.9 }}>{brand || '●●●●'}</div>
                </div>
                <div className="user-card-number">
                  {(cardForm.cardNumber || '•••• •••• •••• ••••').padEnd(19, '•')}
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end' }}>
                  <div>
                    <div style={{ fontSize: '0.65rem', opacity: 0.6, letterSpacing: '0.08em', marginBottom: 2 }}>KART SAHİBİ</div>
                    <div style={{ fontSize: '0.9rem', fontWeight: 600, letterSpacing: '0.05em' }}>
                      {cardForm.cardHolder.toUpperCase() || 'AD SOYAD'}
                    </div>
                  </div>
                  <div>
                    <div style={{ fontSize: '0.65rem', opacity: 0.6, letterSpacing: '0.08em', marginBottom: 2 }}>SON KULLANMA</div>
                    <div style={{ fontSize: '0.9rem', fontWeight: 600 }}>{cardForm.expiry || 'MM/YY'}</div>
                  </div>
                </div>
              </div>

              {cardError && (
                <div className="error-banner" style={{ marginBottom: 16 }}>
                  <AlertCircle size={15} /> {cardError}
                </div>
              )}

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
                <div className="form-group" style={{ gridColumn: 'span 2' }}>
                  <label className="form-label">Kart Numarası</label>
                  <input className="form-input" placeholder="1234 5678 9012 3456"
                    value={cardForm.cardNumber}
                    onChange={e => setCardForm(p => ({ ...p, cardNumber: formatCardNumber(e.target.value) }))}
                    maxLength={19} style={{ letterSpacing: '0.08em', fontFamily: 'monospace' }} />
                </div>
                <div className="form-group" style={{ gridColumn: 'span 2' }}>
                  <label className="form-label">Kart Üzerindeki İsim</label>
                  <input className="form-input" placeholder="AHMET YILMAZ"
                    value={cardForm.cardHolder}
                    onChange={e => setCardForm(p => ({ ...p, cardHolder: e.target.value }))}
                    style={{ textTransform: 'uppercase' }} />
                </div>
                <div className="form-group">
                  <label className="form-label">Son Kullanma Tarihi</label>
                  <input className="form-input" placeholder="MM/YY"
                    value={cardForm.expiry}
                    onChange={e => setCardForm(p => ({ ...p, expiry: formatExpiry(e.target.value) }))}
                    maxLength={5} />
                </div>
                <div className="form-group">
                  <label className="form-label">CVV</label>
                  <input className="form-input" placeholder="•••" type="password"
                    value={cardForm.cvv}
                    onChange={e => setCardForm(p => ({ ...p, cvv: e.target.value.replace(/\D/g, '').slice(0, 4) }))}
                    maxLength={4} />
                </div>
              </div>
              <p style={{ marginTop: 16, fontSize: '0.78rem', color: 'var(--text-muted)', display: 'flex', alignItems: 'center', gap: 6 }}>
                <Lock size={12} /> Kart bilgileriniz 256-bit SSL ile şifrelenerek iletilmektedir.
              </p>
            </div>
          )}

          {/* ── Step 3: Review ── */}
          {step === 'review' && (
            <div>
              <h2 style={{ fontSize: '1.2rem', fontWeight: 700, marginBottom: 24, display: 'flex', alignItems: 'center', gap: 10 }}>
                <CheckCircle2 size={20} color="#a78bfa" /> Sipariş Özeti
              </h2>

              <div style={{ display: 'flex', flexDirection: 'column', gap: 12, marginBottom: 24 }}>
                {items.map(item => (
                  <div key={item.id} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '12px 0', borderBottom: '1px solid var(--glass-border)' }}>
                    <div>
                      <div style={{ fontWeight: 500, fontSize: '0.92rem' }}>{item.name}</div>
                      <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)' }}>{item.quantity} adet × ₺{item.price.toLocaleString('tr-TR')}</div>
                    </div>
                    <div style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, color: '#a78bfa' }}>
                      ₺{(item.price * item.quantity).toLocaleString('tr-TR')}
                    </div>
                  </div>
                ))}
              </div>

              <div style={{ background: 'rgba(0,0,0,0.2)', borderRadius: 12, padding: '16px 20px', marginBottom: 24 }}>
                <div style={{ fontSize: '0.78rem', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.08em', color: 'var(--text-muted)', marginBottom: 10 }}>Teslimat Adresi</div>
                <p style={{ fontSize: '0.9rem', lineHeight: 1.7 }}>
                  {addrForm.fullName}<br />
                  {addrForm.address}, {addrForm.district}/{addrForm.city} {addrForm.zip}<br />
                  {addrForm.phone}
                </p>
              </div>

              <div style={{ background: 'rgba(0,0,0,0.2)', borderRadius: 12, padding: '16px 20px' }}>
                <div style={{ fontSize: '0.78rem', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.08em', color: 'var(--text-muted)', marginBottom: 10 }}>Ödeme Yöntemi</div>
                <p style={{ fontSize: '0.9rem', display: 'flex', alignItems: 'center', gap: 8 }}>
                  <CreditCard size={15} color="#a78bfa" />
                  {brand ? `${brand} — ` : ''}
                  •••• •••• •••• {cardForm.cardNumber.replace(/\s/g, '').slice(-4)}
                </p>
              </div>
            </div>
          )}

          {/* Navigation buttons */}
          <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: 32, gap: 12 }}>
            <button
              className="btn btn-ghost"
              onClick={() => { const i = steps.findIndex(s => s.key === step); if (i > 0) setStep(steps[i - 1].key); else navigate('/user/cart'); }}
            >
              ← Geri
            </button>
            {step !== 'review' ? (
              <button className="btn btn-primary" style={{ gap: 6 }} onClick={handleNext}>
                Devam Et <ChevronRight size={16} />
              </button>
            ) : (
              <button
                className="btn btn-primary"
                style={{ gap: 6, padding: '12px 28px' }}
                disabled={loading}
                onClick={handleSubmit}
              >
                {loading ? <><span style={{ opacity: 0.7 }}>İşleniyor...</span></> : <><Lock size={15} /> Siparişi Onayla</>}
              </button>
            )}
          </div>
        </div>

        {/* Right: mini order summary */}
        <div className="user-order-summary">
          <h3 style={{ fontSize: '1rem', fontWeight: 700, marginBottom: 16 }}>Sipariş Tutarı</h3>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 10, marginBottom: 16 }}>
            <div className="user-summary-row"><span>Ara Toplam</span><span>₺{totalPrice.toLocaleString('tr-TR')}</span></div>
            <div className="user-summary-row"><span>KDV (%18)</span><span>₺{tax.toFixed(2)}</span></div>
            <div className="user-summary-row"><span>Kargo</span><span style={{ color: shipping === 0 ? '#6ee7b7' : undefined }}>{shipping === 0 ? 'Ücretsiz' : `₺${shipping}`}</span></div>
          </div>
          <div className="glow-divider" style={{ marginBottom: 16 }} />
          <div className="user-summary-row">
            <span style={{ fontWeight: 700 }}>Toplam</span>
            <span style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1.2rem', background: 'linear-gradient(135deg, #a78bfa, #60a5fa)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent', backgroundClip: 'text' }}>₺{grandTotal.toFixed(2)}</span>
          </div>

          <div style={{ marginTop: 20, fontSize: '0.78rem', color: 'var(--text-muted)' }}>
            {items.length} ürün çeşidi, {items.reduce((s, i) => s + i.quantity, 0)} adet
          </div>
        </div>
      </div>
    </div>
  );
}
