import { useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { Search, Filter, ShoppingBag, Package, X, ChevronDown } from 'lucide-react';
import api from '../../api/axios';
import { useCart } from '../../context/CartContext';

interface Product {
  id: string;
  name: string;
  sku: string;
  categoryName?: string;
  price?: number;
}

interface Category {
  id: string;
  name: string;
}

const GRADIENTS = [
  'linear-gradient(135deg, #7c3aed 0%, #4f46e5 100%)',
  'linear-gradient(135deg, #3b82f6 0%, #06b6d4 100%)',
  'linear-gradient(135deg, #ec4899 0%, #f59e0b 100%)',
  'linear-gradient(135deg, #10b981 0%, #3b82f6 100%)',
  'linear-gradient(135deg, #f59e0b 0%, #ef4444 100%)',
  'linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%)',
];



const sortOptions = [
  { value: 'default', label: 'Varsayılan Sıralama' },
  { value: 'name_asc', label: 'İsim A–Z' },
  { value: 'name_desc', label: 'İsim Z–A' },
  { value: 'price_asc', label: 'Fiyat: Düşükten Yükseğe' },
  { value: 'price_desc', label: 'Fiyat: Yüksekten Düşüğe' },
];

export default function UserCatalog() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [products, setProducts]   = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading]     = useState(true);
  const [search, setSearch]       = useState(searchParams.get('q') ?? '');
  const [selCategory, setSelCategory] = useState<string>('');
  const [sort, setSort]           = useState('default');
  const [showFilters, setShowFilters] = useState(false);
  const [addedId, setAddedId]     = useState<string | null>(null);

  const { addItem } = useCart();

  useEffect(() => {
    void api.get('/ProductListing?pageSize=50').then(res => {
      const data = res.data?.items || res.data?.data || [];
      // Backend'den gelen ProductName alanını frontend Product arayüzündeki name alanına mapliyoruz
      const mappedData = data.map((item: any) => ({
        ...item,
        name: item.productName || item.name,
      }));
      setProducts(mappedData);
      setLoading(false);
    }).catch(() => { setProducts([]); setLoading(false); });
    void api.get('/Category?pageSize=20').then(res => {
      const data = res.data?.items || res.data?.data || [];
      setCategories(data);
    }).catch(() => setCategories([]));
    const q = searchParams.get('q');
    if (q) setSearch(q);
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setSearchParams(search ? { q: search } : {});
  };

  const handleAddToCart = (p: Product, e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    const price = p.price ?? 0;
    addItem({
      id: p.id, name: p.name, sku: p.sku, price,
      categoryName: p.categoryName,
      imageGradient: GRADIENTS[parseInt(p.id.replace(/-/g, '').slice(0, 8), 16) % GRADIENTS.length] ?? GRADIENTS[0],
    });
    setAddedId(p.id);
    setTimeout(() => setAddedId(null), 1500);
  };

  // Filter & sort
  let filtered = products.filter(p => {
    const q = search.toLowerCase();
    const matchSearch = !q || p.name.toLowerCase().includes(q) || p.sku.toLowerCase().includes(q);
    const matchCat    = !selCategory || p.categoryName === selCategory;
    return matchSearch && matchCat;
  });

  if (sort === 'name_asc')   filtered = [...filtered].sort((a, b) => a.name.localeCompare(b.name));
  if (sort === 'name_desc')  filtered = [...filtered].sort((a, b) => b.name.localeCompare(a.name));
  if (sort === 'price_asc')  filtered = [...filtered].sort((a, b) => (a.price ?? 0) - (b.price ?? 0));
  if (sort === 'price_desc') filtered = [...filtered].sort((a, b) => (b.price ?? 0) - (a.price ?? 0));

  return (
    <div className="user-page">
      {/* Header */}
      <div style={{ marginBottom: 32 }}>
        <div className="user-section-label">Alışveriş</div>
        <h1 style={{ fontSize: '1.9rem', fontWeight: 700 }}>Ürün Kataloğu</h1>
        <p style={{ color: 'var(--text-muted)', marginTop: 4, fontSize: '0.9rem' }}>
          {products.length} ürün listeleniyor
        </p>
      </div>

      {/* Toolbar */}
      <div style={{ display: 'flex', gap: 12, marginBottom: 24, flexWrap: 'wrap', alignItems: 'center' }}>
        <form className="search-bar" style={{ flex: 1, minWidth: 260 }} onSubmit={handleSearch}>
          <Search size={15} style={{ color: 'var(--text-muted)', flexShrink: 0 }} />
          <input
            type="text"
            placeholder="Ürün adı veya SKU ara..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
          {search && (
            <button type="button" style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--text-muted)', display: 'flex' }}
              onClick={() => { setSearch(''); setSearchParams({}); }}>
              <X size={14} />
            </button>
          )}
        </form>

        <button
          className="btn btn-ghost"
          style={{ gap: 6 }}
          onClick={() => setShowFilters(v => !v)}
        >
          <Filter size={14} />
          Filtreler
          {(selCategory) && <span className="user-filter-dot" />}
        </button>

        {/* Sort */}
        <div style={{ position: 'relative' }}>
          <select
            value={sort}
            onChange={e => setSort(e.target.value)}
            className="user-select"
          >
            {sortOptions.map(o => (
              <option key={o.value} value={o.value}>{o.label}</option>
            ))}
          </select>
          <ChevronDown size={13} style={{ position: 'absolute', right: 10, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none', color: 'var(--text-muted)' }} />
        </div>
      </div>

      {/* Filters panel */}
      {showFilters && (
        <div className="user-filter-panel animate-fade-up">
          <div>
            <div style={{ fontSize: '0.75rem', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.08em', color: 'var(--text-muted)', marginBottom: 10 }}>Kategori</div>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8 }}>
              <button
                className={`btn ${!selCategory ? 'btn-primary' : 'btn-ghost'}`}
                style={{ padding: '6px 14px', fontSize: '0.82rem' }}
                onClick={() => setSelCategory('')}
              >Tümü</button>
              {categories.map(c => (
                <button
                  key={c.id}
                  className={`btn ${selCategory === c.name ? 'btn-primary' : 'btn-ghost'}`}
                  style={{ padding: '6px 14px', fontSize: '0.82rem' }}
                  onClick={() => setSelCategory(selCategory === c.name ? '' : c.name)}
                >{c.name}</button>
              ))}
            </div>
          </div>
        </div>
      )}

      {/* Results count */}
      <div style={{ marginBottom: 16, fontSize: '0.85rem', color: 'var(--text-muted)' }}>
        {filtered.length} sonuç bulundu
        {selCategory && <> — <button style={{ background: 'none', border: 'none', color: '#a78bfa', cursor: 'pointer', fontSize: '0.85rem' }} onClick={() => setSelCategory('')}>Filtreyi Kaldır ×</button></>}
      </div>

      {/* Product grid */}
      {loading ? (
        <div style={{ textAlign: 'center', padding: '80px 0', color: 'var(--text-muted)' }}>
          <Package size={40} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
          <p>Yükleniyor...</p>
        </div>
      ) : filtered.length === 0 ? (
        <div style={{ textAlign: 'center', padding: '80px 0', color: 'var(--text-muted)' }}>
          <Search size={40} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
          <p style={{ marginBottom: 8 }}>Sonuç bulunamadı.</p>
          <p style={{ fontSize: '0.85rem' }}>Farklı bir arama terimi deneyin.</p>
        </div>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(260px, 1fr))', gap: 20 }}>
          {filtered.map((p, i) => {
            const price = p.price ?? 0;
            const grad  = GRADIENTS[i % GRADIENTS.length];
            const isAdded = addedId === p.id;
            return (
              <Link
                key={p.id}
                to={`/user/catalog/${p.id}`}
                className={`user-product-card animate-fade-up`}
                style={{ textDecoration: 'none', animationDelay: `${Math.min(i, 8) * 0.05}s` } as React.CSSProperties}
              >
                <div className="user-product-img" style={{ background: grad }}>
                  <ShoppingBag size={36} color="rgba(255,255,255,0.6)" />
                </div>
                <div className="user-product-body">
                  {p.categoryName && (
                    <span className="badge badge-purple" style={{ fontSize: '0.7rem', marginBottom: 6 }}>{p.categoryName}</span>
                  )}
                  <h3 className="user-product-name">{p.name}</h3>
                  <code style={{ fontSize: '0.75rem', color: 'var(--text-muted)', display: 'block', marginBottom: 12 }}>{p.sku}</code>
                  <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <span className="user-product-price">₺{price.toLocaleString('tr-TR')}</span>
                    <button
                      className={`btn ${isAdded ? 'btn-ghost' : 'btn-primary'}`}
                      style={{ padding: '8px 14px', fontSize: '0.8rem', transition: 'all 0.3s', ...(isAdded ? { color: '#6ee7b7', borderColor: 'rgba(16,185,129,0.3)' } : {}) }}
                      onClick={e => handleAddToCart(p, e)}
                    >
                      {isAdded ? '✓ Eklendi' : 'Sepete Ekle'}
                    </button>
                  </div>
                </div>
              </Link>
            );
          })}
        </div>
      )}
    </div>
  );
}
