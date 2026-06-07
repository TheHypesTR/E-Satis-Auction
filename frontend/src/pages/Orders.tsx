import { useState } from 'react';
import { Search, Filter, MoreVertical, Package, CheckCircle, Truck, X } from 'lucide-react';

interface Order {
  id: string;
  orderNumber: string;
  customerName: string;
  totalAmount: number;
  date: string;
  status: 'Bekliyor' | 'Onaylandı' | 'Kargolandı' | 'İptal';
}

const MOCK_ORDERS: Order[] = [
  { id: '1', orderNumber: 'ESA-A4F2B1', customerName: 'Ali Yılmaz', totalAmount: 2340, date: '2026-05-27', status: 'Bekliyor' },
  { id: '2', orderNumber: 'ESA-C8D3E0', customerName: 'Ayşe Demir', totalAmount: 890, date: '2026-05-21', status: 'Onaylandı' },
  { id: '3', orderNumber: 'ESA-F1A9B7', customerName: 'Mehmet Kaya', totalAmount: 5120, date: '2026-05-10', status: 'Kargolandı' },
];

const statusColors: Record<string, string> = {
  'Bekliyor': 'amber',
  'Onaylandı': 'blue',
  'Kargolandı': 'green',
  'İptal': 'red',
};

export default function Orders() {
  const [orders, setOrders] = useState<Order[]>(MOCK_ORDERS);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);

  const filtered = orders.filter(o =>
    o.orderNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
    o.customerName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleUpdateStatus = (id: string, newStatus: Order['status']) => {
    setOrders(prev => prev.map(o => o.id === id ? { ...o, status: newStatus } : o));
    if (selectedOrder && selectedOrder.id === id) {
      setSelectedOrder({ ...selectedOrder, status: newStatus });
    }
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

        {filtered.length === 0 ? (
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
                return (
                  <tr key={o.id}>
                    <td>
                      <code style={{ fontFamily: 'monospace', fontSize: '0.82rem', background: 'rgba(255,255,255,0.05)', padding: '3px 8px', borderRadius: '5px', color: '#a78bfa' }}>
                        {o.orderNumber}
                      </code>
                    </td>
                    <td>
                      <div style={{ fontWeight: 500, color: 'var(--text-primary)', fontSize: '0.92rem' }}>
                        {o.customerName}
                      </div>
                    </td>
                    <td>
                      <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>{o.date}</span>
                    </td>
                    <td>
                      <span style={{ fontWeight: 600 }}>₺{o.totalAmount.toLocaleString('tr-TR')}</span>
                    </td>
                    <td>
                      <span className={`badge badge-${colorKey}`}>
                        {o.status}
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
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 8 }}>Müşteri: <strong style={{ color: 'var(--text-primary)' }}>{selectedOrder.customerName}</strong></p>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 8 }}>Tutar: <strong style={{ color: 'var(--text-primary)' }}>₺{selectedOrder.totalAmount.toLocaleString('tr-TR')}</strong></p>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 24 }}>Mevcut Durum: <span className={`badge badge-${statusColors[selectedOrder.status]}`}>{selectedOrder.status}</span></p>

              <div className="glow-divider" style={{ marginBottom: 20 }} />

              <h3 style={{ fontSize: '0.95rem', fontWeight: 600, marginBottom: 12 }}>Durum Güncelle</h3>
              <div style={{ display: 'flex', gap: 12 }}>
                <button 
                  className={`btn ${selectedOrder.status === 'Onaylandı' ? 'btn-primary' : 'btn-ghost'}`}
                  style={{ flex: 1, gap: 8, padding: '10px' }}
                  onClick={() => handleUpdateStatus(selectedOrder.id, 'Onaylandı')}
                >
                  <CheckCircle size={16} /> Onaylandı
                </button>
                <button 
                  className={`btn ${selectedOrder.status === 'Kargolandı' ? 'btn-primary' : 'btn-ghost'}`}
                  style={{ flex: 1, gap: 8, padding: '10px' }}
                  onClick={() => handleUpdateStatus(selectedOrder.id, 'Kargolandı')}
                >
                  <Truck size={16} /> Kargolandı
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
