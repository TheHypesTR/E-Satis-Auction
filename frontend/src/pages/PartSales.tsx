/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import { Check, Puzzle, Plus, Search, X } from 'lucide-react';
import { adminApi } from '../services/adminApi';
import { getApiErrorMessage, unitOfMeasureLabel } from '../services/apiUtils';
import type { FacilityDto, ItemSummaryDto, PartSaleOperationDto, ProductSummaryDto } from '../types/admin';

export default function PartSales() {
  const [operations, setOperations] = useState<PartSaleOperationDto[]>([]);
  const [items, setItems] = useState<ItemSummaryDto[]>([]);
  const [products, setProducts] = useState<ProductSummaryDto[]>([]);
  const [facilities, setFacilities] = useState<FacilityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [search, setSearch] = useState('');
  const [showAdd, setShowAdd] = useState(false);
  const [form, setForm] = useState({ sourceItemId: '', productId: '', facilityId: '', quantity: '1', unitOfMeasure: '1', notes: '' });

  const load = () => { setLoading(true); setError(''); Promise.all([adminApi.partSaleOperations({ pageSize: 100 }), adminApi.items({ pageSize: 100 }), adminApi.products({ pageSize: 100 }), adminApi.facilities({ pageSize: 100 })]).then(([ops, itemData, productData, facilityData]) => { setOperations(ops); setItems(itemData); setProducts(productData); setFacilities(facilityData); }).catch(err => setError(getApiErrorMessage(err, 'Parça satış verileri yüklenemedi.'))).finally(() => setLoading(false)); };
  useEffect(load, []);

  const filtered = operations.filter(op => op.sourceItemId.toLowerCase().includes(search.toLowerCase()) || op.createdPartItemId.toLowerCase().includes(search.toLowerCase()));
  const create = async () => {
    if (!form.sourceItemId || !form.productId || !form.facilityId || Number(form.quantity) <= 0) return;
    setSaving(true); setError(''); setMessage('');
    try {
      await adminApi.createPartSaleOperation({ sourceItemId: form.sourceItemId, productId: form.productId, quantity: Number(form.quantity), facilityId: form.facilityId, unitOfMeasure: Number(form.unitOfMeasure), dynamicAttributes: {}, notes: form.notes || null });
      setMessage('Part-sale operation oluşturuldu. Backend CreatedPartItem üzerinde SourceItemId bağlantısını korur.');
      setShowAdd(false); load();
    } catch (err) { setError(getApiErrorMessage(err, 'Part-sale operation oluşturulamadı.')); }
    finally { setSaving(false); }
  };

  return <div><div className="page-header"><div><h1 className="page-title">Parça Satış Operasyonları</h1><p className="page-subtitle">Mevcut Item üzerinden SourceItemId bağlantısıyla parça item oluşturma altyapısı.</p></div><button className="btn btn-primary" onClick={() => setShowAdd(true)}><Plus size={16}/> Yeni Part Sale</button></div>{error && <div className="error-banner" style={{ marginBottom: 16 }}>{error}</div>}{message && <div style={{ color: '#6ee7b7', marginBottom: 16 }}>{message}</div>}<div className="data-table-wrapper"><div className="data-table-header"><div className="search-bar" style={{ minWidth: 300 }}><Search size={15}/><input placeholder="Source veya part item id ara..." value={search} onChange={e => setSearch(e.target.value)} /></div></div>{loading ? <Empty text="Part-sale operasyonları yükleniyor..." /> : filtered.length === 0 ? <Empty text="Part-sale operation bulunamadı." /> : <table className="data-table"><thead><tr><th>SourceItemId</th><th>CreatedPartItemId</th><th>ProductId</th><th>FacilityId</th><th>Miktar</th><th>Durum</th><th>Tarih</th></tr></thead><tbody>{filtered.map(op => <tr key={op.id}><td><code>{op.sourceItemId}</code></td><td><code>{op.createdPartItemId}</code></td><td><code>{op.productId}</code></td><td><code>{op.facilityId}</code></td><td>{op.quantity} {unitOfMeasureLabel[String(op.unitOfMeasure)] ?? String(op.unitOfMeasure)}</td><td><span className="badge badge-green">{String(op.status) === '1' ? 'Completed' : String(op.status)}</span></td><td>{new Date(op.createdAt).toLocaleString('tr-TR')}</td></tr>)}</tbody></table>}</div>{showAdd && <div className="modal-overlay" onClick={() => setShowAdd(false)}><div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 640 }}><Header title="Yeni Part Sale Operation" close={() => setShowAdd(false)} /><Select label="Source Item" value={form.sourceItemId} onChange={v => setForm(p => ({ ...p, sourceItemId: v }))} options={items.map(i => ({ value: i.id, label: `${i.displayName} (${i.facilityName})` }))} /><Select label="Part Product" value={form.productId} onChange={v => setForm(p => ({ ...p, productId: v }))} options={products.filter(p => p.isActive).map(p => ({ value: p.id, label: `${p.name} (${p.sku})` }))} /><Select label="Facility" value={form.facilityId} onChange={v => setForm(p => ({ ...p, facilityId: v }))} options={facilities.map(f => ({ value: f.id, label: `${f.name} - ${f.city}` }))} /><Input label="Miktar" value={form.quantity} onChange={v => setForm(p => ({ ...p, quantity: v }))} /><Select label="Birim" value={form.unitOfMeasure} onChange={v => setForm(p => ({ ...p, unitOfMeasure: v }))} options={[{ value: '1', label: 'Adet' }, { value: '2', label: 'Kg' }, { value: '3', label: 'Litre' }, { value: '4', label: 'Kutu' }, { value: '5', label: 'Paket' }, { value: '6', label: 'Palet' }]} /><div className="form-group"><label className="form-label">Not</label><textarea className="form-input" rows={3} value={form.notes} onChange={e => setForm(p => ({ ...p, notes: e.target.value }))} /></div><p style={{ color: 'var(--text-muted)', fontSize: '0.8rem' }}>Backend source item tüketimi ve created part item oluşturmayı transaction içinde yürütür; frontend stok hesabı yapmaz.</p><div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12, marginTop: 20 }}><button className="btn btn-ghost" onClick={() => setShowAdd(false)}>İptal</button><button className="btn btn-primary" disabled={saving} onClick={create}><Check size={16}/> Oluştur</button></div></div></div>}</div>;
}
function Empty({ text }: { text: string }) { return <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}><Puzzle size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>; }
function Header({ title, close }: { title: string; close: () => void }) { return <div style={{ display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:20 }}><h2>{title}</h2><button className="btn btn-ghost" style={{ padding:4 }} onClick={close}><X size={18}/></button></div>; }
function Input({ label, value, onChange }: { label: string; value: string; onChange: (v:string)=>void }) { return <div className="form-group"><label className="form-label">{label}</label><input type="number" className="form-input" value={value} onChange={e=>onChange(e.target.value)}/></div>; }
function Select({ label, value, onChange, options }: { label: string; value: string; onChange: (v:string)=>void; options: Array<{value:string; label:string}> }) { return <div className="form-group"><label className="form-label">{label}</label><select className="form-input" style={{ appearance:'auto' }} value={value} onChange={e=>onChange(e.target.value)}><option value="">Seçiniz</option>{options.map(o=><option key={o.value} value={o.value}>{o.label}</option>)}</select></div>; }
