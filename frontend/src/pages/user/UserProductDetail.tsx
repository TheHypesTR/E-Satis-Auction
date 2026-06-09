/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { ShoppingBag, ArrowLeft, ShoppingCart, Check, Package, Truck, Shield, Star, Gavel, AlertCircle } from 'lucide-react';
import { useCart } from '../../context/CartContext';
import { commerceApi } from '../../services/commerceApi';
import { formatMoney, getApiErrorMessage, isActiveListing, listingStatusLabel, makeIdempotencyKey, requireAuth } from '../../services/apiUtils';
import type { AuctionSummaryDto, ProductListingDetailDto } from '../../types/commerce';

const GRADIENTS = [
  'linear-gradient(135deg, #7c3aed 0%, #4f46e5 100%)',
  'linear-gradient(135deg, #3b82f6 0%, #06b6d4 100%)',
  'linear-gradient(135deg, #ec4899 0%, #f59e0b 100%)',
  'linear-gradient(135deg, #10b981 0%, #3b82f6 100%)',
  'linear-gradient(135deg, #f59e0b 0%, #ef4444 100%)',
  'linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%)',
];

const highlights = [
  { icon: Truck, text: 'Admin onayı sonrası ayrı kargo aksiyonu' },
  { icon: Shield, text: 'Ödeme sağlayıcısı simüle edilir; stok backend tarafında rezerve edilir' },
  { icon: Star, text: 'Tekil ikinci el listing ve stok kontrolü' },
];

