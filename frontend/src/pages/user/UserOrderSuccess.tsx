import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { CheckCircle2, ShoppingBag, ArrowRight, Package } from 'lucide-react';

export default function UserOrderSuccess() {
  const navigate = useNavigate();
  const [orderId] = useState(() => `ESA-${Date.now().toString(36).toUpperCase().slice(-8)}`);

  return (
    <div className="user-page" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: 500 }}>
      <div style={{ textAlign: 'center', maxWidth: 520 }}>
        {/* Success circle */}
        <div style={{
          width: 100, height: 100, borderRadius: '50%',
          background: 'rgba(16,185,129,0.12)',
          border: '2px solid rgba(16,185,129,0.3)',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          margin: '0 auto 32px',
          boxShadow: '0 0 60px rgba(16,185,129,0.2)',
          animation: 'pulse-glow-green 3s infinite',
        }}>
          <CheckCircle2 size={48} color="#6ee7b7" />
        </div>

        <h1 style={{ fontSize: '2rem', fontWeight: 700, marginBottom: 12 }}>
          Siparişiniz Alındı! 🎉
        </h1>
        <p style={{ color: 'var(--text-secondary)', fontSize: '1rem', lineHeight: 1.7, marginBottom: 8 }}>
          Siparişiniz başarıyla oluşturuldu ve işleme alındı.<br />
          Takip bilgileri e-posta adresinize gönderilecektir.
        </p>

        {/* Order ID */}
        <div style={{
          display: 'inline-flex', alignItems: 'center', gap: 10,
          padding: '12px 24px', margin: '20px 0 32px',
          background: 'rgba(124,58,237,0.1)',
          border: '1px solid rgba(124,58,237,0.25)',
          borderRadius: 12,
        }}>
          <Package size={16} color="#a78bfa" />
          <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Sipariş No:</span>
          <code style={{ fontFamily: 'monospace', fontWeight: 700, color: '#a78bfa', fontSize: '1rem' }}>{orderId}</code>
        </div>

        {/* Delivery estimate */}
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: 12, marginBottom: 36 }}>
          {[
            { emoji: '✅', title: 'Onaylandı',    sub: 'Hemen' },
            { emoji: '📦', title: 'Paketleniyor', sub: '1-2 iş günü' },
            { emoji: '🚚', title: 'Teslimatta',   sub: '2-4 iş günü' },
          ].map((s, i) => (
            <div key={i} style={{
              padding: '16px 12px', background: 'rgba(255,255,255,0.03)',
              border: '1px solid var(--glass-border)', borderRadius: 12, textAlign: 'center',
            }}>
              <div style={{ fontSize: '1.5rem', marginBottom: 6 }}>{s.emoji}</div>
              <div style={{ fontSize: '0.85rem', fontWeight: 600, marginBottom: 2 }}>{s.title}</div>
              <div style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{s.sub}</div>
            </div>
          ))}
        </div>

        <div style={{ display: 'flex', gap: 12, justifyContent: 'center', flexWrap: 'wrap' }}>
          <button className="btn btn-primary" style={{ gap: 6 }} onClick={() => navigate('/user/catalog')}>
            <ShoppingBag size={16} /> Alışverişe Devam
          </button>
          <button className="btn btn-ghost" style={{ gap: 6 }} onClick={() => navigate('/user/profile')}>
            Siparişlerim <ArrowRight size={15} />
          </button>
        </div>
      </div>
    </div>
  );
}
