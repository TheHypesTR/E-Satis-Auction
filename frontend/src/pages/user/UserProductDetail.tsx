import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ShoppingBag, ArrowLeft, ShoppingCart, Check, Package, Truck, Shield, Star, Handshake, X } from 'lucide-react';
import api from '../../api/axios';
import { useCart } from '../../context/CartContext';

interface Product {
  id: string;
  name: string;
  sku: string;
  categoryName?: string;
  barcode?: string;
  price?: number;
  description?: string;
}

const GRADIENTS = [
  'linear-gradient(135deg, #7c3aed 0%, #4f46e5 100%)',
  'linear-gradient(135deg, #3b82f6 0%, #06b6d4 100%)',
  'linear-gradient(135deg, #ec4899 0%, #f59e0b 100%)',
  'linear-gradient(135deg, #10b981 0%, #3b82f6 100%)',
  'linear-gradient(135deg, #f59e0b 0%, #ef4444 100%)',
  'linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%)',
];



const highlights = [
  { icon: Truck,  text: 'Ücretsiz kargo — 500₺ üzeri siparişlerde' },
  { icon: Shield, text: '7/24 güvenceli ödeme altyapısı' },
  { icon: Star,   text: 'Onaylı tedarikçi, orijinal ürün garantisi' },
];

