import { useEffect, useState } from 'react';
import { Layers, Search, TrendingUp, TrendingDown, ArrowRightLeft, Filter } from 'lucide-react';
import api from '../api/axios';

// Matches backend InventoryTransactionType enum
const TxType: Record<number, { label: string; badge: string; icon: React.ReactNode; dir: 'in' | 'out' | 'neutral' }> = {
  0: { label: 'Giriş',        badge: 'badge-green',  icon: <TrendingUp  size={14} />, dir: 'in'      },
  1: { label: 'Çıkış',        badge: 'badge-amber',  icon: <TrendingDown size={14} />, dir: 'out'    },
  2: { label: 'Transfer',     badge: 'badge-blue',   icon: <ArrowRightLeft size={14} />, dir: 'neutral' },
  3: { label: 'Düzeltme +',   badge: 'badge-green',  icon: <TrendingUp  size={14} />, dir: 'in'      },
  4: { label: 'Düzeltme -',   badge: 'badge-amber',  icon: <TrendingDown size={14} />, dir: 'out'     },
};

interface InventoryTransaction {
  id: string;
  itemName: string;
  unitOfMeasure: number;
  facilityName: string;
  transactionType: number;
  quantityChange: number;
  previousQuantity: number;
  newQuantity: number;
  referenceTrackingNumber?: string;
  createdByUserName: string;
  createdAt: string;
}

const UoM: Record<number, string> = { 0: 'Adet', 1: 'kg', 2: 'lt', 3: 'm', 4: 'm²', 5: 'm³' };

const mockTx: InventoryTransaction[] = [
  { id: '1', itemName: 'Çadır XL 6 Kişilik', unitOfMeasure: 0, facilityName: 'Ankara Depo A1', transactionType: 0, quantityChange: 150, previousQuantity: 0,   newQuantity: 150, createdByUserName: 'admin',   createdAt: '2025-05-27T08:15:00Z', referenceTrackingNumber: 'TRK-001' },
  { id: '2', itemName: 'Jeneratör 5kW',       unitOfMeasure: 0, facilityName: 'İstanbul B',    transactionType: 1, quantityChange: 10,  previousQuantity: 45,  newQuantity: 35,  createdByUserName: 'manager1', createdAt: '2025-05-27T07:30:00Z', referenceTrackingNumber: 'TRK-002' },
  { id: '3', itemName: 'Su Arıtma Sistemi',   unitOfMeasure: 0, facilityName: 'İzmir Merkez',  transactionType: 2, quantityChange: 5,   previousQuantity: 20,  newQuantity: 15,  createdByUserName: 'operator', createdAt: '2025-05-26T18:00:00Z' },
  { id: '4', itemName: 'İlk Yardım Seti Pro', unitOfMeasure: 0, facilityName: 'Ankara Depo A1', transactionType: 0, quantityChange: 200, previousQuantity: 50,  newQuantity: 250, createdByUserName: 'admin',   createdAt: '2025-05-26T14:20:00Z', referenceTrackingNumber: 'TRK-003' },
  { id: '5', itemName: 'Fiber Uydu Kiti',     unitOfMeasure: 0, facilityName: 'Bursa Depo',    transactionType: 3, quantityChange: 3,   previousQuantity: 10,  newQuantity: 13,  createdByUserName: 'manager2', createdAt: '2025-05-26T11:00:00Z' },
  { id: '6', itemName: 'Güneş Paneli 200W',   unitOfMeasure: 0, facilityName: 'İzmir Merkez',  transactionType: 1, quantityChange: 8,   previousQuantity: 30,  newQuantity: 22,  createdByUserName: 'operator', createdAt: '2025-05-25T16:45:00Z', referenceTrackingNumber: 'TRK-004' },
  { id: '7', itemName: 'Battaniye Seti',       unitOfMeasure: 0, facilityName: 'Konya Üssü',   transactionType: 0, quantityChange: 500, previousQuantity: 100, newQuantity: 600, createdByUserName: 'admin',   createdAt: '2025-05-25T09:00:00Z' },
];

