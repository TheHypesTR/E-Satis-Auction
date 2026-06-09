/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useMemo, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { Search, Filter, ShoppingBag, Package, X, ChevronDown } from 'lucide-react';
import { useCart } from '../../context/CartContext';
import { commerceApi } from '../../services/commerceApi';
import { formatMoney, getApiErrorMessage } from '../../services/apiUtils';
import type { CategoryDto, ProductListingSummaryDto } from '../../types/commerce';

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
  { value: 'name_asc', label: 'İsim A-Z' },
  { value: 'name_desc', label: 'İsim Z-A' },
  { value: 'price_asc', label: 'Fiyat: Düşükten Yükseğe' },
  { value: 'price_desc', label: 'Fiyat: Yüksekten Düşüğe' },
];

export default function UserCatalog() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [listings, setListings] = useState<ProductListingSummaryDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [search, setSearch] = useState(searchParams.get('q') ?? '');
  const [selCategory, setSelCategory] = useState(searchParams.get('categoryId') ?? '');
  const [minPrice, setMinPrice] = useState(searchParams.get('minPrice') ?? '');
  const [maxPrice, setMaxPrice] = useState(searchParams.get('maxPrice') ?? '');
  const [sort, setSort] = useState('default');
  const [showFilters, setShowFilters] = useState(false);
  const [addedId, setAddedId] = useState<string | null>(null);
  const { addItem } = useCart();

  useEffect(() => {
    const params = {
      pageSize: 50,
      searchTerm: searchParams.get('q') || undefined,
      categoryId: searchParams.get('categoryId') || undefined,
      minPrice: searchParams.get('minPrice') || undefined,
      maxPrice: searchParams.get('maxPrice') || undefined,
    };
    setLoading(true);
    setError('');
    commerceApi.getListings(params)
      .then(setListings)
      .catch(err => { setListings([]); setError(getApiErrorMessage(err, 'Ürünler yüklenemedi.')); })
      .finally(() => setLoading(false));
  }, [searchParams]);

  useEffect(() => {
    commerceApi.getCategories({ pageSize: 100 }).then(setCategories).catch(() => setCategories([]));
  }, []);

  const filtered = useMemo(() => {
    const data = [...listings];
    if (sort === 'name_asc') data.sort((a, b) => a.productName.localeCompare(b.productName));
    if (sort === 'name_desc') data.sort((a, b) => b.productName.localeCompare(a.productName));
    if (sort === 'price_asc') data.sort((a, b) => a.price - b.price);
    if (sort === 'price_desc') data.sort((a, b) => b.price - a.price);
    return data;
  }, [listings, sort]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const next: Record<string, string> = {};
    if (search.trim()) next.q = search.trim();
    if (selCategory) next.categoryId = selCategory;
    if (minPrice) next.minPrice = minPrice;
    if (maxPrice) next.maxPrice = maxPrice;
    setSearchParams(next);
  };

  const clearFilters = () => {
    setSearch(''); setSelCategory(''); setMinPrice(''); setMaxPrice(''); setSearchParams({});
  };

  const handleAddToCart = (p: ProductListingSummaryDto, e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    addItem({ id: p.id, name: p.productName, sku: p.sku, price: p.price, categoryName: p.sourceFacilityName, imageGradient: GRADIENTS[parseInt(p.id.replace(/-/g, '').slice(0, 8), 16) % GRADIENTS.length] ?? GRADIENTS[0] });
    setAddedId(p.id);
    setTimeout(() => setAddedId(null), 1500);
  };

  return (
    <div className="user-page">
      <div style={{ marginBottom: 32 }}>
        <div className="user-section-label">Alışveriş</div>
        <h1 style={{ fontSize: '1.9rem', fontWeight: 700 }}>Ürün Kataloğu</h1>
        <p style={{ color: 'var(--text-muted)', marginTop: 4, fontSize: '0.9rem' }}>{listings.length} aktif listing listeleniyor</p>
      </div>

      <div style={{ display: 'flex', gap: 12, marginBottom: 24, flexWrap: 'wrap', alignItems: 'center' }}>
        <form className="search-bar" style={{ flex: 1, minWidth: 260 }} onSubmit={handleSearch}>
          <Search size={15} style={{ color: 'var(--text-muted)', flexShrink: 0 }} />
          <input type="text" placeholder="Ürün adı veya SKU ara..." value={search} onChange={e => setSearch(e.target.value)} />
          {search && <button type="button" style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--text-muted)', display: 'flex' }} onClick={() => { setSearch(''); setSearchParams({}); }}><X size={14} /></button>}
        </form>
        <button className="btn btn-ghost" style={{ gap: 6 }} onClick={() => setShowFilters(v => !v)}><Filter size={14} /> Filtreler {(selCategory || minPrice || maxPrice) && <span className="user-filter-dot" />}</button>
        <div style={{ position: 'relative' }}>
          <select value={sort} onChange={e => setSort(e.target.value)} className="user-select">{sortOptions.map(o => <option key={o.value} value={o.value}>{o.label}</option>)}</select>
          <ChevronDown size={13} style={{ position: 'absolute', right: 10, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none', color: 'var(--text-muted)' }} />
        </div>
      </div>

      {showFilters && (
        <form className="user-filter-panel animate-fade-up" onSubmit={handleSearch}>
          <div>
            <div style={{ fontSize: '0.75rem', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.08em', color: 'var(--text-muted)', marginBottom: 10 }}>Kategori</div>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8 }}>
              <button type="button" className={`btn ${!selCategory ? 'btn-primary' : 'btn-ghost'}`} style={{ padding: '6px 14px', fontSize: '0.82rem' }} onClick={() => setSelCategory('')}>Tümü</button>
              {categories.map(c => <button type="button" key={c.id} className={`btn ${selCategory === c.id ? 'btn-primary' : 'btn-ghost'}`} style={{ padding: '6px 14px', fontSize: '0.82rem' }} onClick={() => setSelCategory(selCategory === c.id ? '' : c.id)}>{c.name}</button>)}
            </div>
          </div>
          <div style={{ display: 'flex', gap: 10, marginTop: 16, flexWrap: 'wrap' }}>
            <input className="form-input" style={{ maxWidth: 160 }} type="number" placeholder="Min fiyat" value={minPrice} onChange={e => setMinPrice(e.target.value)} />
            <input className="form-input" style={{ maxWidth: 160 }} type="number" placeholder="Max fiyat" value={maxPrice} onChange={e => setMaxPrice(e.target.value)} />
            <button className="btn btn-primary" type="submit">Uygula</button>
            <button className="btn btn-ghost" type="button" onClick={clearFilters}>Temizle</button>
          </div>
        </form>
      )}

      <div style={{ marginBottom: 16, fontSize: '0.85rem', color: 'var(--text-muted)' }}>{filtered.length} sonuç bulundu</div>
      {loading ? <Empty icon={Package} text="Yükleniyor..." /> : error ? <Empty icon={Package} text={error} /> : filtered.length === 0 ? <Empty icon={Search} text="Sonuç bulunamadı." /> : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(260px, 1fr))', gap: 20 }}>
          {filtered.map((p, i) => {
            const isAdded = addedId === p.id;
            return <Link key={p.id} to={`/user/catalog/${p.id}`} className="user-product-card animate-fade-up" style={{ textDecoration: 'none', animationDelay: `${Math.min(i, 8) * 0.05}s` } as React.CSSProperties}>
              <div className="user-product-img" style={{ background: GRADIENTS[i % GRADIENTS.length] }}><ShoppingBag size={36} color="rgba(255,255,255,0.6)" /></div>
              <div className="user-product-body">
                <span className="badge badge-purple" style={{ fontSize: '0.7rem', marginBottom: 6 }}>{p.sourceFacilityName}</span>
                <h3 className="user-product-name">{p.productName}</h3>
                <code style={{ fontSize: '0.75rem', color: 'var(--text-muted)', display: 'block', marginBottom: 12 }}>{p.sku}</code>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                  <span className="user-product-price">{formatMoney(p.price, p.currency)}</span>
                  <button className={`btn ${isAdded ? 'btn-ghost' : 'btn-primary'}`} style={{ padding: '8px 14px', fontSize: '0.8rem', ...(isAdded ? { color: '#6ee7b7', borderColor: 'rgba(16,185,129,0.3)' } : {}) }} onClick={e => handleAddToCart(p, e)}>{isAdded ? 'Eklendi' : 'Sepete Ekle'}</button>
                </div>
              </div>
            </Link>;
          })}
        </div>
      )}
    </div>
  );
}

function Empty({ icon: Icon, text }: { icon: typeof Package; text: string }) {
  return <div style={{ textAlign: 'center', padding: '80px 0', color: 'var(--text-muted)' }}><Icon size={40} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>;
}

