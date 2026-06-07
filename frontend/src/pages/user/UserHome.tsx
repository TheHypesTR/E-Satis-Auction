import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { ArrowRight, ShoppingBag, Zap, Shield, Truck, Star, TrendingUp, Package } from 'lucide-react';
import api from '../../api/axios';
import { useCart } from '../../context/CartContext';

interface Product {
  id: string;
  name: string;
  sku: string;
  categoryName?: string;
  price?: number;
}

const GRADIENTS = [
  'linear-gradient(135deg, #7c3aed 0%, #4f46e5 100%)',
  'linear-gradient(135deg, #3b82f6 0%, #06b6d4 100%)',
  'linear-gradient(135deg, #ec4899 0%, #f59e0b 100%)',
  'linear-gradient(135deg, #10b981 0%, #3b82f6 100%)',
  'linear-gradient(135deg, #f59e0b 0%, #ef4444 100%)',
  'linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%)',
];

const features = [
  { icon: Truck,   title: 'Hızlı Teslimat',    desc: '24-48 saat içinde kapınızda',  color: '#6ee7b7' },
  { icon: Shield,  title: 'Güvenli Ödeme',      desc: '256-bit SSL şifreleme',        color: '#93c5fd' },
  { icon: Star,    title: 'Kalite Güvencesi',   desc: 'Onaylı tedarikçiler',          color: '#fcd34d' },
  { icon: Zap,     title: 'Anlık Stok Takibi',  desc: 'Gerçek zamanlı envanter',      color: '#a78bfa' },
];

const MOCK_PRICE = (sku: string) => {
  let h = 0;
  for (let i = 0; i < sku.length; i++) h = (h * 31 + sku.charCodeAt(i)) >>> 0;
  return (50 + (h % 950));
};

