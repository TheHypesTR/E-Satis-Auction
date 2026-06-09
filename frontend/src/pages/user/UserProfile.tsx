/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AlertCircle, CheckCircle2, Clock, LogOut, Package, RefreshCcw, ShoppingBag, X } from 'lucide-react';
import { commerceApi } from '../../services/commerceApi';
import { formatMoney, getApiErrorMessage, orderStatusLabel } from '../../services/apiUtils';
import type { OrderDetailDto, OrderSummaryDto } from '../../types/commerce';

const returnableStatuses = new Set(['Shipped', 'Delivered', '4', '5']);

export default function UserProfile() {
  const navigate = useNavigate();
  const [orders, setOrders] = useState<OrderSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [returnOrder, setReturnOrder] = useState<OrderDetailDto | null>(null);
  const [returnReason, setReturnReason] = useState('');
  const [submittingReturn, setSubmittingReturn] = useState(false);

  const loadOrders = () => {
    setLoading(true); setError('');
    commerceApi.getOrders({ pageSize: 50 })
      .then(setOrders)
      .catch(err => setError(getApiErrorMessage(err, 'Siparişler yüklenemedi.')))
      .finally(() => setLoading(false));
  };

  useEffect(loadOrders, []);

  const handleLogout = () => { localStorage.removeItem('token'); navigate('/login'); };

  const openReturn = async (orderId: string) => {
    setError(''); setMessage('');
    try {
      const detail = await commerceApi.getOrder(orderId);
      setReturnOrder(detail);
      setReturnReason('');
    } catch (err) {
      setError(getApiErrorMessage(err, 'Sipariş detayı alınamadı.'));
    }
  };

  const createReturn = async () => {
    if (!returnOrder || !returnReason.trim()) return;
    const lines = returnOrder.lines.map(line => ({ purchaseOrderLineId: line.id, quantity: line.quantity, reason: returnReason.trim() }));
    setSubmittingReturn(true); setError(''); setMessage('');
    try {
      await commerceApi.createReturnRequest(returnOrder.id, returnReason.trim(), lines);
      setReturnOrder(null);
      setMessage('İade talebiniz oluşturuldu. Ürün fiziksel olarak teslim alınıp kontrol edildikten sonra süreç tamamlanır.');
      loadOrders();
    } catch (err) {
      setError(getApiErrorMessage(err, 'İade talebi oluşturulamadı.'));
    } finally {
      setSubmittingReturn(false);
    }
  };

  return (
    <div className="user-page">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 32 }}>
        <div><div className="user-section-label">Hesabım</div><h1 style={{ fontSize: '1.9rem', fontWeight: 700 }}>Siparişlerim</h1><p style={{ color: 'var(--text-muted)', marginTop: 4, fontSize: '0.9rem' }}>Sipariş durumu, admin onayı, kargo ve iade talebi akışları.</p></div>
        <button className="btn btn-ghost" style={{ color: '#f87171' }} onClick={handleLogout}><LogOut size={16} /> Çıkış Yap</button>
      </div>
      {error && <div className="error-banner" style={{ marginBottom: 16 }}><AlertCircle size={15} /> {error}</div>}
      {message && <div style={{ marginBottom: 16, padding: '12px 16px', color: '#6ee7b7', background: 'rgba(16,185,129,0.08)', borderRadius: 10, border: '1px solid rgba(16,185,129,0.15)' }}><CheckCircle2 size={15} /> {message}</div>}
      <div className="glass-card" style={{ padding: 24 }}>
        {loading ? <Empty text="Siparişler yükleniyor..." /> : orders.length === 0 ? <div style={{ textAlign: 'center', padding: '60px 0', color: 'var(--text-muted)' }}><ShoppingBag size={40} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>Henüz siparişiniz bulunmuyor.</p><button className="btn btn-primary" style={{ marginTop: 16 }} onClick={() => navigate('/user/catalog')}>Alışverişe Başla</button></div> : <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>{orders.map(order => <div key={order.id} className="glass-card" style={{ display: 'flex', alignItems: 'center', gap: 16, padding: '20px 24px' }}><div style={{ width: 44, height: 44, borderRadius: 12, background: 'rgba(124,58,237,0.12)', border: '1px solid rgba(124,58,237,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}><Package size={20} color="#a78bfa" /></div><div style={{ flex: 1, minWidth: 0 }}><div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 4, flexWrap: 'wrap' }}><code style={{ fontWeight: 700, color: '#a78bfa', fontSize: '0.9rem' }}>{order.orderNumber}</code><span className="badge badge-purple">{orderStatusLabel[String(order.status)] ?? String(order.status)}</span></div><div style={{ fontSize: '0.82rem', color: 'var(--text-muted)', display: 'flex', alignItems: 'center', gap: 6 }}><Clock size={12} /> {new Date(order.createdAt).toLocaleDateString('tr-TR')} · Kaynak: {String(order.orderSource)}</div></div><div style={{ textAlign: 'right' }}><div style={{ fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700, fontSize: '1rem', color: 'var(--text-primary)' }}>{formatMoney(order.totalAmount, order.currency)}</div><div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end', marginTop: 6 }}><button className="btn btn-ghost" style={{ padding: '5px 12px', fontSize: '0.78rem' }} onClick={() => openReturn(order.id)}>Detay</button>{returnableStatuses.has(String(order.status)) && <button className="btn btn-ghost" style={{ padding: '5px 12px', fontSize: '0.78rem', color: '#fcd34d' }} onClick={() => openReturn(order.id)}><RefreshCcw size={12} /> İade Talebi</button>}</div></div></div>)}</div>}
      </div>
      {returnOrder && <div className="modal-overlay" onClick={() => setReturnOrder(null)}><div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 560, width: '100%' }}><div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}><h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8 }}><RefreshCcw size={20} color="#fcd34d" /> Sipariş Detayı / İade</h2><button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setReturnOrder(null)}><X size={18} /></button></div><div style={{ marginBottom: 18, color: 'var(--text-muted)', fontSize: '0.9rem' }}>Sipariş: <code>{returnOrder.orderNumber}</code><br />Durum: {orderStatusLabel[String(returnOrder.status)] ?? String(returnOrder.status)}<br />Toplam: {formatMoney(returnOrder.totalAmount, returnOrder.currency)}</div><div style={{ marginBottom: 18 }}>{returnOrder.lines.map(line => <div key={line.id} style={{ display: 'flex', justifyContent: 'space-between', padding: '10px 0', borderBottom: '1px solid var(--glass-border)' }}><span>{line.productName} x {line.quantity}</span><strong>{formatMoney(line.discountedUnitPrice * line.quantity, line.currency)}</strong></div>)}</div>{returnableStatuses.has(String(returnOrder.status)) ? <><div className="form-group"><label className="form-label">İade Nedeni</label><textarea className="form-input" rows={4} style={{ resize: 'vertical' }} value={returnReason} onChange={e => setReturnReason(e.target.value)} /></div><p style={{ color: 'var(--text-muted)', fontSize: '0.8rem', marginBottom: 16 }}>İade onayı stok artırmaz; fiziksel teslim alma/restock admin operasyonudur.</p><div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}><button className="btn btn-ghost" onClick={() => setReturnOrder(null)}>Kapat</button><button className="btn btn-primary" onClick={createReturn} disabled={submittingReturn || !returnReason.trim()}>{submittingReturn ? 'Gönderiliyor...' : 'İade Talebi Oluştur'}</button></div></> : <p style={{ color: 'var(--text-muted)' }}>Bu sipariş için iade talebi yalnızca kargoya verildi/teslim edildi durumlarında açılabilir.</p>}</div></div>}
    </div>
  );
}

function Empty({ text }: { text: string }) { return <div style={{ textAlign: 'center', padding: '60px 0', color: 'var(--text-muted)' }}><Package size={40} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>; }

