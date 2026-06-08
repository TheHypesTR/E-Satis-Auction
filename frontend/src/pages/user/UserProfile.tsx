import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Package, MapPin, Bell, Shield, ChevronRight,
  Edit3, LogOut, ShoppingBag, Clock, CreditCard, X, Check, XCircle
} from 'lucide-react';

import { useEffect } from 'react';
import api from '../../api/axios';

interface Order {
  id: string;
  orderNumber: string;
  totalAmount: number;
  createdAt: string;
  status: string;
}

const MOCK_ADDRESSES = [
  { label: 'Ev', full: 'Merkez Mah. Bor Cad. No:1 Niğde Merkez / Niğde 51000' },
];

const MOCK_CARDS = [
  { id: '1', bank: 'Ziraat Bankası', last4: '1234', brand: 'Troy' },
];

const tabs = [
  { key: 'orders',    label: 'Siparişlerim',    icon: Package },
  { key: 'addresses', label: 'Adreslerim',      icon: MapPin },
  { key: 'cards',     label: 'Kayıtlı Kartlarım',icon: CreditCard },
  { key: 'security',  label: 'Güvenlik',        icon: Shield },
  { key: 'notifs',    label: 'Bildirimler',     icon: Bell },
];

const statusTranslations: Record<string, string> = {
  'PendingApproval': 'Bekliyor',
  'Approved': 'Onaylandı',
  'Shipped': 'Kargolandı',
  'Cancelled': 'İptal',
  'Rejected': 'Reddedildi',
  'Delivered': 'Teslim Edildi',
  'PaymentPending': 'Ödeme Bekleniyor'
};

const statusConfig: Record<string, { badge: string; dot: string }> = {
  'Delivered': { badge: 'badge-green',  dot: '#6ee7b7' },
  'Shipped':       { badge: 'badge-blue',   dot: '#93c5fd' },
  'PendingApproval':     { badge: 'badge-amber',  dot: '#fcd34d' },
  'Approved': { badge: 'badge-blue', dot: '#93c5fd' },
  'Cancelled': { badge: 'badge-red', dot: '#f87171' },
  'Rejected': { badge: 'badge-red', dot: '#f87171' },
  'PaymentPending': { badge: 'badge-amber', dot: '#fcd34d' }
};

