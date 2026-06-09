import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { ArrowRight, ShoppingBag, Zap, Shield, Truck, Star, TrendingUp, Package, Gavel } from 'lucide-react';
import { useCart } from '../../context/CartContext';
import { commerceApi } from '../../services/commerceApi';
import { formatMoney, getApiErrorMessage, isActiveAuction } from '../../services/apiUtils';
import type { AuctionSummaryDto, ProductListingSummaryDto } from '../../types/commerce';

const GRADIENTS = [
  'linear-gradient(135deg, #7c3aed 0%, #4f46e5 100%)',
  'linear-gradient(135deg, #3b82f6 0%, #06b6d4 100%)',
  'linear-gradient(135deg, #ec4899 0%, #f59e0b 100%)',
  'linear-gradient(135deg, #10b981 0%, #3b82f6 100%)',
  'linear-gradient(135deg, #f59e0b 0%, #ef4444 100%)',
  'linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%)',
];

const features = [
  { icon: Truck, title: 'Kontrollü Sevkiyat', desc: 'Admin onayı sonrası ayrı kargo aksiyonu', color: '#6ee7b7' },
  { icon: Shield, title: 'Simüle Ödeme', desc: 'Rezervasyon ve idempotency backend tarafından yönetilir', color: '#93c5fd' },
  { icon: Star, title: 'İkinci El Odaklı', desc: 'Tekil listing ve açık artırma akışları', color: '#fcd34d' },
  { icon: Zap, title: '15 Dakika Rezervasyon', desc: 'Ödeme sürecinde stok geçici ayrılır', color: '#a78bfa' },
];

