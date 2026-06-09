import { Link, useLocation, useNavigate } from 'react-router-dom';
import {
  LayoutDashboard,
  Package,
  Building2,
  Tag,
  Zap,
  LogOut,
  ChevronRight,
  Layers,
  ShoppingCart,
  RefreshCcw,
  Store,
  Gift,
  Handshake,
  Gavel,
} from 'lucide-react';

const navItems = [
  { label: 'Dashboard', path: '/dashboard', icon: LayoutDashboard },
  { label: 'Siparişler', path: '/orders', icon: ShoppingCart },
  { label: 'İade Talepleri', path: '/returns', icon: RefreshCcw },
  { label: 'Ürünler', path: '/products', icon: Package },
  { label: 'Satış İlanları', path: '/listings', icon: Store },
  { label: 'Kampanyalar', path: '/campaigns', icon: Gift },
  { label: 'Alım Talepleri', path: '/user-sale-requests', icon: Handshake },
  { label: 'Açık Artırmalar', path: '/auctions', icon: Gavel },
  { label: 'Tesisler', path: '/facilities', icon: Building2 },
  { label: 'Kategoriler', path: '/categories', icon: Tag },
  { label: 'Envanter', path: '/inventory', icon: Layers },
];

export default function Sidebar() {
  const location = useLocation();
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/login');
  };

  return (
    <aside className="sidebar">
      {/* Logo */}
      <div className="sidebar-logo">
        <div className="sidebar-logo-icon">
          <Zap size={20} color="white" fill="white" />
        </div>
        <span className="sidebar-logo-text">E-Satis HUB</span>
      </div>

      <hr className="glow-divider" style={{ marginBottom: '16px' }} />

      {/* Navigation */}
      <span className="sidebar-section-label">Ana Menü</span>
      {navItems.map((item) => {
        const isActive = location.pathname.startsWith(item.path);
        const Icon = item.icon;
        return (
          <Link key={item.path} to={item.path} className={`nav-item ${isActive ? 'active' : ''}`}>
            <span className="nav-item-icon">
              <Icon size={18} />
            </span>
            <span style={{ flex: 1 }}>{item.label}</span>
            {isActive && <ChevronRight size={14} style={{ opacity: 0.5 }} />}
          </Link>
        );
      })}

      <div className="sidebar-spacer" />

      {/* User section */}
      <hr className="glow-divider" style={{ marginBottom: '16px' }} />

      <div className="sidebar-user" onClick={handleLogout} title="Çıkış yap">
        <div className="avatar">A</div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ fontSize: '0.88rem', fontWeight: 600, color: 'var(--text-primary)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
            Admin
          </div>
          <div style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>Sistem Yöneticisi</div>
        </div>
        <LogOut size={16} style={{ color: 'var(--text-muted)', flexShrink: 0 }} />
      </div>
    </aside>
  );
}