export default function UserHome() {
  const [featured, setFeatured] = useState<Product[]>([]);
  const { addItem } = useCart();
  const navigate = useNavigate();

  useEffect(() => {
    api.get('/Product?pageSize=6').then(res => {
      const data = res.data?.items || res.data?.data || [];
      setFeatured(data.slice(0, 6));
    }).catch(() => setFeatured([]));
  }, []);

  const handleAddToCart = (p: Product, e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    const price = p.price ?? MOCK_PRICE(p.sku);
    addItem({
      id: p.id,
      name: p.name,
      sku: p.sku,
      price,
      categoryName: p.categoryName,
      imageGradient: GRADIENTS[parseInt(p.id, 16) % GRADIENTS.length] ?? GRADIENTS[0],
    });
  };

  return (
    <div className="user-page">
      {/* ── Hero ── */}
      <section className="user-hero animate-fade-up">
        <div className="user-hero-badge">
          <TrendingUp size={12} />
          Acil Tedarik Platformu
        </div>
        <h1 className="user-hero-title">
          İhtiyacınız Olan Her Şey<br />
          <span className="user-hero-gradient">Tek Platformda</span>
        </h1>
        <p className="user-hero-sub">
          Afet, lojistik ve acil durum ekipmanlarını hızlıca temin edin.
          Binlerce onaylı ürün, güvenli ödeme, anlık teslimat takibi.
        </p>
        <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap', justifyContent: 'center' }}>
          <button className="btn btn-primary" style={{ padding: '14px 32px', fontSize: '1rem' }}
            onClick={() => navigate('/user/catalog')}>
            Alışverişe Başla <ArrowRight size={18} />
          </button>
          <button className="btn btn-ghost" style={{ padding: '14px 28px', fontSize: '1rem' }}
            onClick={() => navigate('/user/catalog')}>
            Kataloğu Gör
          </button>
        </div>

        {/* Hero stats */}
        <div className="user-hero-stats">
          {[
            { value: '12,000+', label: 'Aktif Ürün' },
            { value: '48',      label: 'Lojistik Tesis' },
            { value: '99.8%',   label: 'Müşteri Memnuniyeti' },
            { value: '24/7',    label: 'Destek' },
          ].map((s, i) => (
            <div key={i} className="user-hero-stat">
              <div className="user-hero-stat-value">{s.value}</div>
              <div className="user-hero-stat-label">{s.label}</div>
            </div>
          ))}
        </div>
      </section>

      {/* ── Features ── */}
      <section style={{ padding: '0 0 64px' }}>
        <div className="user-section-label">Neden E-Satis?</div>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: 20 }}>
          {features.map(({ icon: Icon, title, desc, color }, i) => (
            <div key={i} className="glass-card animate-fade-up" style={{ animationDelay: `${i * 0.07}s`, padding: '28px 24px' }}>
              <div style={{
                width: 48, height: 48, borderRadius: 14,
                background: `${color}20`, border: `1px solid ${color}33`,
                display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: 16,
              }}>
                <Icon size={22} color={color} />
              </div>
              <h3 style={{ fontSize: '1rem', fontWeight: 700, marginBottom: 6 }}>{title}</h3>
              <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', lineHeight: 1.6 }}>{desc}</p>
            </div>
          ))}
        </div>
      </section>

      {/* ── Featured Products ── */}
      <section>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
          <div>
            <div className="user-section-label">Öne Çıkan Ürünler</div>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 700 }}>Popüler Ürünler</h2>
          </div>
          <Link to="/user/catalog" className="btn btn-ghost" style={{ gap: 6, fontSize: '0.88rem' }}>
            Tümünü Gör <ArrowRight size={14} />
          </Link>
        </div>

        {featured.length === 0 ? (
          <div style={{ textAlign: 'center', padding: '60px 0', color: 'var(--text-muted)' }}>
            <Package size={40} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
            <p>Ürünler yükleniyor...</p>
          </div>
        ) : (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(260px, 1fr))', gap: 20 }}>
            {featured.map((p, i) => {
              const price = p.price ?? MOCK_PRICE(p.sku);
              const grad  = GRADIENTS[i % GRADIENTS.length];
              return (
                <Link
                  key={p.id}
                  to={`/user/catalog/${p.id}`}
                  className="user-product-card animate-fade-up"
                  style={{ textDecoration: 'none', animationDelay: `${i * 0.06}s` } as React.CSSProperties}
                >
                  <div className="user-product-img" style={{ background: grad }}>
                    <ShoppingBag size={36} color="rgba(255,255,255,0.6)" />
                  </div>
                  <div className="user-product-body">
                    {p.categoryName && (
                      <span className="badge badge-purple" style={{ fontSize: '0.7rem', marginBottom: 6 }}>{p.categoryName}</span>
                    )}
                    <h3 className="user-product-name">{p.name}</h3>
                    <code style={{ fontSize: '0.75rem', color: 'var(--text-muted)', display: 'block', marginBottom: 12 }}>{p.sku}</code>
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <span className="user-product-price">₺{price.toLocaleString('tr-TR')}</span>
                      <button
                        className="btn btn-primary"
                        style={{ padding: '8px 16px', fontSize: '0.82rem' }}
                        onClick={e => handleAddToCart(p, e)}
                      >
                        Sepete Ekle
                      </button>
                    </div>
                  </div>
                </Link>
              );
            })}
          </div>
        )}
      </section>

      {/* ── CTA Banner ── */}
      <section className="user-cta-banner">
        <div style={{ textAlign: 'center' }}>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 700, marginBottom: 10 }}>Acil Tedarik Mi Gerekiyor?</h2>
          <p style={{ color: 'var(--text-secondary)', marginBottom: 24, fontSize: '0.95rem' }}>
            7/24 öncelikli destek ekibimizle iletişime geçin veya toplu sipariş formu doldurun.
          </p>
          <button className="btn btn-primary" style={{ padding: '14px 36px', fontSize: '1rem' }}
            onClick={() => navigate('/user/catalog')}>
            Toplu Sipariş <ArrowRight size={16} />
          </button>
        </div>
      </section>
    </div>
  );
}
