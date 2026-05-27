import { Outlet, useLocation } from 'react-router-dom';
import Sidebar from './Sidebar';
import { Bell, Search } from 'lucide-react';

const routeLabels: Record<string, string> = {
  '/dashboard':  'Dashboard',
  '/products':   'Ürünler',
  '/facilities': 'Tesisler',
  '/categories': 'Kategoriler',
  '/inventory':  'Envanter',
};

export default function Layout() {
  const location = useLocation();
  const currentLabel = routeLabels[location.pathname] ?? 'Sayfa';

  return (
    <div className="shell">
      <Sidebar />

      <div className="content-area">
        {/* Top Bar */}
        <header className="topbar">
          <div className="breadcrumb">
            <span>Ana Menü</span>
            <span style={{ opacity: 0.4 }}>/</span>
            <span className="breadcrumb-current">{currentLabel}</span>
          </div>

          <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
            {/* Search */}
            <div className="search-bar">
              <Search size={15} style={{ color: 'var(--text-muted)', flexShrink: 0 }} />
              <input type="text" placeholder="Hızlı arama..." />
            </div>

            {/* Notifications */}
            <button
              className="btn btn-ghost"
              style={{ width: '40px', height: '40px', padding: 0, position: 'relative' }}
              title="Bildirimler"
            >
              <Bell size={17} />
              <span style={{
                position: 'absolute',
                top: '7px',
                right: '7px',
                width: '7px',
                height: '7px',
                borderRadius: '50%',
                background: '#ec4899',
                boxShadow: '0 0 8px #ec4899',
              }} />
            </button>
          </div>
        </header>

        {/* Page */}
        <main className="page">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
