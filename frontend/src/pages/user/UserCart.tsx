import { useNavigate } from 'react-router-dom';
import { ShoppingCart, Trash2, Plus, Minus, ArrowRight, ShoppingBag, ArrowLeft } from 'lucide-react';
import { useCart } from '../../context/CartContext';

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
  const { items, removeItem, updateQty, clearCart, totalPrice, totalItems } = useCart();

  if (items.length === 0) {
    return (
      <div className="user-page" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: 400 }}>
        <div style={{ textAlign: 'center' }}>
          <ShoppingCart size={60} style={{ margin: '0 auto 20px', opacity: 0.15 }} />
          <h2 style={{ fontSize: '1.4rem', fontWeight: 700, marginBottom: 8 }}>Sepetiniz Boş</h2>
          <p style={{ color: 'var(--text-muted)', marginBottom: 28, fontSize: '0.9rem' }}>
            Kataloğa göz atarak ürün ekleyebilirsiniz.
          </p>
          <button className="btn btn-primary" onClick={() => navigate('/user/catalog')}>
            <ShoppingBag size={16} />
            Alışverişe Başla
          </button>
        </div>
      </div>
    );
  }

  const shipping  = totalPrice >= 500 ? 0 : 49;
  const taxRate   = 0.18;
  const tax       = totalPrice * taxRate;
  const grandTotal = totalPrice + shipping + tax;

  return (
    <div className="user-page">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 32 }}>
        <div>
          <div className="user-section-label">Alışveriş</div>
          <h1 style={{ fontSize: '1.9rem', fontWeight: 700 }}>
            Sepetim
            <span style={{ marginLeft: 12, fontSize: '1rem', fontWeight: 400, color: 'var(--text-muted)' }}>({totalItems} ürün)</span>
          </h1>
        </div>
        <button className="btn btn-ghost" style={{ gap: 6 }} onClick={() => navigate('/user/catalog')}>
          <ArrowLeft size={14} /> Alışverişe Devam
        </button>
      </div>

      <div className="user-cart-grid">
        {/* Items */}
        <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
          {items.map((item, i) => {
            const grad = item.imageGradient ?? GRADIENTS[i % GRADIENTS.length];
            return (
              <div key={item.id} className="user-cart-item animate-fade-up" style={{ animationDelay: `${i * 0.05}s` }}>
                {/* Mini product image */}
                <div style={{
                  width: 72, height: 72, borderRadius: 12, flexShrink: 0,
                  background: grad, display: 'flex', alignItems: 'center', justifyContent: 'center',
                }}>
                  <ShoppingBag size={24} color="rgba(255,255,255,0.6)" />
                </div>

                <div style={{ flex: 1, minWidth: 0 }}>
                  <div style={{ fontWeight: 600, fontSize: '0.95rem', color: 'var(--text-primary)', marginBottom: 2 }}>{item.name}</div>
                  <code style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{item.sku}</code>
                  {item.categoryName && (
                    <span className="badge badge-purple" style={{ fontSize: '0.68rem', marginLeft: 8 }}>{item.categoryName}</span>
                  )}
                </div>

                {/* Qty controls */}
                <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                  <button className="btn btn-ghost" style={{ width: 34, height: 34, padding: 0 }}
                    onClick={() => updateQty(item.id, item.quantity - 1)}>
                    <Minus size={13} />
                  </button>
                  <span style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, minWidth: 28, textAlign: 'center', fontSize: '1rem' }}>{item.quantity}</span>
                  <button className="btn btn-ghost" style={{ width: 34, height: 34, padding: 0 }}
                    onClick={() => updateQty(item.id, item.quantity + 1)}>
                    <Plus size={13} />
                  </button>
                </div>

                {/* Price */}
                <div style={{ minWidth: 80, textAlign: 'right' }}>
                  <div style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1rem', color: '#a78bfa' }}>
                    ₺{(item.price * item.quantity).toLocaleString('tr-TR')}
                  </div>
                  <div style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>
                    ₺{item.price.toLocaleString('tr-TR')} / adet
                  </div>
                </div>

                {/* Remove */}
                <button className="btn btn-ghost" style={{ width: 34, height: 34, padding: 0, color: '#f87171', borderColor: 'rgba(239,68,68,0.2)' }}
                  onClick={() => removeItem(item.id)}>
                  <Trash2 size={14} />
                </button>
              </div>
            );
          })}

          <button
            className="btn btn-ghost"
            style={{ alignSelf: 'flex-start', gap: 6, color: '#f87171', borderColor: 'rgba(239,68,68,0.2)', fontSize: '0.85rem' }}
            onClick={clearCart}
          >
            <Trash2 size={13} /> Sepeti Temizle
          </button>
        </div>

        {/* Order summary */}
        <div>
          <div className="user-order-summary">
            <h2 style={{ fontSize: '1.1rem', fontWeight: 700, marginBottom: 20 }}>Sipariş Özeti</h2>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 12, marginBottom: 20 }}>
              <div className="user-summary-row">
                <span>Ara Toplam</span>
                <span>₺{totalPrice.toLocaleString('tr-TR')}</span>
              </div>
              <div className="user-summary-row">
                <span>KDV (%18)</span>
                <span>₺{tax.toFixed(2)}</span>
              </div>
              <div className="user-summary-row">
                <span>Kargo</span>
                <span style={{ color: shipping === 0 ? '#6ee7b7' : 'var(--text-secondary)' }}>
                  {shipping === 0 ? 'Ücretsiz 🎉' : `₺${shipping}`}
                </span>
              </div>
              {shipping === 0 && (
                <div style={{ fontSize: '0.78rem', color: '#6ee7b7', padding: '8px 12px', background: 'rgba(16,185,129,0.08)', borderRadius: 8, border: '1px solid rgba(16,185,129,0.15)' }}>
                  500₺ üzeri alışverişinizde kargo bedava!
                </div>
              )}
              {shipping > 0 && (
                <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)', padding: '8px 12px', background: 'rgba(255,255,255,0.03)', borderRadius: 8, border: '1px solid var(--glass-border)' }}>
                  ₺{(500 - totalPrice).toFixed(0)} daha ekle, kargo bedava!
                </div>
              )}
            </div>

            <div className="glow-divider" style={{ marginBottom: 16 }} />

            <div className="user-summary-row" style={{ marginBottom: 24 }}>
              <span style={{ fontWeight: 700, fontSize: '1rem', color: 'var(--text-primary)' }}>Genel Toplam</span>
              <span style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1.3rem', background: 'linear-gradient(135deg, #a78bfa, #60a5fa)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent', backgroundClip: 'text' }}>
                ₺{grandTotal.toFixed(2)}
              </span>
            </div>

            <button
              className="btn btn-primary"
              style={{ width: '100%', padding: '15px', fontSize: '1rem', gap: 8 }}
              onClick={() => navigate('/user/checkout')}
            >
              Ödemeye Geç <ArrowRight size={18} />
            </button>

            <p style={{ textAlign: 'center', marginTop: 12, fontSize: '0.78rem', color: 'var(--text-muted)' }}>
              🔒 256-bit SSL ile güvenli ödeme
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