const summaryStats = [
  { label: 'Toplam Giriş',  value: '+860', color: '#6ee7b7', glow: '#10b981' },
  { label: 'Toplam Çıkış',  value: '-18',  color: '#fcd34d', glow: '#f59e0b' },
  { label: 'Transfer',      value: '5',    color: '#93c5fd', glow: '#3b82f6' },
  { label: 'Düzeltme',      value: '3',    color: '#a78bfa', glow: '#7c3aed' },
];

export default function Inventory() {
  const [transactions, setTransactions] = useState<InventoryTransaction[]>([]);
  const [loading, setLoading]           = useState(true);
  const [search, setSearch]             = useState('');
  const [filterType, setFilterType]     = useState<number | null>(null);

  useEffect(() => { fetchTransactions(); }, []);

  const fetchTransactions = async () => {
    setLoading(true);
    try {
      const res  = await api.get('/InventoryTransaction');
      const data = res.data?.items || res.data?.data || [];
      setTransactions(data.length ? data : mockTx);
    } catch {
      setTransactions(mockTx);
    } finally {
      setLoading(false);
    }
  };

  const filtered = transactions.filter(t => {
    const matchSearch = t.itemName.toLowerCase().includes(search.toLowerCase()) ||
      t.facilityName.toLowerCase().includes(search.toLowerCase());
    const matchFilter = filterType === null ? true : t.transactionType === filterType;
    return matchSearch && matchFilter;
  });

  const formatDate = (s: string) =>
    new Date(s).toLocaleString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });

  return (
    <div>
      {/* Header */}
      <div className="page-header">
        <div>
          <h1 className="page-title">Envanter Hareketleri</h1>
          <p className="page-subtitle">Tüm stok giriş-çıkış ve transfer kayıtları</p>
        </div>
      </div>

      {/* Summary strip */}
      <div style={{ display: 'flex', gap: '16px', marginBottom: '28px', flexWrap: 'wrap' }}>
        {summaryStats.map((s, i) => (
          <div key={i} className="animate-fade-up" style={{
            animationDelay: `${i * 0.07}s`,
            display: 'flex', alignItems: 'center', gap: '14px',
            background: 'rgba(255,255,255,0.03)', border: '1px solid var(--glass-border)',
            borderRadius: '14px', padding: '16px 22px',
          }}>
            <div style={{
              width: '10px', height: '10px', borderRadius: '50%',
              background: s.glow, boxShadow: `0 0 14px ${s.glow}`, flexShrink: 0,
            }} />
            <div>
              <div style={{
                fontSize: '1.5rem', fontWeight: 700, lineHeight: 1,
                color: s.color, fontFamily: "'Space Grotesk', sans-serif",
              }}>{s.value}</div>
              <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)', marginTop: '3px' }}>{s.label}</div>
            </div>
          </div>
        ))}
      </div>

      {/* Table wrapper */}
      <div className="data-table-wrapper animate-fade-up">
        {/* Toolbar */}
        <div className="data-table-header" style={{ flexWrap: 'wrap', gap: '10px' }}>
          <div className="search-bar" style={{ minWidth: 280 }}>
            <Search size={15} style={{ color: 'var(--text-muted)', flexShrink: 0 }} />
            <input
              type="text"
              placeholder="Kalem adı veya tesis ara..."
              value={search}
              onChange={e => setSearch(e.target.value)}
            />
          </div>

          <div style={{ display: 'flex', gap: '8px', flexWrap: 'wrap' }}>
            <button
              className="btn btn-ghost"
              onClick={() => setFilterType(null)}
              style={{
                padding: '8px 14px', fontSize: '0.8rem',
                background: filterType === null ? 'rgba(124,58,237,0.15)' : undefined,
                color: filterType === null ? '#a78bfa' : undefined,
              }}
            >
              <Filter size={13} /> Tümü
            </button>
            {Object.entries(TxType).map(([key, cfg]) => (
              <button
                key={key}
                className="btn btn-ghost"
                onClick={() => setFilterType(Number(key))}
                style={{
                  padding: '8px 14px', fontSize: '0.8rem',
                  background: filterType === Number(key) ? 'rgba(124,58,237,0.15)' : undefined,
                  color: filterType === Number(key) ? '#a78bfa' : undefined,
                }}
              >
                {cfg.icon} {cfg.label}
              </button>
            ))}
          </div>
        </div>

        {/* Table */}
        {loading ? (
          <div style={{ padding: '60px', textAlign: 'center', color: 'var(--text-muted)' }}>
            <Layers size={36} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
            <p>Yükleniyor...</p>
          </div>
        ) : (
          <div style={{ overflowX: 'auto' }}>
            <table className="data-table" style={{ minWidth: '860px' }}>
              <thead>
                <tr>
                  <th>Tarih</th>
                  <th>Kalem</th>
                  <th>Tesis</th>
                  <th>İşlem Tipi</th>
                  <th style={{ textAlign: 'right' }}>Değişim</th>
                  <th style={{ textAlign: 'right' }}>Önceki</th>
                  <th style={{ textAlign: 'right' }}>Sonraki</th>
                  <th>Ref. No</th>
                  <th>Kullanıcı</th>
                </tr>
              </thead>
              <tbody>
                {filtered.length === 0 ? (
                  <tr>
                    <td colSpan={9} style={{ textAlign: 'center', padding: '48px', color: 'var(--text-muted)' }}>
                      Kayıt bulunamadı.
                    </td>
                  </tr>
                ) : filtered.map(tx => {
                  const cfg = TxType[tx.transactionType] ?? TxType[0];
                  const isIn  = cfg.dir === 'in';
                  const isOut = cfg.dir === 'out';
                  return (
                    <tr key={tx.id}>
                      <td style={{ whiteSpace: 'nowrap', fontSize: '0.8rem' }}>{formatDate(tx.createdAt)}</td>
                      <td style={{ fontWeight: 500, color: 'var(--text-primary)' }}>{tx.itemName}</td>
                      <td style={{ fontSize: '0.85rem' }}>{tx.facilityName}</td>
                      <td><span className={`badge ${cfg.badge}`}>{cfg.icon}{cfg.label}</span></td>
                      <td style={{ textAlign: 'right' }}>
                        <span style={{
                          fontFamily: "'Space Grotesk', sans-serif",
                          fontWeight: 700,
                          fontSize: '0.95rem',
                          color: isIn ? '#6ee7b7' : isOut ? '#f87171' : '#93c5fd',
                        }}>
                          {isIn ? '+' : isOut ? '−' : '⇄'}{tx.quantityChange} {UoM[tx.unitOfMeasure] ?? ''}
                        </span>
                      </td>
                      <td style={{ textAlign: 'right', fontFamily: 'monospace', fontSize: '0.85rem' }}>
                        {tx.previousQuantity}
                      </td>
                      <td style={{ textAlign: 'right', fontFamily: 'monospace', fontSize: '0.85rem', fontWeight: 600, color: 'var(--text-primary)' }}>
                        {tx.newQuantity}
                      </td>
                      <td>
                        {tx.referenceTrackingNumber ? (
                          <code style={{
                            fontSize: '0.78rem', background: 'rgba(255,255,255,0.05)',
                            padding: '2px 7px', borderRadius: '4px', color: '#a78bfa',
                          }}>{tx.referenceTrackingNumber}</code>
                        ) : (
                          <span style={{ color: 'var(--text-muted)', fontSize: '0.8rem' }}>—</span>
                        )}
                      </td>
                      <td style={{ fontSize: '0.82rem', color: 'var(--text-secondary)' }}>{tx.createdByUserName}</td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}

        {/* Footer */}
        <div style={{
          display: 'flex', alignItems: 'center', justifyContent: 'space-between',
          padding: '14px 24px', borderTop: '1px solid var(--glass-border)',
          color: 'var(--text-muted)', fontSize: '0.82rem',
        }}>
          <span>{filtered.length} kayıt gösteriliyor</span>
          <div style={{ display: 'flex', gap: '6px' }}>
            {[1, 2, 3].map(n => (
              <button key={n} className="btn btn-ghost" style={{
                width: '32px', height: '32px', padding: 0, fontSize: '0.82rem',
                background: n === 1 ? 'rgba(124,58,237,0.15)' : undefined,
                color:      n === 1 ? '#a78bfa' : undefined,
                borderColor: n === 1 ? 'rgba(124,58,237,0.3)' : undefined,
              }}>{n}</button>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
