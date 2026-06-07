import { useEffect, useState } from 'react';
import { Plus, Search, Filter, MoreVertical, Package, Play, Square, Gift, X, Check } from 'lucide-react';
import api from '../api/axios';

interface Product {
  id: string;
  name: string;
  sku: string;
  categoryName?: string;
  barcode?: string;
  saleStatus?: 'Satışta' | 'Kapalı';
}

const categoryColors: Record<string, string> = {
  Barınma: 'purple',
  Enerji: 'amber',
  Medikal: 'green',
  'Su & Gıda': 'blue',
  İletişim: 'blue',
};

export default function Products() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  
  const [showCampaignModal, setShowCampaignModal] = useState(false);
  const [campaignTitle, setCampaignTitle] = useState('');
  const [campaignDiscount, setCampaignDiscount] = useState('');
  const [campaignSaved, setCampaignSaved] = useState(false);

  useEffect(() => {
    void api.get('/Product').then(res => {
      const data = res.data?.items || res.data?.data || [];
      // Add mock saleStatus for demonstration if not provided
      const enrichedData = data.map((p: any) => ({ ...p, saleStatus: p.saleStatus || 'Kapalı' }));
      setProducts(enrichedData);
      setLoading(false);
    }).catch(() => { setProducts([]); setLoading(false); });
  }, []);

  const filtered = products.filter(p =>
    p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    p.sku.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleToggleSale = (id: string, currentStatus: string | undefined) => {
    const newStatus = currentStatus === 'Satışta' ? 'Kapalı' : 'Satışta';
    setProducts(prev => prev.map(p => p.id === id ? { ...p, saleStatus: newStatus } : p));
  };

  const handleSaveCampaign = () => {
    if(!campaignTitle || !campaignDiscount) return;
    setCampaignSaved(true);
    setTimeout(() => {
      setCampaignSaved(false);
      setShowCampaignModal(false);
      setCampaignTitle('');
      setCampaignDiscount('');
    }, 1500);
  };

  return (
    <div>
      {/* Page header */}
      <div className="page-header">
        <div>
          <h1 className="page-title">Ürünler</h1>
          <p className="page-subtitle">{products.length} ürün listeleniyor</p>
        </div>
        <div style={{ display: 'flex', gap: 12 }}>
          <button className="btn btn-ghost" style={{ gap: 8 }} onClick={() => setShowCampaignModal(true)}>
            <Gift size={16} /> Kampanyalar
          </button>
          <button className="btn btn-primary" style={{ gap: 8 }}>
            <Plus size={16} /> Yeni Ürün
          </button>
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
              placeholder="Ürün adı veya SKU ara..."
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

        {/* Table */}
        {loading ? (
          <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}>
            <Package size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
            <p>Yükleniyor...</p>
          </div>
        ) : filtered.length === 0 ? (
          <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}>
            <Package size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
            <p>Ürün bulunamadı.</p>
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>SKU</th>
                <th>Ürün Adı</th>
                <th>Kategori</th>
                <th>Satış Durumu</th>
                <th style={{ textAlign: 'right' }}>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((p) => {
                const colorKey = categoryColors[p.categoryName ?? ''] ?? 'purple';
                const isSelling = p.saleStatus === 'Satışta';
                return (
                  <tr key={p.id}>
                    <td>
                      <code style={{ fontFamily: 'monospace', fontSize: '0.82rem', background: 'rgba(255,255,255,0.05)', padding: '3px 8px', borderRadius: '5px', color: '#a78bfa' }}>
                        {p.sku}
                      </code>
                    </td>
                    <td>
                      <div style={{ fontWeight: 500, color: 'var(--text-primary)', fontSize: '0.92rem' }}>
                        {p.name}
                      </div>
                    </td>
                    <td>
                      {p.categoryName ? (
                        <span className={`badge badge-${colorKey}`}>
                          {p.categoryName}
                        </span>
                      ) : '—'}
                    </td>
                    <td>
                      <span className={`badge ${isSelling ? 'badge-green' : 'badge-amber'}`}>
                        {p.saleStatus}
                      </span>
                    </td>
                    <td style={{ textAlign: 'right' }}>
                      <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8 }}>
                        <button
                          className={`btn ${isSelling ? 'btn-ghost' : 'btn-primary'}`}
                          style={{ padding: '6px 12px', fontSize: '0.75rem', gap: 4 }}
                          onClick={() => handleToggleSale(p.id, p.saleStatus)}
                        >
                          {isSelling ? <><Square size={12} /> Durdur</> : <><Play size={12} /> Başlat</>}
                        </button>
                        <button
                          className="btn btn-ghost"
                          style={{ padding: '7px 14px', fontSize: '0.8rem' }}
                        >
                          <MoreVertical size={14} />
                        </button>
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}

        {/* Footer */}
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '16px 24px', borderTop: '1px solid var(--glass-border)', color: 'var(--text-muted)', fontSize: '0.82rem' }}>
          <span>{filtered.length} kayıt gösteriliyor</span>
          <div style={{ display: 'flex', gap: '6px' }}>
            {[1, 2, 3].map(n => (
              <button key={n} className="btn btn-ghost" style={{ width: '32px', height: '32px', padding: 0, fontSize: '0.82rem', background: n === 1 ? 'rgba(124,58,237,0.15)' : undefined, color: n === 1 ? '#a78bfa' : undefined, borderColor: n === 1 ? 'rgba(124,58,237,0.3)' : undefined }}>
                {n}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Campaign Modal */}
      {showCampaignModal && (
        <div className="modal-overlay" onClick={() => setShowCampaignModal(false)}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 450, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8 }}>
                <Gift size={20} color="#ec4899" /> Yeni Kampanya
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowCampaignModal(false)}>
                <X size={18} />
              </button>
            </div>
            
            <div style={{ marginBottom: 24, display: 'flex', flexDirection: 'column', gap: 16 }}>
              <div className="form-group">
                <label className="form-label">Kampanya Adı</label>
                <input 
                  type="text" 
                  className="form-input" 
                  placeholder="Örn: Yıl Sonu İndirimi" 
                  value={campaignTitle}
                  onChange={e => setCampaignTitle(e.target.value)}
                />
              </div>
              <div className="form-group">
                <label className="form-label">İndirim Oranı (%)</label>
                <input 
                  type="number" 
                  className="form-input" 
                  placeholder="Örn: 20" 
                  value={campaignDiscount}
                  onChange={e => setCampaignDiscount(e.target.value)}
                />
              </div>
              <div className="form-group">
                <label className="form-label">Geçerli Kategoriler / Ürünler</label>
                <select className="form-input" style={{ appearance: 'auto', backgroundColor: 'var(--bg-secondary)' }}>
                  <option>Tüm Ürünler</option>
                  <option>Sadece Enerji</option>
                  <option>Sadece Medikal</option>
                </select>
              </div>
            </div>

            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => setShowCampaignModal(false)}>İptal</button>
              <button className="btn btn-primary" style={{ gap: 8 }} onClick={handleSaveCampaign}>
                {campaignSaved ? <><Check size={16}/> Kaydedildi</> : 'Kampanyayı Başlat'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
