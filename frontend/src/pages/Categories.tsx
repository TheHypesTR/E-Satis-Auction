import { useEffect, useState } from 'react';
import { Tag, Plus, Search, ChevronDown, ChevronRight, CheckCircle2, XCircle, Layers } from 'lucide-react';
import api from '../api/axios';

interface AttributeSummary {
  name: string;
  dataType: number; // enum
  target: number;
  isRequired: boolean;
}

interface Category {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  attributes: AttributeSummary[];
}

const dataTypeLabel: Record<number, string> = {
  0: 'Metin',
  1: 'Sayı',
  2: 'Tarih',
  3: 'Liste',
  4: 'Onay',
};

const targetLabel: Record<number, { text: string; badge: string }> = {
  1: { text: 'Ürün Düzeyi', badge: 'badge-purple' },
  2: { text: 'Kalem Düzeyi', badge: 'badge-blue' },
};



export default function Categories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading]       = useState(true);
  const [search, setSearch]         = useState('');
  const [expanded, setExpanded]     = useState<string | null>(null);
  const [filterActive, setFilterActive] = useState<boolean | null>(null);

  const fetchCategories = async () => {
    setLoading(true);
    try {
      const res  = await api.get('/Category');
      const data = res.data?.items || res.data?.data || [];
      setCategories(data);
    } catch {
      setCategories([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchCategories(); }, []);

  const filtered = categories.filter(c => {
    const matchSearch = c.name.toLowerCase().includes(search.toLowerCase());
    const matchFilter = filterActive === null ? true : c.isActive === filterActive;
    return matchSearch && matchFilter;
  });

  const toggleExpand = (id: string) => setExpanded(prev => prev === id ? null : id);

  return (
    <div>
      {/* Header */}
      <div className="page-header">
        <div>
          <h1 className="page-title">Kategoriler</h1>
          <p className="page-subtitle">{categories.length} kategori — dinamik attribute şeması</p>
        </div>
        <button className="btn btn-primary">
          <Plus size={16} />
          Yeni Kategori
        </button>
      </div>

      {/* Toolbar */}
      <div style={{ display: 'flex', gap: '12px', marginBottom: '24px', flexWrap: 'wrap' }}>
        <div className="search-bar" style={{ minWidth: 280 }}>
          <Search size={15} style={{ color: 'var(--text-muted)', flexShrink: 0 }} />
          <input
            type="text"
            placeholder="Kategori ara..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        {/* Filter chips */}
        {([null, true, false] as const).map((val, i) => {
          const label = val === null ? 'Tümü' : val ? 'Aktif' : 'Pasif';
          const active = filterActive === val;
          return (
            <button
              key={i}
              onClick={() => setFilterActive(val)}
              className="btn btn-ghost"
              style={{
                padding: '9px 16px', fontSize: '0.82rem',
                background: active ? (val === false ? 'rgba(245,158,11,0.15)' : 'rgba(124,58,237,0.15)') : undefined,
                color:      active ? (val === false ? '#fcd34d' : '#a78bfa') : undefined,
                borderColor: active ? (val === false ? 'rgba(245,158,11,0.3)' : 'rgba(124,58,237,0.3)') : undefined,
              }}
            >
              {label}
            </button>
          );
        })}
      </div>

      {/* Accordion list */}
      {loading ? (
        <div style={{ textAlign: 'center', padding: '80px', color: 'var(--text-muted)' }}>
          <Tag size={36} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
          <p>Yükleniyor...</p>
        </div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
          {filtered.map((cat, i) => {
            const isOpen = expanded === cat.id;
            return (
              <div
                key={cat.id}
                className="animate-fade-up"
                style={{
                  animationDelay: `${i * 0.05}s`,
                  background: 'var(--glass-1)',
                  backdropFilter: 'blur(20px)',
                  WebkitBackdropFilter: 'blur(20px)',
                  border: isOpen ? '1px solid rgba(124,58,237,0.35)' : '1px solid var(--glass-border)',
                  borderRadius: '16px',
                  overflow: 'hidden',
                  transition: 'border-color 0.3s',
                }}
              >
                {/* Accordion header */}
                <div
                  style={{
                    display: 'flex', alignItems: 'center', gap: '16px',
                    padding: '20px 24px', cursor: 'pointer', userSelect: 'none',
                  }}
                  onClick={() => toggleExpand(cat.id)}
                >
                  {/* Icon */}
                  <div style={{
                    width: '40px', height: '40px', borderRadius: '11px', flexShrink: 0,
                    background: isOpen ? 'rgba(124,58,237,0.18)' : 'rgba(255,255,255,0.05)',
                    border: isOpen ? '1px solid rgba(124,58,237,0.3)' : '1px solid var(--glass-border)',
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    transition: 'all 0.3s',
                  }}>
                    <Tag size={17} color={isOpen ? '#a78bfa' : 'var(--text-muted)'} />
                  </div>

                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '10px', flexWrap: 'wrap' }}>
                      <span style={{ fontWeight: 600, fontSize: '0.95rem', color: 'var(--text-primary)' }}>{cat.name}</span>
                      {cat.isActive
                        ? <span className="badge badge-green"><CheckCircle2 size={11} /> Aktif</span>
                        : <span className="badge badge-amber"><XCircle size={11} /> Pasif</span>
                      }
                      <span style={{ fontSize: '0.78rem', color: 'var(--text-muted)', display: 'flex', alignItems: 'center', gap: '4px' }}>
                        <Layers size={11} /> {cat.attributes.length} attribute
                      </span>
                    </div>
                    {cat.description && (
                      <p style={{ fontSize: '0.82rem', color: 'var(--text-muted)', marginTop: '3px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                        {cat.description}
                      </p>
                    )}
                  </div>

                  <div style={{ color: 'var(--text-muted)', transition: 'transform 0.3s', transform: isOpen ? 'rotate(90deg)' : 'none' }}>
                    <ChevronRight size={18} />
                  </div>
                </div>

                {/* Expanded details */}
                {isOpen && (
                  <div style={{ borderTop: '1px solid var(--glass-border)', padding: '20px 24px' }}>
                    <div style={{ marginBottom: '16px', display: 'flex', gap: '24px', flexWrap: 'wrap' }}>
                      <div>
                        <div style={{ fontSize: '0.75rem', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: '4px' }}>Oluşturulma</div>
                        <div style={{ fontSize: '0.88rem' }}>{new Date(cat.createdAt).toLocaleDateString('tr-TR')}</div>
                      </div>
                      <div>
                        <div style={{ fontSize: '0.75rem', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: '4px' }}>Son Güncelleme</div>
                        <div style={{ fontSize: '0.88rem' }}>{new Date(cat.updatedAt).toLocaleDateString('tr-TR')}</div>
                      </div>
                    </div>

                    {cat.attributes.length > 0 ? (
                      <>
                        <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.08em', marginBottom: '12px', fontWeight: 600 }}>
                          Dinamik Attribute'lar
                        </div>
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                          {cat.attributes.map((attr, ai) => (
                            <div key={ai} style={{
                              display: 'flex', alignItems: 'center', gap: '12px', flexWrap: 'wrap',
                              padding: '11px 16px', background: 'rgba(0,0,0,0.2)',
                              border: '1px solid var(--glass-border)', borderRadius: '10px',
                            }}>
                              <span style={{ fontWeight: 500, fontSize: '0.88rem', color: 'var(--text-primary)', flex: 1, minWidth: 120 }}>{attr.name}</span>
                              <span className="badge badge-blue">{dataTypeLabel[attr.dataType] ?? 'Bilinmiyor'}</span>
                              <span className={`badge ${(targetLabel[attr.target] ?? targetLabel[1]).badge}`}>{(targetLabel[attr.target] ?? targetLabel[1]).text}</span>
                              {attr.isRequired
                                ? <span className="badge badge-green">Zorunlu</span>
                                : <span style={{ fontSize: '0.76rem', color: 'var(--text-muted)' }}>İsteğe Bağlı</span>
                              }
                            </div>
                          ))}
                        </div>
                      </>
                    ) : (
                      <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem' }}>Bu kategoriye henüz attribute eklenmemiş.</p>
                    )}

                    <div style={{ display: 'flex', gap: '8px', marginTop: '20px' }}>
                      <button className="btn btn-ghost" style={{ fontSize: '0.82rem', padding: '8px 14px' }}>Düzenle</button>
                      {cat.isActive
                        ? <button className="btn btn-ghost" style={{ fontSize: '0.82rem', padding: '8px 14px', color: '#fcd34d' }}>Deaktif Et</button>
                        : <button className="btn btn-ghost" style={{ fontSize: '0.82rem', padding: '8px 14px', color: '#6ee7b7' }}>Aktif Et</button>
                      }
                      <button className="btn btn-ghost" style={{ fontSize: '0.82rem', padding: '8px 14px' }}>
                        <Plus size={13} /> Attribute Ekle
                      </button>
                    </div>
                  </div>
                )}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