export default function UserProductDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [listing, setListing] = useState<ProductListingDetailDto | null>(null);
  const [auction, setAuction] = useState<AuctionSummaryDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [qty, setQty] = useState(1);
  const { addItem } = useCart();

  useEffect(() => {
    if (!id) return;
    let mounted = true;
    setLoading(true);
    Promise.all([
      commerceApi.getListing(id),
      commerceApi.getAuctions({ pageSize: 50 }),
    ])
      .then(([detail, auctions]) => {
        if (!mounted) return;
        setListing(detail);
        setAuction(auctions.find(a => a.productListingId === id && ['Active', 'Scheduled', '2', '1'].includes(String(a.status))) ?? null);
      })
      .catch(err => mounted && setError(getApiErrorMessage(err, 'Ürün detayı yüklenemedi.')))
      .finally(() => mounted && setLoading(false));
    return () => { mounted = false; };
  }, [id]);

  const handleAddToCart = () => {
    if (!listing || !isPurchasable) return;
    const idx = parseInt(listing.id.replace(/-/g, '').slice(0, 8), 16);
    addItem({ id: listing.id, name: listing.productName, sku: listing.sku, price: listing.price, categoryName: listing.sourceFacilityName, imageGradient: GRADIENTS[idx % GRADIENTS.length] ?? GRADIENTS[0] }, qty);
    setMessage('Ürün sepete eklendi. Sepet tek listing odaklı olduğu için önceki sepet içeriği değiştirildi.');
  };

  const handleBuyNow = async () => {
    if (!listing || !isPurchasable || !requireAuth(navigate)) return;
    setActionLoading(true);
    setError('');
    setMessage('');
    try {
      const order = await commerceApi.buyNow(listing.id, qty, makeIdempotencyKey('buy_now'));
      navigate('/user/order-success', { state: { order, mode: 'buyNow' } });
    } catch (err) {
      setError(getApiErrorMessage(err, 'Direkt satın alma işlemi başlatılamadı.'));
    } finally {
      setActionLoading(false);
    }
  };

  if (loading) return <Centered icon={Package} text="Yükleniyor..." />;
  if (error && !listing) return <Centered icon={AlertCircle} text={error} />;
  if (!listing) return <Centered icon={Package} text="Ürün bulunamadı." />;

  const isPurchasable = isActiveListing(listing.status) && listing.availableStockQuantity > 0 && !auction;
  const idx = parseInt(listing.id.replace(/-/g, '').slice(0, 8), 16);
  const gradient = GRADIENTS[idx % GRADIENTS.length] ?? GRADIENTS[0];
  const statusText = listingStatusLabel[String(listing.status)] ?? String(listing.status);

  return (
    <div className="user-page">
      <button className="btn btn-ghost" style={{ marginBottom: 28, gap: 6 }} onClick={() => navigate(-1)}><ArrowLeft size={15} /> Geri</button>
      <div className="user-detail-grid">
        <div>
          <div className="user-detail-img" style={{ background: gradient }}><ShoppingBag size={80} color="rgba(255,255,255,0.5)" /></div>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 12, marginTop: 20 }}>
            {highlights.map(({ icon: Icon, text }) => <div key={text} style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '12px 16px', background: 'rgba(255,255,255,0.03)', borderRadius: 10, border: '1px solid var(--glass-border)' }}><Icon size={16} color="#a78bfa" /><span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>{text}</span></div>)}
          </div>
        </div>
        <div>
          <span className="badge badge-purple" style={{ marginBottom: 12 }}>{listing.sourceFacilityName}</span>
          <h1 style={{ fontSize: '1.8rem', fontWeight: 700, marginBottom: 8, lineHeight: 1.3 }}>{listing.productName}</h1>
          <div style={{ display: 'flex', gap: 16, marginBottom: 20, flexWrap: 'wrap' }}>
            <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>SKU: <code style={{ color: '#a78bfa' }}>{listing.sku}</code></span>
            <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Durum: <strong style={{ color: 'var(--text-secondary)' }}>{statusText}</strong></span>
            <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Satılabilir stok: <strong style={{ color: 'var(--text-secondary)' }}>{listing.availableStockQuantity}</strong></span>
          </div>
          <div className="glow-divider" style={{ marginBottom: 24 }} />
          <div style={{ marginBottom: 28 }}>
            <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: 4, textTransform: 'uppercase', letterSpacing: '0.06em' }}>Listing Fiyatı</div>
            <div style={{ fontFamily: "'Space Grotesk', sans-serif", fontSize: '2.4rem', fontWeight: 700, background: 'linear-gradient(135deg, #a78bfa, #60a5fa)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent', backgroundClip: 'text' }}>{formatMoney(listing.price, listing.currency)}</div>
          </div>

          {auction ? (
            <div style={{ marginBottom: 24, padding: 16, background: 'rgba(167, 139, 250, 0.1)', border: '1px solid rgba(167, 139, 250, 0.3)', borderRadius: 10 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 12 }}><div className="badge badge-amber">AÇIK ARTIRMADA</div><div style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>Güncel: <strong style={{ color: 'var(--text-primary)' }}>{formatMoney(auction.currentPrice, auction.currency)}</strong></div></div>
              <p style={{ fontSize: '0.9rem', color: 'var(--text-secondary)' }}>Bu listing açık artırmaya bağlı olduğu için doğrudan satın alma butonu kapalıdır.</p>
              <Link className="btn btn-primary" style={{ width: '100%', marginTop: 16, padding: 15, textDecoration: 'none', justifyContent: 'center' }} to={`/user/auctions/${auction.id}`}><Gavel size={16} /> Açık Artırmaya Git</Link>
            </div>
          ) : (
            <>
              <div style={{ marginBottom: 24 }}>
                <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: 10, textTransform: 'uppercase', letterSpacing: '0.06em' }}>Miktar</div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                  <button className="btn btn-ghost" style={{ width: 40, height: 40, padding: 0, fontSize: '1.2rem' }} onClick={() => setQty(q => Math.max(1, q - 1))} disabled={!isPurchasable}>-</button>
                  <span style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1.2rem', minWidth: 32, textAlign: 'center' }}>{qty}</span>
                  <button className="btn btn-ghost" style={{ width: 40, height: 40, padding: 0, fontSize: '1.2rem' }} onClick={() => setQty(q => Math.min(listing.availableStockQuantity, q + 1))} disabled={!isPurchasable}>+</button>
                  <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Toplam: <strong style={{ color: 'var(--text-primary)' }}>{formatMoney(listing.price * qty, listing.currency)}</strong></span>
                </div>
              </div>
              <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap' }}>
                <button className="btn btn-primary" style={{ flex: 1, padding: 15, fontSize: '1rem', gap: 8, minWidth: 150 }} onClick={handleAddToCart} disabled={!isPurchasable}><ShoppingCart size={18} /> Sepete Ekle</button>
                <button className="btn btn-ghost" style={{ flex: 1, padding: 15, minWidth: 150 }} onClick={handleBuyNow} disabled={!isPurchasable || actionLoading}>{actionLoading ? 'İşleniyor...' : 'Hemen Al'}</button>
              </div>
              {!isPurchasable && <div className="error-banner" style={{ marginTop: 16 }}><AlertCircle size={15} /> Bu ürün şu anda satın alınamaz.</div>}
              {message && <div className="animate-fade-up" style={{ marginTop: 16, padding: '12px 16px', background: 'rgba(16,185,129,0.1)', border: '1px solid rgba(16,185,129,0.2)', borderRadius: 10, color: '#6ee7b7', fontSize: '0.88rem', display: 'flex', alignItems: 'center', gap: 8 }}><Check size={16} /> {message} <button style={{ background: 'none', border: 'none', cursor: 'pointer', color: '#a78bfa', fontSize: '0.88rem', textDecoration: 'underline' }} onClick={() => navigate('/user/cart')}>Sepete git</button></div>}
              {error && <div className="error-banner" style={{ marginTop: 16 }}><AlertCircle size={15} /> {error}</div>}
            </>
          )}
        </div>
      </div>
    </div>
  );
}

function Centered({ icon: Icon, text }: { icon: typeof Package; text: string }) {
  return <div className="user-page" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: 400 }}><div style={{ textAlign: 'center', color: 'var(--text-muted)' }}><Icon size={44} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div></div>;
}

