import { Package, Building2, Users, Zap, TrendingUp, ArrowUpRight, Activity } from 'lucide-react';
import { Link } from 'react-router-dom';

const stats = [
  {
    label: 'Toplam Ürün',
    value: '1,284',
    change: '+12%',
    positive: true,
    icon: Package,
    iconBg: 'rgba(124, 58, 237, 0.15)',
    iconColor: '#a78bfa',
    badgeBg: 'rgba(16, 185, 129, 0.1)',
    badgeColor: '#6ee7b7',
  },
  {
    label: 'Kayıtlı Tesis',
    value: '48',
    change: '+3',
    positive: true,
    icon: Building2,
    iconBg: 'rgba(59, 130, 246, 0.15)',
    iconColor: '#93c5fd',
    badgeBg: 'rgba(16, 185, 129, 0.1)',
    badgeColor: '#6ee7b7',
  },
  {
    label: 'Aktif Kullanıcı',
    value: '215',
    change: '-2%',
    positive: false,
    icon: Users,
    iconBg: 'rgba(236, 72, 153, 0.15)',
    iconColor: '#f9a8d4',
    badgeBg: 'rgba(239, 68, 68, 0.1)',
    badgeColor: '#f87171',
  },
  {
    label: 'İşlem Hacmi',
    value: '₺4.2M',
    change: '+28%',
    positive: true,
    icon: Zap,
    iconBg: 'rgba(245, 158, 11, 0.15)',
    iconColor: '#fcd34d',
    badgeBg: 'rgba(16, 185, 129, 0.1)',
    badgeColor: '#6ee7b7',
  },
];

const recentActivity = [
  { action: 'Yeni ürün eklendi', subject: 'Çadır XL 6 Kişilik', time: '2 dakika önce', type: 'success' },
  { action: 'Stok güncellendi', subject: 'Jeneratör 5kW', time: '14 dakika önce', type: 'info' },
  { action: 'Tesis oluşturuldu', subject: 'Ankara Depo B3', time: '1 saat önce', type: 'purple' },
  { action: 'Kullanıcı kaydoldu', subject: 'operator@esatis.com', time: '3 saat önce', type: 'info' },
  { action: 'Envanter sevkiyatı', subject: 'İlk Yardım Seti x200', time: '5 saat önce', type: 'amber' },
];

const typeColor: Record<string, string> = {
  success: '#6ee7b7',
  info: '#93c5fd',
  purple: '#a78bfa',
  amber: '#fcd34d',
};

