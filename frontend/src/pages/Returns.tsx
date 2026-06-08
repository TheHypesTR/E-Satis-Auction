import { useState, useEffect } from 'react';
import { Search, Filter, RefreshCcw, CheckCircle, XCircle, X } from 'lucide-react';
import api from '../api/axios';

interface ReturnRequest {
  id: string;
  orderNumber: string;
  userDisplayName: string;
  reason: string;
  createdAt: string;
  status: string | number;
}

const statusEnumMap: Record<number, string> = {
  1: 'Pending',
  2: 'Approved',
  3: 'Rejected',
  4: 'Received',
  5: 'Cancelled'
};

const statusColors: Record<string, string> = {
  'Pending': 'amber',
  'Approved': 'green',
  'Rejected': 'red',
  'Received': 'blue',
  'Cancelled': 'gray'
};

const statusTranslations: Record<string, string> = {
  'Pending': 'Bekliyor',
  'Approved': 'Onaylandı',
  'Rejected': 'Reddedildi',
  'Received': 'Teslim Alındı',
  'Cancelled': 'İptal Edildi'
};

export default function Returns() {
  const [returns, setReturns] = useState<ReturnRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedReturn, setSelectedReturn] = useState<ReturnRequest | null>(null);

  const fetchReturns = () => {
    setLoading(true);
    api.get('/AdminReturnRequest')
      .then(res => {
        const data = res.data?.items || res.data?.data || [];
        setReturns(data);
        setLoading(false);
      })
      .catch(() => {
        setReturns([]);
        setLoading(false);
      });
  };

  useEffect(() => {
    fetchReturns();
  }, []);

  const filtered = returns.filter(r =>
    r.orderNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (r.userDisplayName && r.userDisplayName.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const handleUpdateStatus = (id: string, action: 'approve' | 'reject' | 'receive') => {
    api.put(`/AdminReturnRequest/${id}/${action}`, {})
      .then(() => {
        // Refresh list
        fetchReturns();
        setSelectedReturn(null);
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
          <h1 className="page-title">İade Talepleri</h1>
          <p className="page-subtitle">{returns.length} iade talebi listeleniyor</p>
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
             <RefreshCcw size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
             <p>Yükleniyor...</p>
           </div>
        ) : filtered.length === 0 ? (
           <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}>
             <RefreshCcw size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
             <p>İade talebi bulunamadı.</p>
           </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Sipariş No</th>
                <th>Müşteri</th>
                <th>Tarih</th>
                <th>Sebep</th>
                <th>Durum</th>
                <th style={{ textAlign: 'right' }}>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((r) => {
                const statusStr = typeof r.status === 'number' ? statusEnumMap[r.status] : r.status;
                const colorKey = statusColors[statusStr] || 'purple';
                const statusName = statusTranslations[statusStr] || statusStr;
                return (
                  <tr key={r.id}>
                    <td>
                      <code style={{ fontFamily: 'monospace', fontSize: '0.82rem', background: 'rgba(255,255,255,0.05)', padding: '3px 8px', borderRadius: '5px', color: '#a78bfa' }}>
                        {r.orderNumber}
                      </code>
                    </td>
                    <td>
                      <div style={{ fontWeight: 500, color: 'var(--text-primary)', fontSize: '0.92rem' }}>
                        {r.userDisplayName}
                      </div>
                    </td>
                    <td>
                      <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>{new Date(r.createdAt).toLocaleDateString('tr-TR')}</span>
                    </td>
                    <td>
                      <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>{r.reason}</span>
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
                        onClick={() => setSelectedReturn(r)}
                      >
                        İncele
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}
      </div>

      {/* Return Management Modal */}
      {selectedReturn && (() => {
        const statusStr = typeof selectedReturn.status === 'number' ? statusEnumMap[selectedReturn.status] : selectedReturn.status;
        return (
        <div className="modal-overlay" onClick={() => setSelectedReturn(null)}>
          <div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 500, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.2rem', fontWeight: 600 }}>İade Talebi İnceleme</h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setSelectedReturn(null)}>
                <X size={18} />
              </button>
            </div>

            <div style={{ marginBottom: 24 }}>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 8 }}>Sipariş No: <strong style={{ color: 'var(--text-primary)' }}>{selectedReturn.orderNumber}</strong></p>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 8 }}>Müşteri: <strong style={{ color: 'var(--text-primary)' }}>{selectedReturn.userDisplayName}</strong></p>
              <div style={{ padding: '12px', background: 'rgba(255,255,255,0.03)', borderRadius: 8, border: '1px solid var(--glass-border)', margin: '12px 0' }}>
                <p style={{ color: 'var(--text-muted)', fontSize: '0.8rem', marginBottom: 4 }}>İade Sebebi:</p>
                <p style={{ fontSize: '0.9rem', color: 'var(--text-primary)' }}>{selectedReturn.reason}</p>
              </div>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 24 }}>Mevcut Durum: <span className={`badge badge-${statusColors[statusStr]}`}>{statusTranslations[statusStr] || statusStr}</span></p>

              <div className="glow-divider" style={{ marginBottom: 20 }} />

              <h3 style={{ fontSize: '0.95rem', fontWeight: 600, marginBottom: 12 }}>İşlem Yap</h3>
              <div style={{ display: 'flex', gap: 12 }}>
                <button 
                  className={`btn ${statusStr === 'Approved' ? 'btn-primary' : 'btn-ghost'}`}
                  style={{ flex: 1, gap: 8, padding: '10px' }}
                  onClick={() => handleUpdateStatus(selectedReturn.id, 'approve')}
                >
                  <CheckCircle size={16} /> Onayla
                </button>
                <button 
                  className={`btn ${statusStr === 'Rejected' ? 'btn-primary' : 'btn-ghost'}`}
                  style={{ flex: 1, gap: 8, padding: '10px', color: statusStr !== 'Rejected' ? '#f87171' : undefined }}
                  onClick={() => handleUpdateStatus(selectedReturn.id, 'reject')}
                >
                  <XCircle size={16} /> Reddet
                </button>
              </div>
            </div>
            
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
               <button className="btn btn-ghost" onClick={() => setSelectedReturn(null)}>Kapat</button>
            </div>
          </div>
        </div>
        );
      })()}
    </div>
  );
}
