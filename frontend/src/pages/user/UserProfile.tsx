import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Package, MapPin, Bell, Shield, ChevronRight,
  Edit3, LogOut, ShoppingBag, Clock,
} from 'lucide-react';

const MOCK_ORDERS = [
  { id: 'ESA-A4F2B1', date: '27 Mayıs 2026', total: '₺2.340', status: 'Teslim Edildi', items: 3 },
  { id: 'ESA-C8D3E0', date: '21 Mayıs 2026', total: '₺890',   status: 'Kargoda',       items: 1 },
  { id: 'ESA-F1A9B7', date: '10 Mayıs 2026', total: '₺5.120', status: 'Teslim Edildi', items: 7 },
];

const MOCK_ADDRESSES = [
  { label: 'Ev', full: 'Bağcılar Mah. Atatürk Cad. No:14/3 Kadıköy / İstanbul 34000' },
  { label: 'İş', full: 'Maslak Plaza Kat:7 Sarıyer / İstanbul 34398' },
];

const tabs = [
  { key: 'orders',    label: 'Siparişlerim',    icon: Package },
  { key: 'addresses', label: 'Adreslerim',      icon: MapPin },
  { key: 'security',  label: 'Güvenlik',        icon: Shield },
  { key: 'notifs',    label: 'Bildirimler',     icon: Bell },
];

const statusConfig: Record<string, { badge: string; dot: string }> = {
  'Teslim Edildi': { badge: 'badge-green',  dot: '#6ee7b7' },
  'Kargoda':       { badge: 'badge-blue',   dot: '#93c5fd' },
  'İşleniyor':     { badge: 'badge-amber',  dot: '#fcd34d' },
};