export default function UserHome() {
  const [featured, setFeatured] = useState<ProductListingSummaryDto[]>([]);
  const [auctions, setAuctions] = useState<AuctionSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const { addItem } = useCart();
  const navigate = useNavigate();

  useEffect(() => {
    let mounted = true;
    Promise.all([
      commerceApi.getListings({ pageSize: 6 }),
      commerceApi.getAuctions({ pageSize: 4 }),
    ])
      .then(([listings, auctionList]) => {
        if (!mounted) return;
        setFeatured(listings);
        setAuctions(auctionList.filter(a => isActiveAuction(a.status)).slice(0, 4));
      })
      .catch(err => mounted && setError(getApiErrorMessage(err, 'Ana sayfa verileri yüklenemedi.')))
      .finally(() => mounted && setLoading(false));
    return () => { mounted = false; };
  }, []);

  const handleAddToCart = (p: ProductListingSummaryDto, e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    addItem({
      id: p.id,
      name: p.productName,
      sku: p.sku,
      price: p.price,
      categoryName: p.sourceFacilityName,
      imageGradient: GRADIENTS[parseInt(p.id.replace(/-/g, '').slice(0, 8), 16) % GRADIENTS.length] ?? GRADIENTS[0],
    });
  };

  return (
    <div className="user-page">
      <section className="user-hero animate-fade-up">
        <div className="user-hero-badge"><TrendingUp size={12} /> Yeniden Satış ve Açık Artırma Platformu</div>
        <h1 className="user-hero-title">Tekil Ürünleri Keşfet<br /><span className="user-hero-gradient">Güvenli Akışla Satın Al</span></h1>
        <p className="user-hero-sub">Satışta olan ikinci el ürünleri, açık artırmaları ve platforma ürün satma taleplerinizi tek müşteri arayüzünden yönetin.</p>
        <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap', justifyContent: 'center' }}>
          <button className="btn btn-primary" style={{ padding: '14px 32px', fontSize: '1rem' }} onClick={() => navigate('/user/catalog')}>Kataloğa Git <ArrowRight size={18} /></button>
          <button className="btn btn-ghost" style={{ padding: '14px 28px', fontSize: '1rem' }} onClick={() => navigate('/user/auctions')}>Açık Artırmalar</button>
          <button className="btn btn-ghost" style={{ padding: '14px 28px', fontSize: '1rem' }} onClick={() => navigate('/user/sell')}>Ürünümü Sat</button>
        </div>
        <div className="user-hero-stats">
          <div className="user-hero-stat"><div className="user-hero-stat-value">{featured.length}</div><div className="user-hero-stat-label">Aktif Listing</div></div>
          <div className="user-hero-stat"><div className="user-hero-stat-value">{auctions.length}</div><div className="user-hero-stat-label">Aktif Açık Artırma</div></div>
          <div className="user-hero-stat"><div className="user-hero-stat-value">15 dk</div><div className="user-hero-stat-label">Ödeme Rezervasyonu</div></div>
          <div className="user-hero-stat"><div className="user-hero-stat-value">Admin</div><div className="user-hero-stat-label">Onaylı Sevkiyat</div></div>
        </div>
      </section>

      <section style={{ padding: '0 0 64px' }}>
        <div className="user-section-label">Platform Akışı</div>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: 20 }}>
          {features.map(({ icon: Icon, title, desc, color }, i) => (
            <div key={title} className="glass-card animate-fade-up" style={{ animationDelay: `${i * 0.07}s`, padding: '28px 24px' }}>
              <div style={{ width: 48, height: 48, borderRadius: 14, background: `${color}20`, border: `1px solid ${color}33`, display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: 16 }}><Icon size={22} color={color} /></div>
              <h3 style={{ fontSize: '1rem', fontWeight: 700, marginBottom: 6 }}>{title}</h3>
              <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', lineHeight: 1.6 }}>{desc}</p>
            </div>
          ))}
        </div>
      </section>

      <section>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
          <div><div className="user-section-label">Öne Çıkan Listingler</div><h2 style={{ fontSize: '1.5rem', fontWeight: 700 }}>Satışta Olan Ürünler</h2></div>
          <Link to="/user/catalog" className="btn btn-ghost" style={{ gap: 6, fontSize: '0.88rem' }}>Tümünü Gör <ArrowRight size={14} /></Link>
        </div>
        {loading ? <Empty icon={Package} text="Ürünler yükleniyor..." /> : error ? <Empty icon={Package} text={error} /> : featured.length === 0 ? <Empty icon={Package} text="Satışta ürün bulunamadı." /> : (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(260px, 1fr))', gap: 20 }}>
            {featured.map((p, i) => (
              <Link key={p.id} to={`/user/catalog/${p.id}`} className="user-product-card animate-fade-up" style={{ textDecoration: 'none', animationDelay: `${i * 0.06}s` } as React.CSSProperties}>
                <div className="user-product-img" style={{ background: GRADIENTS[i % GRADIENTS.length] }}><ShoppingBag size={36} color="rgba(255,255,255,0.6)" /></div>
                <div className="user-product-body">
                  <span className="badge badge-purple" style={{ fontSize: '0.7rem', marginBottom: 6 }}>{p.sourceFacilityName}</span>
                  <h3 className="user-product-name">{p.productName}</h3>
                  <code style={{ fontSize: '0.75rem', color: 'var(--text-muted)', display: 'block', marginBottom: 12 }}>{p.sku}</code>
                  <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <span className="user-product-price">{formatMoney(p.price, p.currency)}</span>
                    <button className="btn btn-primary" style={{ padding: '8px 16px', fontSize: '0.82rem' }} onClick={e => handleAddToCart(p, e)}>Sepete Ekle</button>
                  </div>
                </div>
              </Link>
            ))}
          </div>
        )}
      </section>

      <section style={{ marginTop: 64 }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
          <div><div className="user-section-label">Açık Artırmalar</div><h2 style={{ fontSize: '1.5rem', fontWeight: 700 }}>Aktif Teklif Akışları</h2></div>
          <Link to="/user/auctions" className="btn btn-ghost" style={{ gap: 6, fontSize: '0.88rem' }}>Tümünü Gör <ArrowRight size={14} /></Link>
        </div>
        {auctions.length === 0 ? <Empty icon={Gavel} text="Aktif açık artırma bulunamadı." /> : (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(260px, 1fr))', gap: 20 }}>
            {auctions.map(a => (
              <Link key={a.id} to={`/user/auctions/${a.id}`} className="glass-card" style={{ padding: 20, textDecoration: 'none' }}>
                <div className="badge badge-amber" style={{ marginBottom: 10 }}>Aktif Açık Artırma</div>
                <h3 style={{ fontSize: '1rem', fontWeight: 700, marginBottom: 8 }}>{a.productName}</h3>
                <p style={{ color: 'var(--text-muted)', fontSize: '0.82rem', marginBottom: 12 }}>SKU: {a.sku}</p>
                <div style={{ color: '#a78bfa', fontWeight: 800 }}>{formatMoney(a.currentPrice, a.currency)}</div>
                <div style={{ color: 'var(--text-muted)', fontSize: '0.78rem', marginTop: 6 }}>Minimum sonraki teklif: {formatMoney(a.minimumNextBid, a.currency)}</div>
              </Link>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}

function Empty({ icon: Icon, text }: { icon: typeof Package; text: string }) {
  return <div style={{ textAlign: 'center', padding: '60px 0', color: 'var(--text-muted)' }}><Icon size={40} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>;
}
