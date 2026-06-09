import { useLocation, useNavigate } from 'react-router-dom';
import { CheckCircle2, Clock, Package } from 'lucide-react';
import type { OrderDetailDto, PaymentAttemptDto } from '../../types/commerce';
import { formatMoney, orderStatusLabel } from '../../services/apiUtils';

interface SuccessState { order?: OrderDetailDto; payment?: PaymentAttemptDto; mode?: 'buyNow' | 'paymentConfirm' | 'auctionPaymentConfirm'; }

export default function UserOrderSuccess() {
  const navigate = useNavigate();
  const { state } = useLocation();
  const data = (state ?? {}) as SuccessState;
  const order = data.order;
  const isPayment = data.mode === 'paymentConfirm' || data.mode === 'auctionPaymentConfirm';
  const title = isPayment ? 'Ödeme Simülasyonu Başarılı' : 'Siparişiniz Oluşturuldu';
  const message = isPayment ? 'Ödeme simülasyonu başarılı. Sipariş admin onayına gönderildi.' : 'Siparişiniz oluşturuldu ve admin onayına gönderildi.';

  return <div className="user-page" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: 500 }}><div className="glass-card animate-fade-up" style={{ padding: '48px 40px', textAlign: 'center', maxWidth: 620, width: '100%' }}><div style={{ width: 100, height: 100, borderRadius: '50%', background: 'rgba(16,185,129,0.12)', border: '2px solid rgba(16,185,129,0.3)', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 28px' }}><CheckCircle2 size={52} color="#6ee7b7" /></div><h1 style={{ fontSize: '1.8rem', fontWeight: 800, marginBottom: 12 }}>{title}</h1><p style={{ color: 'var(--text-secondary)', fontSize: '0.95rem', lineHeight: 1.7, marginBottom: 28 }}>{message}</p>{order ? <div style={{ background: 'rgba(124,58,237,0.08)', border: '1px solid rgba(124,58,237,0.25)', borderRadius: 12, padding: '18px 20px', marginBottom: 28, textAlign: 'left' }}><div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 10 }}><Package size={16} color="#a78bfa" /><strong>Sipariş No: <code style={{ color: '#a78bfa' }}>{order.orderNumber}</code></strong></div><div style={{ color: 'var(--text-muted)', fontSize: '0.88rem' }}>Durum: {orderStatusLabel[String(order.status)] ?? String(order.status)}<br />Toplam: {formatMoney(order.totalAmount, order.currency)}</div>{data.payment && <div style={{ color: 'var(--text-muted)', fontSize: '0.88rem', marginTop: 10 }}><Clock size={13} /> PaymentAttempt: <code>{data.payment.id}</code></div>}</div> : <div style={{ background: 'rgba(255,255,255,0.03)', border: '1px solid var(--glass-border)', borderRadius: 12, padding: '18px 20px', marginBottom: 28, color: 'var(--text-muted)' }}>Sipariş detayına profil sayfasından ulaşabilirsiniz.</div>}<div style={{ display: 'flex', gap: 12, justifyContent: 'center', flexWrap: 'wrap' }}><button className="btn btn-primary" onClick={() => navigate('/user/profile')}>Siparişlerime Git</button><button className="btn btn-ghost" onClick={() => navigate('/user/catalog')}>Alışverişe Devam</button></div><p style={{ marginTop: 22, color: 'var(--text-muted)', fontSize: '0.8rem' }}>Admin onayı kargoyu otomatik başlatmaz; sevkiyat ayrı admin aksiyonu ile yapılır.</p></div></div>;
}
