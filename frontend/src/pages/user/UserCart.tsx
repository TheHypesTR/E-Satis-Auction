/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ShoppingCart, Trash2, Plus, Minus, ArrowRight, ShoppingBag, ArrowLeft, AlertCircle, Ticket } from 'lucide-react';
import { useCart } from '../../context/CartContext';
import { commerceApi } from '../../services/commerceApi';
import { formatMoney, getApiErrorMessage, requireAuth } from '../../services/apiUtils';
import type { CartPricePreviewDto } from '../../types/commerce';

const GRADIENTS = [
  'linear-gradient(135deg, #7c3aed 0%, #4f46e5 100%)',
  'linear-gradient(135deg, #3b82f6 0%, #06b6d4 100%)',
  'linear-gradient(135deg, #ec4899 0%, #f59e0b 100%)',
  'linear-gradient(135deg, #10b981 0%, #3b82f6 100%)',
  'linear-gradient(135deg, #f59e0b 0%, #ef4444 100%)',
  'linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%)',
];

export default function UserCart() {
  const navigate = useNavigate();
  const { items, removeItem, updateQty, clearCart, totalItems } = useCart();
  const [preview, setPreview] = useState<CartPricePreviewDto | null>(null);
  const [coupon, setCoupon] = useState('');
  const [loadingPreview, setLoadingPreview] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (items.length === 0 || !localStorage.getItem('token')) { setPreview(null); return; }
    const item = items[0];
    setLoadingPreview(true);
    setError('');
    commerceApi.updateCart(item.id, item.quantity)
      .then(() => commerceApi.getCartPreview())
      .then(setPreview)
      .catch(err => setError(getApiErrorMessage(err, 'Sepet fiyatı backend üzerinden hesaplanamadı.')))
      .finally(() => setLoadingPreview(false));
  }, [items]);

  const applyCoupon = async () => {
    if (!coupon.trim() || !requireAuth(navigate)) return;
    setLoadingPreview(true);
    setError('');
    try {
      await commerceApi.applyCoupon(coupon.trim());
      setPreview(await commerceApi.getCartPreview());
    } catch (err) {
      setError(getApiErrorMessage(err, 'Kupon uygulanamadı.'));
    } finally {
      setLoadingPreview(false);
    }
  };

  const removeBackendCoupon = async () => {
    if (!requireAuth(navigate)) return;
    setLoadingPreview(true);
    try {
      await commerceApi.removeCoupon();
      setCoupon('');
      setPreview(await commerceApi.getCartPreview());
    } catch (err) {
      setError(getApiErrorMessage(err, 'Kupon kaldırılamadı.'));
    } finally {
      setLoadingPreview(false);
    }
  };

  const clearAll = async () => {
    clearCart();
    if (localStorage.getItem('token')) await commerceApi.clearCart().catch(() => undefined);
  };

  if (items.length === 0) return <EmptyCart navigate={navigate} />;

  const subtotal = preview?.subtotalAmount ?? items.reduce((sum, i) => sum + i.price * i.quantity, 0);
  const discount = preview?.discountAmount ?? 0;
  const shipping = preview?.shippingAmount ?? 0;
  const total = preview?.totalAmount ?? subtotal;
  const currency = preview?.currency ?? 'TRY';

  return (
    <div className="user-page">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 32 }}>
        <div><div className="user-section-label">Alışveriş</div><h1 style={{ fontSize: '1.9rem', fontWeight: 700 }}>Tek Listing Sepeti <span style={{ marginLeft: 12, fontSize: '1rem', fontWeight: 400, color: 'var(--text-muted)' }}>({totalItems} adet)</span></h1></div>
        <button className="btn btn-ghost" style={{ gap: 6 }} onClick={() => navigate('/user/catalog')}><ArrowLeft size={14} /> Alışverişe Devam</button>
      </div>
      <div className="user-cart-grid">
        <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
          {items.map((item, i) => (
            <div key={item.id} className="user-cart-item animate-fade-up" style={{ animationDelay: `${i * 0.05}s` }}>
              <div style={{ width: 72, height: 72, borderRadius: 12, flexShrink: 0, background: item.imageGradient ?? GRADIENTS[i % GRADIENTS.length], display: 'flex', alignItems: 'center', justifyContent: 'center' }}><ShoppingBag size={24} color="rgba(255,255,255,0.6)" /></div>
              <div style={{ flex: 1, minWidth: 0 }}><div style={{ fontWeight: 600, fontSize: '0.95rem', color: 'var(--text-primary)', marginBottom: 2 }}>{item.name}</div><code style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{item.sku}</code>{item.categoryName && <span className="badge badge-purple" style={{ fontSize: '0.68rem', marginLeft: 8 }}>{item.categoryName}</span>}</div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}><button className="btn btn-ghost" style={{ width: 34, height: 34, padding: 0 }} onClick={() => updateQty(item.id, item.quantity - 1)}><Minus size={13} /></button><span style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, minWidth: 28, textAlign: 'center', fontSize: '1rem' }}>{item.quantity}</span><button className="btn btn-ghost" style={{ width: 34, height: 34, padding: 0 }} onClick={() => updateQty(item.id, item.quantity + 1)}><Plus size={13} /></button></div>
              <div style={{ minWidth: 90, textAlign: 'right' }}><div style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1rem', color: '#a78bfa' }}>{formatMoney(item.price * item.quantity)}</div><div style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{formatMoney(item.price)} / adet</div></div>
              <button className="btn btn-ghost" style={{ width: 34, height: 34, padding: 0, color: '#f87171', borderColor: 'rgba(239,68,68,0.2)' }} onClick={() => removeItem(item.id)}><Trash2 size={14} /></button>
            </div>
          ))}
          <button className="btn btn-ghost" style={{ alignSelf: 'flex-start', gap: 6, color: '#f87171', borderColor: 'rgba(239,68,68,0.2)', fontSize: '0.85rem' }} onClick={clearAll}><Trash2 size={13} /> Sepeti Temizle</button>
        </div>
        <div>
          <div className="user-order-summary">
            <h2 style={{ fontSize: '1.1rem', fontWeight: 700, marginBottom: 20 }}>Backend Fiyat Önizleme</h2>
            {error && <div className="error-banner" style={{ marginBottom: 16 }}><AlertCircle size={15} /> {error}</div>}
            {!localStorage.getItem('token') && <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)', padding: '8px 12px', background: 'rgba(255,255,255,0.03)', borderRadius: 8, border: '1px solid var(--glass-border)', marginBottom: 14 }}>Kupon, kampanya ve ücretsiz kargo önizlemesi için giriş gerekir.</div>}
            <div style={{ display: 'flex', gap: 8, marginBottom: 14 }}><input className="form-input" placeholder="Kupon kodu" value={coupon} onChange={e => setCoupon(e.target.value)} /><button className="btn btn-ghost" onClick={applyCoupon} disabled={loadingPreview}><Ticket size={14} /> Uygula</button></div>
            {preview?.appliedCouponCampaignId && <button className="btn btn-ghost" style={{ marginBottom: 14, fontSize: '0.8rem' }} onClick={removeBackendCoupon}>Kuponu kaldır</button>}
            <div style={{ display: 'flex', flexDirection: 'column', gap: 12, marginBottom: 20 }}>
              <div className="user-summary-row"><span>Ara Toplam</span><span>{formatMoney(subtotal, currency)}</span></div>
              <div className="user-summary-row"><span>İndirim</span><span style={{ color: discount > 0 ? '#6ee7b7' : undefined }}>-{formatMoney(discount, currency)}</span></div>
              <div className="user-summary-row"><span>Kargo</span><span style={{ color: shipping === 0 ? '#6ee7b7' : undefined }}>{shipping === 0 ? 'Ücretsiz' : formatMoney(shipping, currency)}</span></div>
              {preview?.appliedLineCampaignId && <Info text="Ürün/listing bazlı kampanya uygulandı." />}
              {preview?.appliedCouponCampaignId && <Info text="Kupon sepet indirimi uygulandı." />}
              {preview?.appliedFreeShippingCampaignId && <Info text="Ücretsiz kargo kampanyası koşul sağlandığı için otomatik uygulandı." />}
            </div>
            <div className="glow-divider" style={{ marginBottom: 16 }} />
            <div className="user-summary-row" style={{ marginBottom: 24 }}><span style={{ fontWeight: 700, fontSize: '1rem', color: 'var(--text-primary)' }}>Genel Toplam</span><span style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1.3rem', background: 'linear-gradient(135deg, #a78bfa, #60a5fa)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent', backgroundClip: 'text' }}>{formatMoney(total, currency)}</span></div>
            <button className="btn btn-primary" style={{ width: '100%', padding: 15, fontSize: '1rem', gap: 8 }} onClick={() => requireAuth(navigate) && navigate('/user/checkout')}><ArrowRight size={18} /> Ödeme Simülasyonuna Geç</button>
            <p style={{ textAlign: 'center', marginTop: 12, fontSize: '0.78rem', color: 'var(--text-muted)' }}>Fiyat burada önizlemedir; frozen order snapshot ödeme/sipariş akışında oluşur.</p>
          </div>
        </div>
      </div>
    </div>
  );
}

function EmptyCart({ navigate }: { navigate: (path: string) => void }) {
  return <div className="user-page" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: 400 }}><div style={{ textAlign: 'center' }}><ShoppingCart size={60} style={{ margin: '0 auto 20px', opacity: 0.15 }} /><h2 style={{ fontSize: '1.4rem', fontWeight: 700, marginBottom: 8 }}>Sepetiniz Boş</h2><p style={{ color: 'var(--text-muted)', marginBottom: 28, fontSize: '0.9rem' }}>Kataloğa göz atarak tek listing checkout sepeti oluşturabilirsiniz.</p><button className="btn btn-primary" onClick={() => navigate('/user/catalog')}><ShoppingBag size={16} /> Alışverişe Başla</button></div></div>;
}

function Info({ text }: { text: string }) {
  return <div style={{ fontSize: '0.78rem', color: '#6ee7b7', padding: '8px 12px', background: 'rgba(16,185,129,0.08)', borderRadius: 8, border: '1px solid rgba(16,185,129,0.15)' }}>{text}</div>;
}

