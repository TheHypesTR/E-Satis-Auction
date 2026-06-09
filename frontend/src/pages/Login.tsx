import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Mail, Lock, ArrowRight, AlertCircle, Zap } from 'lucide-react';
import api from '../api/axios';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const response = await api.post('/Auth/login', { identifier: email, password });
      const token = response.data?.accessToken || response.data?.token;
      if (token) {
        localStorage.setItem('token', token);
        // Fetch user profile to determine role
        try {
          const meResponse = await api.get('/Auth/me', { headers: { Authorization: `Bearer ${token}` } });
          const roles = meResponse.data?.roles || [];
          if (roles.includes('GeneralAdmin') || roles.includes('WarehouseManager')) {
            navigate('/dashboard');
          } else {
            navigate('/user');
          }
        } catch {
          navigate('/user'); // Fallback
        }
      } else {
        localStorage.setItem('token', 'dev-token');
        navigate('/dashboard');
      }
    } catch {
      setError('Giriş başarısız. E-posta veya şifre hatalı.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        {/* Logo mark */}
        <div className="login-logo-mark">
          <Zap size={26} color="white" fill="white" />
        </div>

        <h1 className="login-heading">Tekrar hoş geldiniz 👋</h1>
        <p className="login-sub">Hesabınıza giriş yaparak devam edin.</p>

        {error && (
          <div className="error-banner" style={{ marginBottom: '16px' }}>
            <AlertCircle size={16} />
            {error}
          </div>
        )}

        <form className="login-form" onSubmit={handleLogin}>
          <div className="form-group">
            <label className="form-label">E-posta</label>
            <div style={{ position: 'relative' }}>
              <Mail size={15} style={{
                position: 'absolute', left: '14px', top: '50%',
                transform: 'translateY(-50%)', color: 'var(--text-muted)',
                pointerEvents: 'none'
              }} />
              <input
                type="email"
                className="form-input"
                style={{ paddingLeft: '42px' }}
                placeholder="admin@esatis.com"
                value={email}
                onChange={e => setEmail(e.target.value)}
                required
              />
            </div>
          </div>

          <div className="form-group">
            <label className="form-label">Şifre</label>
            <div style={{ position: 'relative' }}>
              <Lock size={15} style={{
                position: 'absolute', left: '14px', top: '50%',
                transform: 'translateY(-50%)', color: 'var(--text-muted)',
                pointerEvents: 'none'
              }} />
              <input
                type="password"
                className="form-input"
                style={{ paddingLeft: '42px' }}
                placeholder="••••••••••••"
                value={password}
                onChange={e => setPassword(e.target.value)}
                required
              />
            </div>
          </div>

          <button
            type="submit"
            className="btn btn-primary"
            style={{ width: '100%', padding: '13px', fontSize: '0.95rem', marginTop: '4px' }}
            disabled={loading}
          >
            {loading ? (
              <span style={{ opacity: 0.8 }}>Giriş yapılıyor...</span>
            ) : (
              <>
                Giriş Yap
                <ArrowRight size={16} />
              </>
            )}
          </button>
        </form>

        <p style={{
          marginTop: '24px',
          textAlign: 'center',
          fontSize: '0.8rem',
          color: 'var(--text-muted)',
          position: 'relative',
          zIndex: 1
        }}>
          E-Satis Auction Management Platform © 2025
        </p>
      </div>
    </div>
  );
}