export default function UserProductDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [qty, setQty] = useState(1);
  const [added, setAdded] = useState(false);
  const [showOfferModal, setShowOfferModal] = useState(false);
  const [offerAmount, setOfferAmount] = useState('');
  const [offerSent, setOfferSent] = useState(false);
  const [showReviewModal, setShowReviewModal] = useState(false);
  const [reviewText, setReviewText] = useState('');
  const [rating, setRating] = useState(0);
  const [reviewSent, setReviewSent] = useState(false);
  const { addItem } = useCart();

  useEffect(() => {
    if (!id) return;
    void api.get(`/Product/${id}`)
      .then(res => { setProduct(res.data); setLoading(false); })
      .catch(() => { setProduct(null); setLoading(false); });
  }, [id]);

  const handleAddToCart = () => {
    if (!product) return;
    const price = product.price ?? 0;
    const idx   = parseInt((product.id || '0').replace(/-/g, '').slice(0, 8), 16);
    addItem({
      id: product.id, name: product.name, sku: product.sku, price,
      categoryName: product.categoryName,
      imageGradient: GRADIENTS[idx % GRADIENTS.length] ?? GRADIENTS[0],
    }, qty);
    setAdded(true);
    setTimeout(() => setAdded(false), 2000);
  };

  const handleMakeOffer = () => {
    if (!offerAmount) return;
    setOfferSent(true);
    setTimeout(() => {
      setOfferSent(false);
      setShowOfferModal(false);
      setOfferAmount('');
    }, 1500);
  };

  const handleMakeReview = () => {
    if (!reviewText || rating === 0) return;
    setReviewSent(true);
    setTimeout(() => {
      setReviewSent(false);
      setShowReviewModal(false);
      setReviewText('');
      setRating(0);
    }, 1500);
  };

  if (loading) {
    return (
      <div className="user-page" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: 400 }}>
        <div style={{ textAlign: 'center', color: 'var(--text-muted)' }}>
          <Package size={40} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
          <p>Yükleniyor...</p>
        </div>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="user-page" style={{ textAlign: 'center', paddingTop: 80 }}>
        <Package size={48} style={{ margin: '0 auto 16px', opacity: 0.2 }} />
        <h2>Ürün bulunamadı</h2>
        <button className="btn btn-ghost" style={{ marginTop: 20 }} onClick={() => navigate('/user/catalog')}>
          <ArrowLeft size={15} /> Kataloğa Dön
        </button>
      </div>
    );
  }

  const price   = product.price ?? 0;
  const idx     = parseInt((product.id || '0').replace(/-/g, '').slice(0, 8), 16);
  const gradient = GRADIENTS[idx % GRADIENTS.length] ?? GRADIENTS[0];

  return (
    <div className="user-page">
      {/* Back */}
      <button className="btn btn-ghost" style={{ marginBottom: 28, gap: 6 }} onClick={() => navigate(-1)}>
        <ArrowLeft size={15} /> Geri
      </button>

      <div className="user-detail-grid">
        {/* Image */}
        <div>
          <div className="user-detail-img" style={{ background: gradient }}>
            <ShoppingBag size={80} color="rgba(255,255,255,0.5)" />
          </div>
          {/* Highlights */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 12, marginTop: 20 }}>
            {highlights.map(({ icon: Icon, text }, i) => (
              <div key={i} style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '12px 16px', background: 'rgba(255,255,255,0.03)', borderRadius: 10, border: '1px solid var(--glass-border)' }}>
                <Icon size={16} color="#a78bfa" />
                <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>{text}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Info */}
        <div>
          {product.categoryName && (
            <span className="badge badge-purple" style={{ marginBottom: 12 }}>{product.categoryName}</span>
          )}
          <h1 style={{ fontSize: '1.8rem', fontWeight: 700, marginBottom: 8, lineHeight: 1.3 }}>{product.name}</h1>
          <div style={{ display: 'flex', gap: 16, marginBottom: 20 }}>
            <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>SKU: <code style={{ color: '#a78bfa', fontSize: '0.82rem' }}>{product.sku}</code></span>
            {product.barcode && <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Barkod: <code style={{ color: 'var(--text-secondary)', fontSize: '0.82rem' }}>{product.barcode}</code></span>}
          </div>

          <div className="glow-divider" style={{ marginBottom: 24 }} />

          {/* Price */}
          <div style={{ marginBottom: 28 }}>
            <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: 4, textTransform: 'uppercase', letterSpacing: '0.06em' }}>Birim Fiyat</div>
            <div style={{ fontFamily: "'Space Grotesk', sans-serif", fontSize: '2.4rem', fontWeight: 700, background: 'linear-gradient(135deg, #a78bfa, #60a5fa)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent', backgroundClip: 'text' }}>
              ₺{price.toLocaleString('tr-TR')}
            </div>
          </div>

          {/* Qty */}
          <div style={{ marginBottom: 24 }}>
            <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: 10, textTransform: 'uppercase', letterSpacing: '0.06em' }}>Miktar</div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
              <button className="btn btn-ghost" style={{ width: 40, height: 40, padding: 0, fontSize: '1.2rem' }}
                onClick={() => setQty(q => Math.max(1, q - 1))}>−</button>
              <span style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1.2rem', minWidth: 32, textAlign: 'center' }}>{qty}</span>
              <button className="btn btn-ghost" style={{ width: 40, height: 40, padding: 0, fontSize: '1.2rem' }}
                onClick={() => setQty(q => q + 1)}>+</button>
              <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>
                Toplam: <strong style={{ color: 'var(--text-primary)' }}>₺{(price * qty).toLocaleString('tr-TR')}</strong>
              </span>
            </div>
          </div>

          {/* CTA */}
          <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap' }}>
            <button
              className="btn btn-primary"
              style={{ flex: 1, padding: '15px', fontSize: '1rem', gap: 8, minWidth: '150px' }}
              onClick={handleAddToCart}
            >
              {added ? <><Check size={18} /> Sepete Eklendi!</> : <><ShoppingCart size={18} /> Sepete Ekle</>}
            </button>
            <button
              className="btn btn-ghost"
              style={{ flex: 1, padding: '15px', minWidth: '120px' }}
              onClick={() => setShowOfferModal(true)}
            >
              <Handshake size={18} /> Teklif Yap
            </button>
            <button
              className="btn btn-ghost"
              style={{ flex: 1, padding: '15px', minWidth: '150px' }}
              onClick={() => setShowReviewModal(true)}
            >
              <Star size={18} /> Değerlendirme Yaz
            </button>
            <button
              className="btn btn-ghost"
              style={{ padding: '15px 20px', background: 'rgba(255,255,255,0.05)', color: 'var(--text-primary)', flex: '1 1 100%' }}
              onClick={() => { handleAddToCart(); navigate('/user/checkout'); }}
            >
              Hemen Al
            </button>
          </div>

          {added && (
            <div className="animate-fade-up" style={{ marginTop: 16, padding: '12px 16px', background: 'rgba(16,185,129,0.1)', border: '1px solid rgba(16,185,129,0.2)', borderRadius: 10, color: '#6ee7b7', fontSize: '0.88rem', display: 'flex', alignItems: 'center', gap: 8 }}>
              <Check size={16} />
              Ürün sepete eklendi! <button style={{ background: 'none', border: 'none', cursor: 'pointer', color: '#a78bfa', fontSize: '0.88rem', textDecoration: 'underline' }} onClick={() => navigate('/user/cart')}>Sepete git →</button>
            </div>
          )}
        </div>
      </div>

      {/* Offer Modal */}
      {showOfferModal && (
        <div className="modal-overlay" onClick={() => setShowOfferModal(false)}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 400, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8 }}>
                <Handshake size={20} color="#a78bfa" /> Teklif Yap
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowOfferModal(false)}>
                <X size={18} />
              </button>
            </div>
            
            <div style={{ marginBottom: 24, display: 'flex', flexDirection: 'column', gap: 16 }}>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem' }}>
                <strong style={{ color: 'var(--text-primary)' }}>{product.name}</strong> ürünü için teklifinizi belirleyin. Satıcı değerlendirip size dönüş yapacaktır.
              </p>
              <div className="form-group">
                <label className="form-label">Teklifiniz (₺)</label>
                <input 
                  type="number" 
                  className="form-input" 
                  placeholder="Örn: 500" 
                  value={offerAmount}
                  onChange={e => setOfferAmount(e.target.value)}
                />
              </div>
            </div>

            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => setShowOfferModal(false)}>İptal</button>
              <button className="btn btn-primary" style={{ gap: 8 }} onClick={handleMakeOffer}>
                {offerSent ? <><Check size={16}/> Gönderildi</> : 'Teklifi Gönder'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Review Modal */}
      {showReviewModal && (
        <div className="modal-overlay" onClick={() => setShowReviewModal(false)}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 450, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8 }}>
                <Star size={20} color="#fcd34d" /> Ürünü Değerlendir
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowReviewModal(false)}>
                <X size={18} />
              </button>
            </div>
            
            <div style={{ marginBottom: 24, display: 'flex', flexDirection: 'column', gap: 16 }}>
              <div style={{ display: 'flex', justifyContent: 'center', gap: 8, margin: '10px 0' }}>
                {[1, 2, 3, 4, 5].map(s => (
                  <button key={s} style={{ background: 'none', border: 'none', cursor: 'pointer', padding: 0 }} onClick={() => setRating(s)}>
                    <Star size={32} fill={rating >= s ? '#fcd34d' : 'transparent'} color={rating >= s ? '#fcd34d' : 'var(--text-muted)'} />
                  </button>
                ))}
              </div>
              <div className="form-group">
                <label className="form-label">Yorumunuz</label>
                <textarea 
                  className="form-input" 
                  placeholder="Ürün hakkındaki düşüncelerinizi paylaşın..." 
                  rows={4}
                  style={{ resize: 'vertical' }}
                  value={reviewText}
                  onChange={e => setReviewText(e.target.value)}
                />
              </div>
            </div>

            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => setShowReviewModal(false)}>İptal</button>
              <button className="btn btn-primary" style={{ gap: 8 }} onClick={handleMakeReview}>
                {reviewSent ? <><Check size={16}/> Gönderildi</> : 'Değerlendirmeyi Gönder'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
