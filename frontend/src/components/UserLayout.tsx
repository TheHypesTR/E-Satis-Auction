import { Outlet } from 'react-router-dom';
import UserNavbar from './UserNavbar';

export default function UserLayout() {
  return (
    <div className="user-shell">
      <UserNavbar />
      <main className="user-main">
        <Outlet />
      </main>
      <footer className="user-footer">
        <div className="user-footer-inner">
          <span style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 600, fontSize: '0.9rem', background: 'linear-gradient(135deg, #a78bfa, #60a5fa)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent', backgroundClip: 'text' }}>
            E-Satis
          </span>
          <span style={{ color: 'var(--text-muted)', fontSize: '0.8rem' }}>
            © 2025 E-Satis Auction Platform. Tüm hakları saklıdır.
          </span>
          <div style={{ display: 'flex', gap: '20px' }}>
            {['Gizlilik', 'Kullanım Şartları', 'Yardım'].map(l => (
              <a key={l} href="#" style={{ color: 'var(--text-muted)', fontSize: '0.8rem', textDecoration: 'none' }}
                onMouseEnter={e => (e.currentTarget.style.color = 'var(--text-secondary)')}
                onMouseLeave={e => (e.currentTarget.style.color = 'var(--text-muted)')}
              >{l}</a>
            ))}
          </div>
        </div>
      </footer>
    </div>
  );
}
