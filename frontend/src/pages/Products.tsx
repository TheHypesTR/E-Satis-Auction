import { useEffect, useState } from 'react';
import { Plus, Search, Filter, MoreVertical, Package } from 'lucide-react';
import api from '../api/axios';

interface Product {
  id: string;
  name: string;
  sku: string;
  categoryName?: string;
  barcode?: string;
}

const mockProducts: Product[] = [
  { id: '1', name: 'Çadır XL 6 Kişilik', sku: 'CAD-001', categoryName: 'Barınma', barcode: '8681234567890' },
  { id: '2', name: 'Jeneratör 5kW Dizel', sku: 'ENR-002', categoryName: 'Enerji', barcode: '8689876543210' },
  { id: '3', name: 'İlk Yardım Seti Pro', sku: 'MED-003', categoryName: 'Medikal', barcode: '8681122334455' },
  { id: '4', name: 'Su Arıtma Sistemi', sku: 'SU-004', categoryName: 'Su & Gıda', barcode: '8685544332211' },
  { id: '5', name: 'Fiber Uydu Kiti', sku: 'İLT-005', categoryName: 'İletişim', barcode: '8686677889900' },
  { id: '6', name: 'Taşınabilir Güneş Paneli', sku: 'ENR-006', categoryName: 'Enerji', barcode: '8680011223344' },
];

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

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    setLoading(true);
    try {
      const res = await api.get('/Product');
      const data = res.data?.items || res.data?.data || [];
      setProducts(data.length ? data : mockProducts);
    } catch {
      setProducts(mockProducts);
    } finally {
      setLoading(false);
    }
  };

  const filtered = products.filter(p =>
    p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    p.sku.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div>
      {/* Page header */}
      <div className="page-header">
        <div>
          <h1 className="page-title">Ürünler</h1>
          <p className="page-subtitle">{products.length} ürün listeleniyor</p>
        </div>
        <button className="btn btn-primary">
          <Plus size={16} />
          Yeni Ürün
        </button>
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
                <th>Barkod</th>
                <th style={{ textAlign: 'right' }}>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((p) => {
                const colorKey = categoryColors[p.categoryName ?? ''] ?? 'purple';
                return (
                  <tr key={p.id}>
                    <td>
                      <code style={{
                        fontFamily: 'monospace',
                        fontSize: '0.82rem',
                        background: 'rgba(255,255,255,0.05)',
                        padding: '3px 8px',
                        borderRadius: '5px',
                        color: '#a78bfa'
                      }}>
                        {p.sku}
                      </code>
                    </td>
                    <td>
                      <div style={{ fontWeight: 500, color: 'var(--text-primary)', fontSize: '0.92rem' }}>
                        {p.name}
                      </div>
                    </td>
                    <td>
                      {p.categoryName && (
                        <span className={`badge badge-${colorKey}`}>
                          {p.categoryName}
                        </span>
                      )}
                    </td>
                    <td>
                      <span style={{ fontSize: '0.82rem', color: 'var(--text-muted)', fontFamily: 'monospace' }}>
                        {p.barcode ?? '—'}
                      </span>
                    </td>
                    <td style={{ textAlign: 'right' }}>
                      <button
                        className="btn btn-ghost"
                        style={{ padding: '7px 14px', fontSize: '0.8rem' }}
                      >
                        <MoreVertical size={14} />
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}

        {/* Footer */}
        <div style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          padding: '16px 24px',
          borderTop: '1px solid var(--glass-border)',
          color: 'var(--text-muted)',
          fontSize: '0.82rem'
        }}>
          <span>{filtered.length} kayıt gösteriliyor</span>
          <div style={{ display: 'flex', gap: '6px' }}>
            {[1, 2, 3].map(n => (
              <button key={n} className="btn btn-ghost" style={{
                width: '32px',
                height: '32px',
                padding: 0,
                fontSize: '0.82rem',
                background: n === 1 ? 'rgba(124,58,237,0.15)' : undefined,
                color: n === 1 ? '#a78bfa' : undefined,
                borderColor: n === 1 ? 'rgba(124,58,237,0.3)' : undefined,
              }}>
                {n}
              </button>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
