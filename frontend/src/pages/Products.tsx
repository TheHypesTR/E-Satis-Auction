import { useEffect, useState } from 'react';
import { Plus, Search, Filter, MoreVertical, Package, Play, Square, Gift, X, Check, Edit2, Trash2 } from 'lucide-react';
import api from '../api/axios';
import laptopMock from '../assets/laptop-mock.png';

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

// Map product keywords → mock image URL
const getProductMockImage = (name: string, sku: string): string | null => {
  const lower = (name + ' ' + sku).toLowerCase();
  if (lower.includes('laptop') || lower.includes('dizüstü') || lower.includes('notebook') || lower.includes('lpt')) {
    return laptopMock;
  }
  return null;
};

export default function Products() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');

  const [showCampaignModal, setShowCampaignModal] = useState(false);
  const [campaignTitle, setCampaignTitle] = useState('');
  const [campaignDiscount, setCampaignDiscount] = useState('');
  const [campaignSaved, setCampaignSaved] = useState(false);

  // New Modals State
  const [showAddProductModal, setShowAddProductModal] = useState(false);
  const [showEditProductModal, setShowEditProductModal] = useState(false);
  const [showDeleteProductModal, setShowDeleteProductModal] = useState(false);
  const [showAuctionModal, setShowAuctionModal] = useState(false);
  const [auctionStartingPrice, setAuctionStartingPrice] = useState('');
  const [auctionMinBidIncrement, setAuctionMinBidIncrement] = useState('50');
  const [auctionDuration, setAuctionDuration] = useState('60'); // minutes
  const [auctionError, setAuctionError] = useState('');
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [actionSaved, setActionSaved] = useState(false);
  const [activeMenuId, setActiveMenuId] = useState<string | null>(null);

  const handleStartAuction = async () => {
    if (!selectedProduct || !auctionStartingPrice || !auctionDuration) return;
    setAuctionError('');
    try {
      // Step 1: Find or create a ProductListing for this product
      const listingsRes = await api.get(`/AdminProductListing?PageSize=100`);
      const listings: any[] = listingsRes.data?.items || listingsRes.data?.data || [];
      let listingId: string | null = null;

      const existingListing = listings.find((l: any) => l.productId === selectedProduct.id);
      if (existingListing) {
        listingId = existingListing.id;
      } else {
        // We need a facilityId - fetch facilities first
        const facRes = await api.get('/Facility?PageSize=1');
        const facilities: any[] = facRes.data?.items || facRes.data?.data || [];
        if (facilities.length === 0) throw new Error('Tesis bulunamadı. Önce bir tesis ekleyin.');
        const facilityId = facilities[0].id;

        const newListing = await api.post('/AdminProductListing', {
          productId: selectedProduct.id,
          sourceFacilityId: facilityId,
          price: Number(auctionStartingPrice),
          currency: 'TRY',
          quantity: 1
        });
        listingId = newListing.data?.id;
      }

      if (!listingId) throw new Error('ProductListing oluşturulamadı.');

      // Step 2: Create the auction
      const startsAt = new Date().toISOString();
      const endsAt = new Date(Date.now() + Number(auctionDuration) * 60000).toISOString();
      const auctionRes = await api.post('/AdminAuction', {
        productListingId: listingId,
        startingPrice: Number(auctionStartingPrice),
        minimumBidIncrement: Number(auctionMinBidIncrement),
        startsAt,
        endsAt,
        quantity: 1,
        currency: 'TRY'
      });

      // Step 3: Activate the auction immediately
      const auctionId = auctionRes.data?.id;
      if (auctionId) {
        await api.put(`/AdminAuction/${auctionId}/activate`);
      }

      setActionSaved(true);
      setTimeout(() => {
        setActionSaved(false);
        setShowAuctionModal(false);
        setAuctionStartingPrice('');
        setAuctionMinBidIncrement('50');
        setAuctionDuration('60');
      }, 1500);
    } catch (err: any) {
      const msg = err?.response?.data?.errors?.[0]?.message || err?.message || 'Bilinmeyen hata oluştu.';
      setAuctionError(msg);
      console.error('Auction creation failed', err);
    }
  };

  // Add Product form states
  const [categoriesList, setCategoriesList] = useState<{id: string, name: string}[]>([]);
  const [newProductName, setNewProductName] = useState('');
  const [newProductSku, setNewProductSku] = useState('');
  const [newProductBarcode, setNewProductBarcode] = useState('');
  const [newProductCategoryId, setNewProductCategoryId] = useState('');

  const mapCategoryToUnit = (categoryName: string) => {
    const lower = categoryName.toLowerCase();
    if (lower.includes('gıda') || lower.includes('su')) return 2; // Kg
    if (lower.includes('enerji') || lower.includes('medikal') || lower.includes('barınma') || lower.includes('elektronik')) return 1; // Piece
    return 1; // Default to Piece
  };

  const handleAddProduct = async () => {
    if (!newProductName || !newProductSku || !newProductCategoryId) return;
    try {
      const cat = categoriesList.find(c => c.id === newProductCategoryId);
      const unit = cat ? mapCategoryToUnit(cat.name) : 1;
      await api.post('/Product', {
        sku: newProductSku,
        barcode: newProductBarcode || null,
        name: newProductName,
        categoryId: newProductCategoryId,
        unitOfMeasure: unit,
        baseAttributes: {}
      });
      
      setActionSaved(true);
      setTimeout(() => {
        setActionSaved(false);
        setShowAddProductModal(false);
        setNewProductName(''); setNewProductSku(''); setNewProductBarcode(''); setNewProductCategoryId('');
        
        void api.get('/Product').then(res => {
          const data = res.data?.items || res.data?.data || [];
          const enrichedData = data.map((p: any) => ({ ...p, saleStatus: p.saleStatus || 'Kapalı' }));
          setProducts(enrichedData);
        });
      }, 1000);
    } catch (err) {
      console.error('Failed to add product', err);
    }
  };

  useEffect(() => {
    void api.get('/Product').then(res => {
      const data = res.data?.items || res.data?.data || [];
      // Add mock saleStatus for demonstration if not provided
      const enrichedData = data.map((p: any) => ({ ...p, saleStatus: p.saleStatus || 'Kapalı' }));
      setProducts(enrichedData);
      setLoading(false);
    }).catch(() => { setProducts([]); setLoading(false); });

    void api.get('/Category?PageSize=100').then(res => {
      setCategoriesList(res.data?.items || res.data?.data || []);
    }).catch(() => {});
  }, []);

  const filtered = products.filter(p =>
    p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    p.sku.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleToggleSale = (id: string, currentStatus: string | undefined) => {
    const newStatus = currentStatus === 'Satışta' ? 'Kapalı' : 'Satışta';
    setProducts(prev => prev.map(p => p.id === id ? { ...p, saleStatus: newStatus } : p));
  };

  const handleSaveCampaign = async () => {
    if (!campaignTitle || !campaignDiscount) return;
    try {
      await api.post('/AdminCampaign', {
        name: campaignTitle,
        description: 'Frontend üzerinden eklendi',
        scope: 3, // CartOrder (Global)
        discountType: 1, // Percentage
        discountValue: Number(campaignDiscount),
        currency: 'TRY'
      });
      setCampaignSaved(true);
      setTimeout(() => {
        setCampaignSaved(false);
        setShowCampaignModal(false);
        setCampaignTitle('');
        setCampaignDiscount('');
      }, 1500);
    } catch (err) {
      console.error('Campaign save failed', err);
    }
  };

  const handleMockAction = (setter: React.Dispatch<React.SetStateAction<boolean>>) => {
    setActionSaved(true);
    setTimeout(() => {
      setActionSaved(false);
      setter(false);
      setSelectedProduct(null);
    }, 1000);
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
          <button className="btn btn-primary" style={{ gap: 8 }} onClick={() => setShowAddProductModal(true)}>
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
                      <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                        {(() => {
                          const img = getProductMockImage(p.name, p.sku);
                          return img ? (
                            <div style={{
                              width: 48, height: 48, borderRadius: 10, overflow: 'hidden', flexShrink: 0,
                              background: 'rgba(0,0,0,0.3)', border: '1px solid var(--glass-border)',
                              boxShadow: '0 0 16px rgba(124,58,237,0.2)'
                            }}>
                              <img src={img} alt={p.name} style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                            </div>
                          ) : (
                            <div style={{
                              width: 48, height: 48, borderRadius: 10, flexShrink: 0,
                              background: 'rgba(255,255,255,0.04)', border: '1px solid var(--glass-border)',
                              display: 'flex', alignItems: 'center', justifyContent: 'center'
                            }}>
                              <Package size={18} style={{ opacity: 0.3 }} />
                            </div>
                          );
                        })()}
                        <div style={{ fontWeight: 500, color: 'var(--text-primary)', fontSize: '0.92rem' }}>
                          {p.name}
                        </div>
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
                        <div style={{ position: 'relative' }}>
                          <button
                            className="btn btn-ghost"
                            style={{ padding: '7px 14px', fontSize: '0.8rem' }}
                            onClick={() => setActiveMenuId(activeMenuId === p.id ? null : p.id)}
                          >
                            <MoreVertical size={14} />
                          </button>
                          {activeMenuId === p.id && (
                            <div style={{ position: 'absolute', right: 0, top: '100%', marginTop: 4, background: 'var(--bg-secondary)', border: '1px solid var(--glass-border)', borderRadius: 8, padding: 4, zIndex: 10, minWidth: 160, display: 'flex', flexDirection: 'column', gap: 2, textAlign: 'left' }}>
                              <button className="btn btn-ghost" style={{ justifyContent: 'flex-start', padding: '6px 12px', fontSize: '0.8rem', gap: 8 }} onClick={() => { setSelectedProduct(p); setShowEditProductModal(true); setActiveMenuId(null); }}>
                                <Edit2 size={12} /> Düzenle
                              </button>
                              <button className="btn btn-ghost" style={{ justifyContent: 'flex-start', padding: '6px 12px', fontSize: '0.8rem', gap: 8, color: '#a78bfa' }} onClick={() => { setSelectedProduct(p); setShowAuctionModal(true); setActiveMenuId(null); }}>
                                <Play size={12} /> Açık Artırma Başlat
                              </button>
                              <button className="btn btn-ghost" style={{ justifyContent: 'flex-start', padding: '6px 12px', fontSize: '0.8rem', color: '#f87171', gap: 8 }} onClick={() => { setSelectedProduct(p); setShowDeleteProductModal(true); setActiveMenuId(null); }}>
                                <Trash2 size={12} /> Sil
                              </button>
                            </div>
                          )}
                        </div>
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
                {campaignSaved ? <><Check size={16} /> Kaydedildi</> : 'Kampanyayı Başlat'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Auction Modal */}
      {showAuctionModal && selectedProduct && (
        <div className="modal-overlay" onClick={() => { setShowAuctionModal(false); setAuctionError(''); }}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 480, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8, color: '#a78bfa' }}>
                <Play size={20} /> Açık Artırma Başlat
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => { setShowAuctionModal(false); setAuctionError(''); }}>
                <X size={18} />
              </button>
            </div>

            <p style={{ color: 'var(--text-muted)', marginBottom: 20, fontSize: '0.9rem' }}>
              <strong style={{ color: 'var(--text-primary)' }}>{selectedProduct.name}</strong> için açık artırma başlatılacak. Arka planda otomatik olarak bir vitrin kaydı oluşturulacak ve artırma aktive edilecektir.
            </p>

            {auctionError && (
              <div style={{ background: 'rgba(248,113,113,0.1)', border: '1px solid rgba(248,113,113,0.3)', borderRadius: 8, padding: '10px 14px', color: '#f87171', fontSize: '0.85rem', marginBottom: 16 }}>
                {auctionError}
              </div>
            )}

            <div style={{ marginBottom: 24, display: 'flex', flexDirection: 'column', gap: 16 }}>
              <div style={{ display: 'flex', gap: 16 }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label className="form-label">Başlangıç Fiyatı (₺)</label>
                  <input
                    type="number"
                    className="form-input"
                    placeholder="Örn: 1500"
                    value={auctionStartingPrice}
                    onChange={e => setAuctionStartingPrice(e.target.value)}
                  />
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label className="form-label">Min. Artış (₺)</label>
                  <input
                    type="number"
                    className="form-input"
                    placeholder="Örn: 50"
                    value={auctionMinBidIncrement}
                    onChange={e => setAuctionMinBidIncrement(e.target.value)}
                  />
                </div>
              </div>
              <div className="form-group">
                <label className="form-label">Süre (Dakika)</label>
                <input
                  type="number"
                  className="form-input"
                  placeholder="Örn: 60"
                  value={auctionDuration}
                  onChange={e => setAuctionDuration(e.target.value)}
                />
              </div>
            </div>

            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => { setShowAuctionModal(false); setAuctionError(''); }}>İptal</button>
              <button
                className="btn btn-primary"
                style={{ gap: 8, background: 'linear-gradient(135deg, #7c3aed 0%, #ec4899 100%)' }}
                onClick={handleStartAuction}
              >
                {actionSaved ? <><Check size={16} /> Başlatıldı</> : 'Artırmayı Başlat'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Add Product Modal */}
      {showAddProductModal && (
        <div className="modal-overlay" onClick={() => setShowAddProductModal(false)}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 500, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8 }}>
                <Package size={20} color="#a78bfa" /> Yeni Ürün Ekle
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowAddProductModal(false)}>
                <X size={18} />
              </button>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 16, marginBottom: 24 }}>
              <div className="form-group">
                <label className="form-label">Ürün Adı</label>
                <input type="text" className="form-input" placeholder="Örn: Portatif Jeneratör" value={newProductName} onChange={e => setNewProductName(e.target.value)} />
              </div>
              <div style={{ display: 'flex', gap: 16 }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label className="form-label">SKU Kodu</label>
                  <input type="text" className="form-input" placeholder="Örn: JNR-5000" value={newProductSku} onChange={e => setNewProductSku(e.target.value)} />
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label className="form-label">Barkod</label>
                  <input type="text" className="form-input" placeholder="Opsiyonel" value={newProductBarcode} onChange={e => setNewProductBarcode(e.target.value)} />
                </div>
              </div>
              <div className="form-group">
                <label className="form-label">Kategori</label>
                <select className="form-input" style={{ appearance: 'auto', backgroundColor: 'var(--bg-secondary)' }} value={newProductCategoryId} onChange={e => setNewProductCategoryId(e.target.value)}>
                  <option value="">Seçiniz...</option>
                  {categoriesList.map(c => (
                    <option key={c.id} value={c.id}>{c.name}</option>
                  ))}
                </select>
              </div>
            </div>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => setShowAddProductModal(false)}>İptal</button>
              <button className="btn btn-primary" style={{ gap: 8 }} onClick={handleAddProduct}>
                {actionSaved ? <><Check size={16} /> Kaydedildi</> : 'Ürünü Ekle'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Edit Product Modal */}
      {showEditProductModal && selectedProduct && (
        <div className="modal-overlay" onClick={() => setShowEditProductModal(false)}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 500, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8 }}>
                <Edit2 size={20} color="#60a5fa" /> Ürün Düzenle
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowEditProductModal(false)}>
                <X size={18} />
              </button>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 16, marginBottom: 24 }}>
              <div className="form-group">
                <label className="form-label">Ürün Adı</label>
                <input type="text" className="form-input" defaultValue={selectedProduct.name} />
              </div>
              <div style={{ display: 'flex', gap: 16 }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label className="form-label">SKU Kodu</label>
                  <input type="text" className="form-input" defaultValue={selectedProduct.sku} disabled />
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label className="form-label">Satış Durumu</label>
                  <select className="form-input" defaultValue={selectedProduct.saleStatus} style={{ appearance: 'auto', backgroundColor: 'var(--bg-secondary)' }}>
                    <option value="Satışta">Satışta</option>
                    <option value="Kapalı">Kapalı</option>
                  </select>
                </div>
              </div>
            </div>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => setShowEditProductModal(false)}>İptal</button>
              <button className="btn btn-primary" style={{ gap: 8 }} onClick={() => handleMockAction(setShowEditProductModal)}>
                {actionSaved ? <><Check size={16} /> Güncellendi</> : 'Değişiklikleri Kaydet'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Delete Product Modal */}
      {showDeleteProductModal && selectedProduct && (
        <div className="modal-overlay" onClick={() => setShowDeleteProductModal(false)}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 400, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8, color: '#f87171' }}>
                <Trash2 size={20} /> Ürünü Sil
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowDeleteProductModal(false)}>
                <X size={18} />
              </button>
            </div>
            <p style={{ color: 'var(--text-muted)', marginBottom: 24, lineHeight: 1.5 }}>
              <strong style={{ color: 'var(--text-primary)' }}>{selectedProduct.name}</strong> ürününü silmek istediğinize emin misiniz? Bu işlem geri alınamaz ve tüm stok hareketlerini etkileyebilir.
            </p>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => setShowDeleteProductModal(false)}>Vazgeç</button>
              <button className="btn" style={{ background: 'rgba(248,113,113,0.15)', color: '#f87171', border: '1px solid rgba(248,113,113,0.3)' }} onClick={() => handleMockAction(setShowDeleteProductModal)}>
                {actionSaved ? <><Check size={16} /> Silindi</> : 'Evet, Ürünü Sil'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
