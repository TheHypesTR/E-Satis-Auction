import { useState } from 'react';
import { Search, Filter, RefreshCcw, CheckCircle, XCircle, X } from 'lucide-react';

interface ReturnRequest {
  id: string;
  orderNumber: string;
  customerName: string;
  reason: string;
  date: string;
  status: 'Bekliyor' | 'Onaylandı' | 'Reddedildi';
}

const MOCK_RETURNS: ReturnRequest[] = [
  { id: '1', orderNumber: 'ESA-A4F2B1', customerName: 'Ali Yılmaz', reason: 'Ürün hasarlı geldi', date: '2026-05-28', status: 'Bekliyor' },
  { id: '2', orderNumber: 'ESA-C8D3E0', customerName: 'Ayşe Demir', reason: 'Beklediğim gibi çıkmadı', date: '2026-05-22', status: 'Onaylandı' },
  { id: '3', orderNumber: 'ESA-F1A9B7', customerName: 'Mehmet Kaya', reason: 'Yanlış ürün gönderilmiş', date: '2026-05-11', status: 'Reddedildi' },
];

const statusColors: Record<string, string> = {
  'Bekliyor': 'amber',
  'Onaylandı': 'green',
  'Reddedildi': 'red',
};

export default function Returns() {
  const [returns, setReturns] = useState<ReturnRequest[]>(MOCK_RETURNS);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedReturn, setSelectedReturn] = useState<ReturnRequest | null>(null);

  const filtered = returns.filter(r =>
    r.orderNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
    r.customerName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleUpdateStatus = (id: string, newStatus: ReturnRequest['status']) => {
    setReturns(prev => prev.map(r => r.id === id ? { ...r, status: newStatus } : r));
    if (selectedReturn && selectedReturn.id === id) {
      setSelectedReturn({ ...selectedReturn, status: newStatus });
    }
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

        {filtered.length === 0 ? (
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
                const colorKey = statusColors[r.status] || 'purple';
                return (
                  <tr key={r.id}>
                    <td>
                      <code style={{ fontFamily: 'monospace', fontSize: '0.82rem', background: 'rgba(255,255,255,0.05)', padding: '3px 8px', borderRadius: '5px', color: '#a78bfa' }}>
                        {r.orderNumber}
                      </code>
                    </td>
                    <td>
                      <div style={{ fontWeight: 500, color: 'var(--text-primary)', fontSize: '0.92rem' }}>
                        {r.customerName}
                      </div>
                    </td>
                    <td>
                      <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>{r.date}</span>
                    </td>
                    <td>
                      <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>{r.reason}</span>
                    </td>
                    <td>
                      <span className={`badge badge-${colorKey}`}>
                        {r.status}
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
      {selectedReturn && (
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
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 8 }}>Müşteri: <strong style={{ color: 'var(--text-primary)' }}>{selectedReturn.customerName}</strong></p>
              <div style={{ padding: '12px', background: 'rgba(255,255,255,0.03)', borderRadius: 8, border: '1px solid var(--glass-border)', margin: '12px 0' }}>
                <p style={{ color: 'var(--text-muted)', fontSize: '0.8rem', marginBottom: 4 }}>İade Sebebi:</p>
                <p style={{ fontSize: '0.9rem', color: 'var(--text-primary)' }}>{selectedReturn.reason}</p>
              </div>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 24 }}>Mevcut Durum: <span className={`badge badge-${statusColors[selectedReturn.status]}`}>{selectedReturn.status}</span></p>

              <div className="glow-divider" style={{ marginBottom: 20 }} />

              <h3 style={{ fontSize: '0.95rem', fontWeight: 600, marginBottom: 12 }}>İşlem Yap</h3>
              <div style={{ display: 'flex', gap: 12 }}>
                <button 
                  className={`btn ${selectedReturn.status === 'Onaylandı' ? 'btn-primary' : 'btn-ghost'}`}
                  style={{ flex: 1, gap: 8, padding: '10px' }}
                  onClick={() => handleUpdateStatus(selectedReturn.id, 'Onaylandı')}
                >
                  <CheckCircle size={16} /> Onayla
                </button>
                <button 
                  className={`btn ${selectedReturn.status === 'Reddedildi' ? 'btn-primary' : 'btn-ghost'}`}
                  style={{ flex: 1, gap: 8, padding: '10px', color: selectedReturn.status !== 'Reddedildi' ? '#f87171' : undefined }}
                  onClick={() => handleUpdateStatus(selectedReturn.id, 'Reddedildi')}
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
      )}
    </div>
  );
}