export default function UserProfile() {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('orders');
  const [editing, setEditing] = useState(false);
  const [userName, setUserName] = useState('Kullanıcı');
  const [userEmail, setUserEmail] = useState('kullanici@esatis.com');
  const [orders, setOrders] = useState<Order[]>([]);
  const [loadingOrders, setLoadingOrders] = useState(false);

  useEffect(() => {
    setLoadingOrders(true);
    api.get('/PurchaseOrder')
      .then(res => {
        const data = res.data?.items || res.data?.data || [];
        setOrders(data);
        setLoadingOrders(false);
      })
      .catch(() => {
        setOrders([]);
        setLoadingOrders(false);
      });
  }, []);

  // Modals
  const [showAddAddress, setShowAddAddress] = useState(false);
  const [showAddCard, setShowAddCard] = useState(false);
  const [showCancelOrderModal, setShowCancelOrderModal] = useState(false);
  const [cancelingOrder, setCancelingOrder] = useState<Order | null>(null);
  const [actionSaved, setActionSaved] = useState(false);

  const handleMockAction = (setter: React.Dispatch<React.SetStateAction<boolean>>) => {
    setActionSaved(true);
    setTimeout(() => {
      setActionSaved(false);
      setter(false);
    }, 1000);
  };

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
              {loadingOrders ? (
                <div style={{ textAlign: 'center', padding: '60px 0', color: 'var(--text-muted)' }}>
                  <p>Siparişler yükleniyor...</p>
                </div>
              ) : orders.length === 0 ? (
                <div style={{ textAlign: 'center', padding: '60px 0', color: 'var(--text-muted)' }}>
                  <ShoppingBag size={40} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
                  <p>Henüz siparişiniz bulunmuyor.</p>
                  <button className="btn btn-primary" style={{ marginTop: 16 }} onClick={() => navigate('/user/catalog')}>Alışverişe Başla</button>
                </div>
              ) : (
                <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                  {orders.map(order => {
                    const cfg = statusConfig[order.status] ?? statusConfig['PendingApproval'];
                    return (
                      <div key={order.id} className="glass-card" style={{ display: 'flex', alignItems: 'center', gap: 16, padding: '20px 24px' }}>
                        <div style={{ width: 44, height: 44, borderRadius: 12, background: 'rgba(124,58,237,0.12)', border: '1px solid rgba(124,58,237,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                          <Package size={20} color="#a78bfa" />
                        </div>
                        <div style={{ flex: 1, minWidth: 0 }}>
                          <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 4, flexWrap: 'wrap' }}>
                            <code style={{ fontWeight: 700, color: '#a78bfa', fontSize: '0.9rem' }}>{order.orderNumber}</code>
                            <span className={`badge ${cfg.badge}`}>
                              <span style={{ width: 5, height: 5, borderRadius: '50%', background: cfg.dot, display: 'inline-block' }} />
                              {statusTranslations[order.status] || order.status}
                            </span>
                          </div>
                          <div style={{ fontSize: '0.82rem', color: 'var(--text-muted)', display: 'flex', alignItems: 'center', gap: 6 }}>
                            <Clock size={12} /> {new Date(order.createdAt).toLocaleDateString('tr-TR')}
                          </div>
                        </div>
                        <div style={{ textAlign: 'right' }}>
                          <div style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1rem', color: 'var(--text-primary)' }}>₺{order.totalAmount.toLocaleString('tr-TR')}</div>
                          <div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end', marginTop: 6 }}>
                            {(order.status === 'PendingApproval' || order.status === 'PaymentPending') && (
                              <button className="btn btn-ghost" style={{ padding: '5px 12px', fontSize: '0.78rem', color: '#f87171' }} onClick={() => { setCancelingOrder(order); setShowCancelOrderModal(true); }}>İptal Et</button>
                            )}
                            <button className="btn btn-ghost" style={{ padding: '5px 12px', fontSize: '0.78rem' }}>Detay</button>
                          </div>
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
                <button className="btn btn-primary" style={{ padding: '8px 16px', fontSize: '0.85rem', gap: 6 }} onClick={() => setShowAddAddress(true)}>
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

          {/* Cards tab */}
          {activeTab === 'cards' && (
            <div>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
                <h2 style={{ fontSize: '1.15rem', fontWeight: 700 }}>Kayıtlı Kartlarım</h2>
                <button className="btn btn-primary" style={{ padding: '8px 16px', fontSize: '0.85rem', gap: 6 }} onClick={() => setShowAddCard(true)}>
                  <CreditCard size={13} /> Kart Ekle
                </button>
              </div>
              <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                {MOCK_CARDS.map((c) => (
                  <div key={c.id} className="glass-card" style={{ display: 'flex', gap: 16, padding: '20px 24px', alignItems: 'center' }}>
                    <div style={{ width: 44, height: 44, borderRadius: 12, background: 'rgba(16, 185, 129, 0.12)', border: '1px solid rgba(16, 185, 129, 0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                      <CreditCard size={20} color="#6ee7b7" />
                    </div>
                    <div style={{ flex: 1 }}>
                      <div style={{ fontWeight: 700, marginBottom: 4 }}>{c.bank}</div>
                      <div style={{ fontSize: '0.87rem', color: 'var(--text-secondary)' }}>
                        {c.brand} •••• {c.last4}
                      </div>
                    </div>
                    <button className="btn btn-ghost" style={{ padding: '6px 12px', fontSize: '0.8rem', color: '#f87171' }}>
                      Sil
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

      {/* Add Address Modal */}
      {showAddAddress && (
        <div className="modal-overlay" onClick={() => setShowAddAddress(false)}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 500, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8 }}>
                <MapPin size={20} color="#93c5fd" /> Yeni Adres Ekle
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowAddAddress(false)}>
                <X size={18} />
              </button>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 16, marginBottom: 24 }}>
              <div className="form-group">
                <label className="form-label">Adres Başlığı</label>
                <input type="text" className="form-input" placeholder="Örn: Ev, İş" />
              </div>
              <div className="form-group">
                <label className="form-label">İl / İlçe</label>
                <div style={{ display: 'flex', gap: 12 }}>
                  <input type="text" className="form-input" placeholder="İl" style={{ flex: 1 }} />
                  <input type="text" className="form-input" placeholder="İlçe" style={{ flex: 1 }} />
                </div>
              </div>
              <div className="form-group">
                <label className="form-label">Açık Adres</label>
                <textarea className="form-input" placeholder="Mahalle, sokak, bina no..." rows={3} style={{ resize: 'none' }}></textarea>
              </div>
            </div>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => setShowAddAddress(false)}>İptal</button>
              <button className="btn btn-primary" onClick={() => setShowAddAddress(false)}>Kaydet</button>
            </div>
          </div>
        </div>
      )}

      {/* Add Card Modal */}
      {showAddCard && (
        <div className="modal-overlay" onClick={() => setShowAddCard(false)}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 400, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8 }}>
                <CreditCard size={20} color="#6ee7b7" /> Yeni Kart Ekle
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowAddCard(false)}>
                <X size={18} />
              </button>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 16, marginBottom: 24 }}>
              <div className="form-group">
                <label className="form-label">Kart Üzerindeki İsim</label>
                <input type="text" className="form-input" placeholder="Ad Soyad" />
              </div>
              <div className="form-group">
                <label className="form-label">Kart Numarası</label>
                <input type="text" className="form-input" placeholder="0000 0000 0000 0000" maxLength={19} />
              </div>
              <div style={{ display: 'flex', gap: 12 }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label className="form-label">Son Kullanma (AA/YY)</label>
                  <input type="text" className="form-input" placeholder="MM/YY" maxLength={5} />
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label className="form-label">CVV</label>
                  <input type="text" className="form-input" placeholder="123" maxLength={3} />
                </div>
              </div>
            </div>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => setShowAddCard(false)}>İptal</button>
              <button className="btn btn-primary" onClick={() => handleMockAction(setShowAddCard)}>
                {actionSaved ? <><Check size={16}/> Kaydedildi</> : 'Kaydet'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Cancel Order Modal */}
      {showCancelOrderModal && cancelingOrder && (
        <div className="modal-overlay" onClick={() => setShowCancelOrderModal(false)}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 450, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8, color: '#f87171' }}>
                <XCircle size={20} /> Siparişi İptal Et
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowCancelOrderModal(false)}>
                <X size={18} />
              </button>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 16, marginBottom: 24 }}>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem', lineHeight: 1.5 }}>
                <strong style={{ color: 'var(--text-primary)' }}>{cancelingOrder.orderNumber}</strong> numaralı siparişinizi iptal etmek istediğinize emin misiniz? Bu işlem geri alınamaz.
              </p>
              <div className="form-group">
                <label className="form-label">İptal Nedeni</label>
                <select className="form-input" style={{ appearance: 'auto', backgroundColor: 'var(--bg-secondary)' }}>
                  <option value="">Seçiniz...</option>
                  <option value="Yanlış ürün siparişi verdim">Yanlış ürün siparişi verdim</option>
                  <option value="Sipariş vermekten vazgeçtim">Sipariş vermekten vazgeçtim</option>
                  <option value="Teslimat süresi çok uzun">Teslimat süresi çok uzun</option>
                  <option value="Diğer">Diğer</option>
                </select>
              </div>
            </div>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => setShowCancelOrderModal(false)}>Vazgeç</button>
              <button className="btn" style={{ background: 'rgba(248,113,113,0.15)', color: '#f87171', border: '1px solid rgba(248,113,113,0.3)' }} onClick={() => handleMockAction(setShowCancelOrderModal)}>
                {actionSaved ? <><Check size={16}/> İptal Edildi</> : 'Evet, İptal Et'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
