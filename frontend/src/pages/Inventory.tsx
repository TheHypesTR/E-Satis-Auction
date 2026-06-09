/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import { Check, Layers, Plus, Search, X } from 'lucide-react';
import { adminApi } from '../services/adminApi';
import { getApiErrorMessage, itemStatusLabel } from '../services/apiUtils';
import type { FacilityDto, ItemSummaryDto, ProductSummaryDto } from '../types/admin';

export default function Inventory() {
  const [items, setItems] = useState<ItemSummaryDto[]>([]);
  const [products, setProducts] = useState<ProductSummaryDto[]>([]);
  const [facilities, setFacilities] = useState<FacilityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [search, setSearch] = useState('');
  const [showAdd, setShowAdd] = useState(false);
  const [form, setForm] = useState({ productId: '', facilityId: '', quantity: '1', unitOfMeasure: '1', status: '1' });

  const load = () => { setLoading(true); setError(''); Promise.all([adminApi.items({ pageSize: 100 }), adminApi.products({ pageSize: 100 }), adminApi.facilities({ pageSize: 100 })]).then(([i, p, f]) => { setItems(i); setProducts(p); setFacilities(f); }).catch(err => setError(getApiErrorMessage(err, 'Envanter yüklenemedi.'))).finally(() => setLoading(false)); };
  useEffect(load, []);

  const filtered = items.filter(i => i.displayName.toLowerCase().includes(search.toLowerCase()) || i.facilityName.toLowerCase().includes(search.toLowerCase()) || (i.productName ?? '').toLowerCase().includes(search.toLowerCase()));
  const add = async () => {
    if (!form.productId || !form.facilityId || Number(form.quantity) <= 0) return;
    setSaving(true); setError(''); setMessage('');
    try { await adminApi.addStandardizedItem({ productId: form.productId, facilityId: form.facilityId, quantity: Number(form.quantity), unitOfMeasure: Number(form.unitOfMeasure), status: Number(form.status), dynamicAttributes: {} }); setMessage('Standardized Item stoğa eklendi.'); setShowAdd(false); load(); }
    catch (err) { setError(getApiErrorMessage(err, 'Stok eklenemedi.')); }
    finally { setSaving(false); }
  };

  return <div><div className="page-header"><div><h1 className="page-title">Envanter / Item</h1><p className="page-subtitle">Item fiziksel stok gerçekliğidir; Product veya Listing değildir.</p></div><button className="btn btn-primary" onClick={() => setShowAdd(true)}><Plus size={16} /> Standardized Item Ekle</button></div>{error && <div className="error-banner" style={{ marginBottom: 16 }}>{error}</div>}{message && <div style={{ marginBottom: 16, color: '#6ee7b7' }}>{message}</div>}<div className="data-table-wrapper animate-fade-up"><div className="data-table-header"><div className="search-bar" style={{ minWidth: 300 }}><Search size={15} /><input placeholder="Item, product veya tesis ara..." value={search} onChange={e => setSearch(e.target.value)} /></div></div>{loading ? <Empty text="Item listesi yükleniyor..." /> : filtered.length === 0 ? <Empty text="Item bulunamadı." /> : <table className="data-table"><thead><tr><th>Item</th><th>Mode</th><th>Durum</th><th>Miktar</th><th>Tesis</th><th>Product</th></tr></thead><tbody>{filtered.map(item => <tr key={item.id}><td><strong>{item.displayName}</strong><div style={{ color: 'var(--text-muted)', fontSize: '0.75rem' }}>{item.categoryName}</div></td><td>{String(item.mode)}</td><td><span className="badge badge-purple">{itemStatusLabel[String(item.status)] ?? String(item.status)}</span></td><td>{item.quantity} {String(item.unitOfMeasure)}</td><td>{item.facilityName}</td><td>{item.productName ?? 'AdHoc'}</td></tr>)}</tbody></table>}</div>{showAdd && <div className="modal-overlay" onClick={() => setShowAdd(false)}><div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 520 }}><div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}><h2>Standardized Item Ekle</h2><button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowAdd(false)}><X size={18} /></button></div><div style={{ display: 'grid', gap: 14 }}><Select label="Product" value={form.productId} onChange={v => setForm(p => ({ ...p, productId: v }))} options={products.filter(p => p.isActive).map(p => ({ value: p.id, label: `${p.name} (${p.sku})` }))} /><Select label="Facility" value={form.facilityId} onChange={v => setForm(p => ({ ...p, facilityId: v }))} options={facilities.map(f => ({ value: f.id, label: `${f.name} - ${f.city}` }))} /><Input label="Miktar" value={form.quantity} onChange={v => setForm(p => ({ ...p, quantity: v }))} /><Select label="Birim" value={form.unitOfMeasure} onChange={v => setForm(p => ({ ...p, unitOfMeasure: v }))} options={[{ value: '1', label: 'Piece' }, { value: '2', label: 'Kg' }, { value: '3', label: 'Liter' }, { value: '4', label: 'Box' }, { value: '5', label: 'Pack' }, { value: '6', label: 'Pallet' }]} /><Select label="Başlangıç Durumu" value={form.status} onChange={v => setForm(p => ({ ...p, status: v }))} options={[{ value: '1', label: 'Available' }, { value: '4', label: 'Damaged' }]} /></div><p style={{ color: 'var(--text-muted)', fontSize: '0.8rem', marginTop: 14 }}>Backend’de genel Item update/status-change endpoint’i yok; operasyonel stok hareketleri domain akışlarından gelir.</p><div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12, marginTop: 24 }}><button className="btn btn-ghost" onClick={() => setShowAdd(false)}>İptal</button><button className="btn btn-primary" disabled={saving} onClick={add}><Check size={16} /> Stok Ekle</button></div></div></div>}</div>;
}
function Empty({ text }: { text: string }) { return <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}><Layers size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>; }
function Input({ label, value, onChange }: { label: string; value: string; onChange: (v: string) => void }) { return <div className="form-group"><label className="form-label">{label}</label><input type="number" className="form-input" value={value} onChange={e => onChange(e.target.value)} /></div>; }
function Select({ label, value, onChange, options }: { label: string; value: string; onChange: (v: string) => void; options: Array<{ value: string; label: string }> }) { return <div className="form-group"><label className="form-label">{label}</label><select className="form-input" style={{ appearance: 'auto' }} value={value} onChange={e => onChange(e.target.value)}><option value="">Seçiniz</option>{options.map(o => <option key={o.value} value={o.value}>{o.label}</option>)}</select></div>; }