export default function UserProfile() {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('orders');
  const [editing, setEditing] = useState(false);
  const [userName, setUserName] = useState('Kullanıcı');
  const [userEmail, setUserEmail] = useState('kullanici@esatis.com');

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/login');
  };

  return (
    <div className="user-page">
      <div style={{ marginBottom: 32 }}>
        <div className="user-section-label">Hesabım</div>
        <h1 style={{ fontSize: '1.9rem', fontWeight: 700 }}>Profilim</h1>
      </div>

      <div className="user-profile-grid">
        {/* Left: profile card + sidebar */}
        <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
          {/* Avatar card */}
          <div className="glass-card" style={{ textAlign: 'center', padding: '32px 24px' }}>
            <div className="avatar" style={{
              width: 72, height: 72, fontSize: '1.6rem', borderRadius: 20,
              margin: '0 auto 16px',
              background: 'linear-gradient(135deg, #7c3aed, #ec4899)',
              boxShadow: '0 0 30px rgba(124,58,237,0.4)',
            }}>
              {userName.charAt(0).toUpperCase()}
            </div>
            {editing ? (
              <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
                <input className="form-input" style={{ textAlign: 'center', fontSize: '0.9rem' }}
                  value={userName} onChange={e => setUserName(e.target.value)} />
                <input className="form-input" style={{ textAlign: 'center', fontSize: '0.88rem' }}
                  value={userEmail} onChange={e => setUserEmail(e.target.value)} />
                <button className="btn btn-primary" style={{ padding: '8px', fontSize: '0.85rem' }}
                  onClick={() => setEditing(false)}>Kaydet</button>
              </div>
            ) : (
              <>
                <div style={{ fontWeight: 700, fontSize: '1.05rem', marginBottom: 4 }}>{userName}</div>
                <div style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 16 }}>{userEmail}</div>
                <button className="btn btn-ghost" style={{ gap: 6, fontSize: '0.82rem', padding: '8px 14px' }}
                  onClick={() => setEditing(true)}>
                  <Edit3 size={13} /> Düzenle
                </button>
              </>
            )}
          </div>

          {/* Tab nav */}
          <div className="glass-card" style={{ padding: 8 }}>
            {tabs.map(({ key, label, icon: Icon }) => (
              <button
                key={key}
                onClick={() => setActiveTab(key)}
                style={{
                  width: '100%', display: 'flex', alignItems: 'center', gap: 10,
                  padding: '12px 16px', borderRadius: 10, border: 'none',
                  background: activeTab === key ? 'rgba(124,58,237,0.15)' : 'transparent',
                  color: activeTab === key ? '#a78bfa' : 'var(--text-secondary)',
                  fontSize: '0.9rem', fontWeight: 500, cursor: 'pointer',
                  transition: 'all 0.2s',
                }}
              >
                <Icon size={16} />
                <span style={{ flex: 1, textAlign: 'left' }}>{label}</span>
                {activeTab === key && <ChevronRight size={14} style={{ opacity: 0.5 }} />}
              </button>
            ))}

            <div style={{ borderTop: '1px solid var(--glass-border)', margin: '8px 0', paddingTop: 8 }}>
              <button
                onClick={handleLogout}
                style={{
                  width: '100%', display: 'flex', alignItems: 'center', gap: 10,
                  padding: '12px 16px', borderRadius: 10, border: 'none',
                  background: 'transparent', color: '#f87171',
                  fontSize: '0.9rem', fontWeight: 500, cursor: 'pointer',
                }}
              >
                <LogOut size={16} /> Çıkış Yap
              </button>
            </div>
          </div>
        </div>

        {/* Right: tab content */}
        <div className="animate-fade-up">

          {/* Orders tab */}
          {activeTab === 'orders' && (
            <div>
              <h2 style={{ fontSize: '1.15rem', fontWeight: 700, marginBottom: 20 }}>Sipariş Geçmişi</h2>
              {MOCK_ORDERS.length === 0 ? (
                <div style={{ textAlign: 'center', padding: '60px 0', color: 'var(--text-muted)' }}>
                  <ShoppingBag size={40} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
                  <p>Henüz siparişiniz bulunmuyor.</p>
                  <button className="btn btn-primary" style={{ marginTop: 16 }} onClick={() => navigate('/user/catalog')}>Alışverişe Başla</button>
                </div>
              ) : (
                <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                  {MOCK_ORDERS.map(order => {
                    const cfg = statusConfig[order.status] ?? statusConfig['İşleniyor'];
                    return (
                      <div key={order.id} className="glass-card" style={{ display: 'flex', alignItems: 'center', gap: 16, padding: '20px 24px' }}>
                        <div style={{ width: 44, height: 44, borderRadius: 12, background: 'rgba(124,58,237,0.12)', border: '1px solid rgba(124,58,237,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                          <Package size={20} color="#a78bfa" />
                        </div>
                        <div style={{ flex: 1, minWidth: 0 }}>
                          <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 4, flexWrap: 'wrap' }}>
                            <code style={{ fontWeight: 700, color: '#a78bfa', fontSize: '0.9rem' }}>{order.id}</code>
                            <span className={`badge ${cfg.badge}`}>
                              <span style={{ width: 5, height: 5, borderRadius: '50%', background: cfg.dot, display: 'inline-block' }} />
                              {order.status}
                            </span>
                          </div>
                          <div style={{ fontSize: '0.82rem', color: 'var(--text-muted)', display: 'flex', alignItems: 'center', gap: 6 }}>
                            <Clock size={12} /> {order.date} · {order.items} ürün
                          </div>
                        </div>
                        <div style={{ textAlign: 'right' }}>
                          <div style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1rem', color: 'var(--text-primary)' }}>{order.total}</div>
                          <button className="btn btn-ghost" style={{ padding: '5px 12px', fontSize: '0.78rem', marginTop: 6 }}>Detay</button>
                        </div>
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          )}

          {/* Addresses tab */}
          {activeTab === 'addresses' && (
            <div>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
                <h2 style={{ fontSize: '1.15rem', fontWeight: 700 }}>Kayıtlı Adreslerim</h2>
                <button className="btn btn-primary" style={{ padding: '8px 16px', fontSize: '0.85rem', gap: 6 }}>
                  <MapPin size={13} /> Adres Ekle
                </button>
              </div>
              <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                {MOCK_ADDRESSES.map((a, i) => (
                  <div key={i} className="glass-card" style={{ display: 'flex', gap: 16, padding: '20px 24px' }}>
                    <div style={{ width: 44, height: 44, borderRadius: 12, background: 'rgba(59,130,246,0.12)', border: '1px solid rgba(59,130,246,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                      <MapPin size={20} color="#93c5fd" />
                    </div>
                    <div style={{ flex: 1 }}>
                      <div style={{ fontWeight: 700, marginBottom: 4 }}>{a.label}</div>
                      <div style={{ fontSize: '0.87rem', color: 'var(--text-secondary)', lineHeight: 1.6 }}>{a.full}</div>
                    </div>
                    <button className="btn btn-ghost" style={{ padding: '6px 12px', fontSize: '0.8rem', alignSelf: 'flex-start' }}>
                      <Edit3 size={12} />
                    </button>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Security tab */}
          {activeTab === 'security' && (
            <div>
              <h2 style={{ fontSize: '1.15rem', fontWeight: 700, marginBottom: 20 }}>Güvenlik Ayarları</h2>
              <div className="glass-card" style={{ padding: '24px' }}>
                <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
                  <div className="form-group">
                    <label className="form-label">Mevcut Şifre</label>
                    <input type="password" className="form-input" placeholder="••••••••" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Yeni Şifre</label>
                    <input type="password" className="form-input" placeholder="••••••••" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Yeni Şifre (Tekrar)</label>
                    <input type="password" className="form-input" placeholder="••••••••" />
                  </div>
                  <button className="btn btn-primary" style={{ alignSelf: 'flex-start', padding: '10px 24px' }}>
                    Şifreyi Güncelle
                  </button>
                </div>
              </div>
            </div>
          )}

          {/* Notifications tab */}
          {activeTab === 'notifs' && (
            <div>
              <h2 style={{ fontSize: '1.15rem', fontWeight: 700, marginBottom: 20 }}>Bildirim Tercihleri</h2>
              <div className="glass-card" style={{ padding: '24px' }}>
                {[
                  { label: 'Sipariş Güncellemeleri',  sub: 'Kargo ve teslimat bildirimleri',     on: true },
                  { label: 'Kampanyalar',              sub: 'İndirim ve promosyon haberleri',     on: false },
                  { label: 'Fiyat Düşüşleri',         sub: 'Takip ettiğin ürün fiyat bildirimi', on: true },
                  { label: 'Güvenlik Uyarıları',      sub: 'Hesap giriş ve şifre uyarıları',     on: true },
                ].map((n, i) => (
                  <div key={i} style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '16px 0', borderBottom: i < 3 ? '1px solid var(--glass-border)' : 'none' }}>
                    <div>
                      <div style={{ fontWeight: 500, marginBottom: 2 }}>{n.label}</div>
                      <div style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>{n.sub}</div>
                    </div>
                    <div className={`user-toggle ${n.on ? 'on' : ''}`} />
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
