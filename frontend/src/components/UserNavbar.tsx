import { Link, useLocation, useNavigate } from 'react-router-dom';
import {
  ShoppingCart,
  Home,
  LayoutGrid,
  User,
  Zap,
  Search,
  Menu,
  X,
} from 'lucide-react';
import { useState } from 'react';
import { useCart } from '../context/CartContext';

const navLinks = [
  { label: 'Ana Sayfa',  path: '/user',         icon: Home },
  { label: 'Katalog',    path: '/user/catalog',  icon: LayoutGrid },
  { label: 'Profilim',   path: '/user/profile',  icon: User },
];

export default function UserNavbar() {
  const location = useLocation();
  const navigate  = useNavigate();
  const { totalItems } = useCart();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [search, setSearch] = useState('');

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/login');
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (search.trim()) {
      navigate(`/user/catalog?q=${encodeURIComponent(search.trim())}`);
      setSearch('');
    }
  };

  return (
    <>
      <nav className="user-navbar">
        {/* Logo */}
        <Link to="/user" className="user-navbar-logo">
          <div className="sidebar-logo-icon" style={{ width: 34, height: 34, borderRadius: 10 }}>
            <Zap size={16} color="white" fill="white" />
          </div>
          <span className="sidebar-logo-text">E-Satis</span>
        </Link>

        {/* Desktop Nav Links */}
        <div className="user-navbar-links">
          {navLinks.map(({ label, path, icon: Icon }) => {
            const active = location.pathname === path || (path !== '/user' && location.pathname.startsWith(path));
            return (
              <Link
                key={path}
                to={path}
                className={`user-nav-link ${active ? 'active' : ''}`}
              >
                <Icon size={15} />
                {label}
              </Link>
            );
          })}
        </div>

        {/* Search */}
        <form className="user-navbar-search" onSubmit={handleSearch}>
          <Search size={14} style={{ color: 'var(--text-muted)', flexShrink: 0 }} />
          <input
            type="text"
            placeholder="Ürün ara..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </form>

        {/* Right actions */}
        <div className="user-navbar-actions">
          {/* Cart */}
          <Link to="/user/cart" className="user-cart-btn">
            <ShoppingCart size={18} />
            {totalItems > 0 && (
              <span className="user-cart-badge">{totalItems > 99 ? '99+' : totalItems}</span>
            )}
          </Link>

          {/* Profile / Logout */}
          <button className="user-avatar-btn" onClick={handleLogout} title="Çıkış">
            <div className="avatar" style={{ width: 32, height: 32, fontSize: '0.78rem', borderRadius: 8 }}>K</div>
          </button>

          {/* Mobile toggle */}
          <button
            className="btn btn-ghost user-mobile-menu-btn"
            onClick={() => setMobileOpen(v => !v)}
          >
            {mobileOpen ? <X size={18} /> : <Menu size={18} />}
          </button>
        </div>
      </nav>

      {/* Mobile drawer */}
      {mobileOpen && (
        <div className="user-mobile-drawer">
          {navLinks.map(({ label, path, icon: Icon }) => (
            <Link
              key={path}
              to={path}
              className="user-nav-link"
              onClick={() => setMobileOpen(false)}
            >
              <Icon size={16} />
              {label}
            </Link>
          ))}
          <Link to="/user/cart" className="user-nav-link" onClick={() => setMobileOpen(false)}>
            <ShoppingCart size={16} />
            Sepet {totalItems > 0 && `(${totalItems})`}
          </Link>
        </div>
      )}
    </>
  );
}
