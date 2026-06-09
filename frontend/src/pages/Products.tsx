/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import { Check, Package, Plus, Power, Search, X } from 'lucide-react';
import { adminApi } from '../services/adminApi';
import { getApiErrorMessage } from '../services/apiUtils';
import type { CategoryDto } from '../types/commerce';
import type { ProductSummaryDto } from '../types/admin';

export default function Products() {
  const [products, setProducts] = useState<ProductSummaryDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [search, setSearch] = useState('');
  const [showAdd, setShowAdd] = useState(false);
  const [form, setForm] = useState({ sku: '', barcode: '', name: '', categoryId: '', unitOfMeasure: '1' });

  const load = () => {
    setLoading(true); setError('');
    Promise.all([adminApi.products({ pageSize: 100 }), adminApi.categories({ pageSize: 100 })])
      .then(([productData, categoryData]) => { setProducts(productData); setCategories(categoryData); })
      .catch(err => setError(getApiErrorMessage(err, 'Ürünler yüklenemedi.')))
      .finally(() => setLoading(false));
  };

  useEffect(load, []);

  const filtered = products.filter(p => p.name.toLowerCase().includes(search.toLowerCase()) || p.sku.toLowerCase().includes(search.toLowerCase()));

  const create = async () => {
    if (!form.sku || !form.name || !form.categoryId) return;
    setSaving(true); setError(''); setMessage('');
    try {
      await adminApi.createProduct({ sku: form.sku, barcode: form.barcode || null, name: form.name, categoryId: form.categoryId, unitOfMeasure: Number(form.unitOfMeasure), baseAttributes: {} });
      setMessage('Product master kaydı oluşturuldu. Fiziksel stok için Inventory/Item sayfasından item eklenmelidir.');
      setShowAdd(false); setForm({ sku: '', barcode: '', name: '', categoryId: '', unitOfMeasure: '1' }); load();
    } catch (err) { setError(getApiErrorMessage(err, 'Ürün oluşturulamadı.')); }
    finally { setSaving(false); }
  };

  const toggleActive = async (product: ProductSummaryDto) => {
    setSaving(true); setError(''); setMessage('');
    try {
      if (product.isActive) await adminApi.deactivateProduct(product.id); else await adminApi.activateProduct(product.id);
      setMessage(product.isActive ? 'Ürün katalogda pasifleştirildi.' : 'Ürün katalogda aktifleştirildi.');
      load();
    } catch (err) { setError(getApiErrorMessage(err, 'Ürün durumu güncellenemedi.')); }
    finally { setSaving(false); }
  };

  return <div><div className="page-header"><div><h1 className="page-title">Ürünler</h1><p className="page-subtitle">Product katalog/master veridir; fiziksel stok değildir.</p></div><button className="btn btn-primary" onClick={() => setShowAdd(true)}><Plus size={16} /> Yeni Product</button></div>{error && <div className="error-banner" style={{ marginBottom: 16 }}>{error}</div>}{message && <div style={{ marginBottom: 16, color: '#6ee7b7' }}>{message}</div>}<div className="data-table-wrapper animate-fade-up"><div className="data-table-header"><div className="search-bar" style={{ minWidth: 300 }}><Search size={15} /><input placeholder="Ürün adı veya SKU ara..." value={search} onChange={e => setSearch(e.target.value)} /></div></div>{loading ? <Empty text="Ürünler yükleniyor..." /> : filtered.length === 0 ? <Empty text="Ürün bulunamadı." /> : <table className="data-table"><thead><tr><th>SKU</th><th>Ürün</th><th>Kategori</th><th>Birim</th><th>Durum</th><th style={{ textAlign: 'right' }}>İşlem</th></tr></thead><tbody>{filtered.map(p => <tr key={p.id}><td><code style={{ color: '#a78bfa' }}>{p.sku}</code></td><td><strong>{p.name}</strong><div style={{ color: 'var(--text-muted)', fontSize: '0.78rem' }}>{p.barcode || 'Barkod yok'}</div></td><td>{p.categoryName}</td><td>{String(p.unitOfMeasure)}</td><td><span className={`badge badge-${p.isActive ? 'green' : 'amber'}`}>{p.isActive ? 'Aktif' : 'Pasif'}</span></td><td style={{ textAlign: 'right' }}><button className="btn btn-ghost" disabled={saving} onClick={() => toggleActive(p)}><Power size={14} /> {p.isActive ? 'Pasifleştir' : 'Aktifleştir'}</button></td></tr>)}</tbody></table>}</div>{showAdd && <div className="modal-overlay" onClick={() => setShowAdd(false)}><div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 520 }}><ModalHeader title="Yeni Product" onClose={() => setShowAdd(false)} /><div style={{ display: 'grid', gap: 14 }}><Input label="SKU" value={form.sku} onChange={v => setForm(p => ({ ...p, sku: v }))} /><Input label="Ürün adı" value={form.name} onChange={v => setForm(p => ({ ...p, name: v }))} /><Input label="Barkod" value={form.barcode} onChange={v => setForm(p => ({ ...p, barcode: v }))} /><div className="form-group"><label className="form-label">Kategori</label><select className="form-input" style={{ appearance: 'auto' }} value={form.categoryId} onChange={e => setForm(p => ({ ...p, categoryId: e.target.value }))}><option value="">Seçiniz</option>{categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}</select></div><div className="form-group"><label className="form-label">Birim</label><select className="form-input" style={{ appearance: 'auto' }} value={form.unitOfMeasure} onChange={e => setForm(p => ({ ...p, unitOfMeasure: e.target.value }))}><option value="1">Piece</option><option value="2">Kg</option><option value="3">Liter</option><option value="4">Box</option><option value="5">Pack</option><option value="6">Pallet</option></select></div></div><div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12, marginTop: 24 }}><button className="btn btn-ghost" onClick={() => setShowAdd(false)}>İptal</button><button className="btn btn-primary" disabled={saving} onClick={create}><Check size={16} /> Kaydet</button></div></div></div>}</div>;
}

function Empty({ text }: { text: string }) { return <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}><Package size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>; }
function Input({ label, value, onChange }: { label: string; value: string; onChange: (v: string) => void }) { return <div className="form-group"><label className="form-label">{label}</label><input className="form-input" value={value} onChange={e => onChange(e.target.value)} /></div>; }
function ModalHeader({ title, onClose }: { title: string; onClose: () => void }) { return <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}><h2 style={{ fontSize: '1.2rem', fontWeight: 700 }}>{title}</h2><button className="btn btn-ghost" style={{ padding: 4 }} onClick={onClose}><X size={18} /></button></div>; }