export default function Dashboard() {
  return (
    <div>
      {/* Page header */}
      <div className="page-header">
        <div>
          <h1 className="page-title">Dashboard</h1>
          <p className="page-subtitle">Sistemin genel durumuna hoş geldiniz.</p>
        </div>
        <button className="btn btn-primary">
          <TrendingUp size={16} />
          Rapor Oluştur
        </button>
      </div>

      {/* Stat cards */}
      <div className="stat-grid">
        {stats.map((s, i) => {
          const Icon = s.icon;
          return (
            <div
              className={`stat-card animate-fade-up animate-fade-up-${i + 1}`}
              key={s.label}
            >
              <div className="stat-card-top">
                <div
                  className="stat-icon"
                  style={{ background: s.iconBg }}
                >
                  <Icon size={22} color={s.iconColor} />
                </div>
                <span
                  className="stat-badge"
                  style={{ background: s.badgeBg, color: s.badgeColor }}
                >
                  {s.change}
                </span>
              </div>
              <div>
                <div className="stat-value">{s.value}</div>
                <div className="stat-label">{s.label}</div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Bottom grid */}
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>

        {/* Recent Activity */}
        <div className="data-table-wrapper animate-fade-up animate-fade-up-3">
          <div className="data-table-header">
            <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
              <Activity size={18} style={{ color: 'var(--neon-purple-light)' }} />
              <span style={{ fontWeight: 600 }}>Son Aktiviteler</span>
            </div>
            <button className="btn btn-ghost" style={{ padding: '6px 12px', fontSize: '0.8rem' }}>
              Tümünü gör <ArrowUpRight size={13} />
            </button>
          </div>

          <div style={{ padding: '8px 0' }}>
            {recentActivity.map((a, idx) => (
              <div key={idx} style={{
                display: 'flex',
                alignItems: 'center',
                gap: '16px',
                padding: '14px 24px',
                borderBottom: idx < recentActivity.length - 1 ? '1px solid rgba(255,255,255,0.03)' : 'none',
                transition: 'background 0.15s',
                cursor: 'default',
              }}
                onMouseEnter={e => (e.currentTarget.style.background = 'rgba(255,255,255,0.03)')}
                onMouseLeave={e => (e.currentTarget.style.background = 'transparent')}
              >
                {/* Indicator dot */}
                <div style={{
                  width: '8px',
                  height: '8px',
                  borderRadius: '50%',
                  background: typeColor[a.type],
                  boxShadow: `0 0 10px ${typeColor[a.type]}`,
                  flexShrink: 0,
                }} />
                <div style={{ flex: 1 }}>
                  <div style={{ fontSize: '0.88rem', color: 'var(--text-secondary)' }}>
                    {a.action} —{' '}
                    <span style={{ color: 'var(--text-primary)', fontWeight: 500 }}>{a.subject}</span>
                  </div>
                </div>
                <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)', whiteSpace: 'nowrap' }}>
                  {a.time}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Quick Access */}
        <div className="data-table-wrapper animate-fade-up animate-fade-up-4">
          <div className="data-table-header">
            <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
              <Zap size={18} style={{ color: '#fcd34d' }} />
              <span style={{ fontWeight: 600 }}>Hızlı Erişim</span>
            </div>
          </div>
          <div style={{ padding: '20px', display: 'flex', flexDirection: 'column', gap: '12px' }}>
            {[
              { label: 'Yeni Ürün Ekle', sub: 'Katalog veritabanına ürün ekle', color: '#a78bfa', bg: 'rgba(124,58,237,0.08)', border: 'rgba(124,58,237,0.2)' },
              { label: 'Yeni Tesis Oluştur', sub: 'Bölge veya depo kaydı aç', color: '#93c5fd', bg: 'rgba(59,130,246,0.08)', border: 'rgba(59,130,246,0.2)' },
              { label: 'Envanter Sevk Et', sub: 'Stok sevkiyat emri oluştur', color: '#6ee7b7', bg: 'rgba(16,185,129,0.08)', border: 'rgba(16,185,129,0.2)' },
              { label: 'Kategori Yönet', sub: 'Şema ve attribute düzenle', color: '#fcd34d', bg: 'rgba(245,158,11,0.08)', border: 'rgba(245,158,11,0.2)' },
            ].map((item, i) => (
              <div key={i} style={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                padding: '14px 18px',
                background: item.bg,
                border: `1px solid ${item.border}`,
                borderRadius: '12px',
                cursor: 'pointer',
                transition: 'all 0.2s',
              }}
                onMouseEnter={e => {
                  (e.currentTarget as HTMLDivElement).style.transform = 'translateX(4px)';
                  (e.currentTarget as HTMLDivElement).style.boxShadow = `0 4px 20px rgba(0,0,0,0.2)`;
                }}
                onMouseLeave={e => {
                  (e.currentTarget as HTMLDivElement).style.transform = '';
                  (e.currentTarget as HTMLDivElement).style.boxShadow = '';
                }}
              >
                <div>
                  <div style={{ fontSize: '0.9rem', fontWeight: 600, color: item.color, marginBottom: '2px' }}>{item.label}</div>
                  <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)' }}>{item.sub}</div>
                </div>
                <ArrowUpRight size={16} style={{ color: item.color, flexShrink: 0 }} />
              </div>
            ))}
          </div>
        </div>

      </div>

      {/* Test Links */}
      <div className="data-table-wrapper animate-fade-up animate-fade-up-5" style={{ marginTop: '20px' }}>
        <div className="data-table-header">
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
            <Activity size={18} style={{ color: '#f87171' }} />
            <span style={{ fontWeight: 600 }}>Tüm Sayfalar (Test İçin Hızlı Erişim)</span>
          </div>
        </div>
        <div style={{ padding: '20px', display: 'flex', flexWrap: 'wrap', gap: '12px' }}>
          {[
            { label: 'Admin - Dashboard', path: '/dashboard', color: '#a78bfa' },
            { label: 'Admin - Products', path: '/products', color: '#a78bfa' },
            { label: 'Admin - Facilities', path: '/facilities', color: '#a78bfa' },
            { label: 'Admin - Categories', path: '/categories', color: '#a78bfa' },
            { label: 'Admin - Inventory', path: '/inventory', color: '#a78bfa' },
            { label: 'User - Home', path: '/user', color: '#6ee7b7' },
            { label: 'User - Catalog', path: '/user/catalog', color: '#6ee7b7' },
            { label: 'User - Product Detail (Test ID)', path: '/user/catalog/test-id', color: '#6ee7b7' },
            { label: 'User - Cart', path: '/user/cart', color: '#6ee7b7' },
            { label: 'User - Checkout', path: '/user/checkout', color: '#6ee7b7' },
            { label: 'User - Order Success', path: '/user/order-success', color: '#6ee7b7' },
            { label: 'User - Profile', path: '/user/profile', color: '#6ee7b7' },
            { label: 'Login', path: '/login', color: '#fcd34d' },
          ].map((item, i) => (
            <Link key={i} to={item.path} style={{
              display: 'inline-flex',
              alignItems: 'center',
              gap: '6px',
              padding: '10px 16px',
              background: `rgba(255,255,255,0.05)`,
              border: `1px solid ${item.color}40`,
              borderRadius: '8px',
              textDecoration: 'none',
              color: 'var(--text-primary)',
              fontSize: '0.85rem',
              transition: 'all 0.2s',
            }}
              onMouseEnter={e => {
                (e.currentTarget as HTMLAnchorElement).style.background = `rgba(255,255,255,0.1)`;
                (e.currentTarget as HTMLAnchorElement).style.borderColor = item.color;
              }}
              onMouseLeave={e => {
                (e.currentTarget as HTMLAnchorElement).style.background = `rgba(255,255,255,0.05)`;
                (e.currentTarget as HTMLAnchorElement).style.borderColor = `${item.color}40`;
              }}
            >
              {item.label}
              <ArrowUpRight size={14} style={{ color: item.color }} />
            </Link>
          ))}
        </div>
      </div>
    </div>
  );
}
