import { useState, useEffect } from 'react';
import { Search, Filter, Package, CheckCircle, Truck, X } from 'lucide-react';
import api from '../api/axios';

interface Order {
  id: string;
  orderNumber: string;
  userDisplayName: string;
  totalAmount: number;
  createdAt: string;
  status: string;
}

const statusColors: Record<string, string> = {
  'PendingApproval': 'amber',
  'Approved': 'blue',
  'Shipped': 'green',
  'Cancelled': 'red',
  'Rejected': 'red',
  'Delivered': 'green',
  'PaymentPending': 'amber'
};

const statusTranslations: Record<string, string> = {
  'PendingApproval': 'Bekliyor',
  'Approved': 'Onaylandı',
  'Shipped': 'Kargolandı',
  'Cancelled': 'İptal',
  'Rejected': 'Reddedildi',
  'Delivered': 'Teslim Edildi',
  'PaymentPending': 'Ödeme Bekleniyor'
};

export default function Orders() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);

  const fetchOrders = () => {
    setLoading(true);
    api.get('/AdminPurchaseOrder')
      .then(res => {
        const data = res.data?.items || res.data?.data || [];
        setOrders(data);
        setLoading(false);
      })
      .catch(() => {
        setOrders([]);
        setLoading(false);
      });
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  const filtered = orders.filter(o =>
    o.orderNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (o.userDisplayName && o.userDisplayName.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const handleUpdateStatus = (id: string, action: 'approve' | 'ship') => {
    api.put(`/AdminPurchaseOrder/${id}/${action}`, {})
      .then(() => {
        fetchOrders();
        setSelectedOrder(null);
      })
      .catch(err => {
        console.error('Status update failed', err);
      });
  };

  return (
    <div>
      {/* Page header */}
      <div className="page-header">
        <div>
          <h1 className="page-title">Siparişler</h1>
          <p className="page-subtitle">{orders.length} sipariş listeleniyor</p>
        </div>
      </div>

      {/* Table */}
      <div className="data-table-wrapper animate-fade-up">
        {/* Table toolbar */}
        <div className="data-table-header">
          <div className="search-bar" style={{ minWidth: 300 }}>
            <Search size={15} style={{ color: 'var(--text-muted)', flexShrink: 0 }} />
            <input
              type="text"
              placeholder="Sipariş no veya müşteri adı ara..."
              value={searchTerm}
              onChange={e => setSearchTerm(e.target.value)}
            />
          </div>
          <div style={{ display: 'flex', gap: '10px' }}>
            <button className="btn btn-ghost" style={{ padding: '9px 14px', gap: '6px', fontSize: '0.85rem' }}>
              <Filter size={14} />
              Filtrele
            </button>
          </div>
        </div>

        {loading ? (
           <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}>
             <Package size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
             <p>Yükleniyor...</p>
           </div>
        ) : filtered.length === 0 ? (
           <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}>
             <Package size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
             <p>Sipariş bulunamadı.</p>
           </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Sipariş No</th>
                <th>Müşteri</th>
                <th>Tarih</th>
                <th>Tutar</th>
                <th>Durum</th>
                <th style={{ textAlign: 'right' }}>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((o) => {
                const colorKey = statusColors[o.status] || 'purple';
                const statusName = statusTranslations[o.status] || o.status;
                return (
                  <tr key={o.id}>
                    <td>
                      <code style={{ fontFamily: 'monospace', fontSize: '0.82rem', background: 'rgba(255,255,255,0.05)', padding: '3px 8px', borderRadius: '5px', color: '#a78bfa' }}>
                        {o.orderNumber}
                      </code>
                    </td>
                    <td>
                      <div style={{ fontWeight: 500, color: 'var(--text-primary)', fontSize: '0.92rem' }}>
                        {o.userDisplayName}
                      </div>
                    </td>
                    <td>
                      <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>{new Date(o.createdAt).toLocaleDateString('tr-TR')}</span>
                    </td>
                    <td>
                      <span style={{ fontWeight: 600 }}>₺{o.totalAmount.toLocaleString('tr-TR')}</span>
                    </td>
                    <td>
                      <span className={`badge badge-${colorKey}`}>
                        {statusName}
                      </span>
                    </td>
                    <td style={{ textAlign: 'right' }}>
                      <button
                        className="btn btn-ghost"
                        style={{ padding: '7px 14px', fontSize: '0.8rem' }}
                        onClick={() => setSelectedOrder(o)}
                      >
                        Yönet
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}
      </div>

      {/* Order Management Modal */}
      {selectedOrder && (
        <div className="modal-overlay" onClick={() => setSelectedOrder(null)}>
          <div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 500, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.2rem', fontWeight: 600 }}>Sipariş Yönetimi</h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setSelectedOrder(null)}>
                <X size={18} />
              </button>
            </div>

            <div style={{ marginBottom: 24 }}>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 8 }}>Sipariş No: <strong style={{ color: 'var(--text-primary)' }}>{selectedOrder.orderNumber}</strong></p>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 8 }}>Müşteri: <strong style={{ color: 'var(--text-primary)' }}>{selectedOrder.userDisplayName}</strong></p>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 8 }}>Tutar: <strong style={{ color: 'var(--text-primary)' }}>₺{selectedOrder.totalAmount.toLocaleString('tr-TR')}</strong></p>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 24 }}>Mevcut Durum: <span className={`badge badge-${statusColors[selectedOrder.status]}`}>{statusTranslations[selectedOrder.status] || selectedOrder.status}</span></p>

              <div className="glow-divider" style={{ marginBottom: 20 }} />

              <h3 style={{ fontSize: '0.95rem', fontWeight: 600, marginBottom: 12 }}>Durum Güncelle</h3>
              <div style={{ display: 'flex', gap: 12 }}>
                <button 
                  className={`btn ${selectedOrder.status === 'Approved' ? 'btn-primary' : 'btn-ghost'}`}
                  style={{ flex: 1, gap: 8, padding: '10px' }}
                  onClick={() => handleUpdateStatus(selectedOrder.id, 'approve')}
                >
                  <CheckCircle size={16} /> Onayla
                </button>
                <button 
                  className={`btn ${selectedOrder.status === 'Shipped' ? 'btn-primary' : 'btn-ghost'}`}
                  style={{ flex: 1, gap: 8, padding: '10px' }}
                  onClick={() => handleUpdateStatus(selectedOrder.id, 'ship')}
                >
                  <Truck size={16} /> Kargola
                </button>
              </div>
            </div>
            
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
               <button className="btn btn-ghost" onClick={() => setSelectedOrder(null)}>Kapat</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
