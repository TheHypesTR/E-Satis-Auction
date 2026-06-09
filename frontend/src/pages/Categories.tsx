/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import type { Dispatch, SetStateAction } from 'react';
import { Check, CheckCircle2, ChevronRight, Layers, Plus, Search, Tag, Trash2, X, XCircle } from 'lucide-react';
import { adminApi } from '../services/adminApi';
import { attributeDataTypeLabel, attributeTargetLabel, getApiErrorMessage } from '../services/apiUtils';
import type { CategoryAttributeDto, CategoryDetailDto } from '../types/admin';
import type { CategoryDto } from '../types/commerce';

const emptyAttribute = { name: '', code: '', dataType: '1', target: '1', isRequired: false };
const typeOptions: Array<[string, string]> = [['1', 'Metin'], ['2', 'Sayı'], ['3', 'Tarih'], ['4', 'Boolean'], ['5', 'Seçim listesi']];
const targetOptions: Array<[string, string]> = [['1', 'ProductLevel'], ['2', 'ItemLevel']];

export default function Categories() {
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [selected, setSelected] = useState<CategoryDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [search, setSearch] = useState('');
  const [filterActive, setFilterActive] = useState<boolean | null>(null);
  const [showAdd, setShowAdd] = useState(false);
  const [categoryForm, setCategoryForm] = useState({ name: '', description: '', isActive: true });
  const [attributeForm, setAttributeForm] = useState(emptyAttribute);
  const [editingAttributeId, setEditingAttributeId] = useState<string | null>(null);
  const [optionValue, setOptionValue] = useState<Record<string, string>>({});

  const load = () => {
    setLoading(true); setError('');
    adminApi.categories({ pageSize: 100 })
      .then(setCategories)
      .catch(err => setError(getApiErrorMessage(err, 'Kategoriler yüklenemedi.')))
      .finally(() => setLoading(false));
  };

  useEffect(load, []);

  const open = async (id: string) => {
    setError(''); setMessage('');
    try {
      const detail = await adminApi.category(id);
      setSelected(detail);
      setCategoryForm({ name: detail.name, description: detail.description ?? '', isActive: detail.isActive ?? false });
      setAttributeForm(emptyAttribute);
      setEditingAttributeId(null);
    } catch (err) {
      setError(getApiErrorMessage(err, 'Kategori detayı alınamadı.'));
    }
  };

  const create = async () => {
    if (!categoryForm.name.trim()) return;
    setSaving(true); setError(''); setMessage('');
    try {
      await adminApi.createCategory({ name: categoryForm.name.trim(), description: categoryForm.description || null, isActive: categoryForm.isActive, attributes: [] });
      setMessage('Kategori oluşturuldu. Attribute şeması Product/Item dynamic attributes altyapısının temelidir.');
      setShowAdd(false); setCategoryForm({ name: '', description: '', isActive: true }); load();
    } catch (err) { setError(getApiErrorMessage(err, 'Kategori oluşturulamadı.')); }
    finally { setSaving(false); }
  };

  const updateCategory = async () => {
    if (!selected || !categoryForm.name.trim()) return;
    setSaving(true); setError(''); setMessage('');
    try {
      await adminApi.updateCategory(selected.id, { name: categoryForm.name.trim(), description: categoryForm.description || null });
      setMessage('Kategori temel bilgileri güncellendi.');
      await open(selected.id); load();
    } catch (err) { setError(getApiErrorMessage(err, 'Kategori güncellenemedi.')); }
    finally { setSaving(false); }
  };

  const toggleCategory = async (category: CategoryDto | CategoryDetailDto) => {
    setSaving(true); setError(''); setMessage('');
    try {
      if (category.isActive) await adminApi.deactivateCategory(category.id); else await adminApi.activateCategory(category.id);
      setMessage(category.isActive ? 'Kategori pasifleştirildi. Attribute/option şema değişiklikleri yalnız pasif kategoride yapılabilir.' : 'Kategori aktifleştirildi.');
      if (selected?.id === category.id) await open(category.id);
      load();
    } catch (err) { setError(getApiErrorMessage(err, 'Kategori durumu değiştirilemedi.')); }
    finally { setSaving(false); }
  };

  const saveAttribute = async () => {
    if (!selected || !attributeForm.name.trim() || !attributeForm.code.trim()) return;
    setSaving(true); setError(''); setMessage('');
    const payload = { name: attributeForm.name.trim(), code: attributeForm.code.trim(), dataType: Number(attributeForm.dataType), target: Number(attributeForm.target), isRequired: attributeForm.isRequired };
    try {
      if (editingAttributeId) await adminApi.updateCategoryAttribute(selected.id, editingAttributeId, payload);
      else await adminApi.addCategoryAttribute(selected.id, payload);
      setMessage(editingAttributeId ? 'Attribute güncellendi.' : 'Attribute eklendi. ProductLevel ürün baseAttributes, ItemLevel item dynamicAttributes için kullanılır.');
      setAttributeForm(emptyAttribute); setEditingAttributeId(null); await open(selected.id); load();
    } catch (err) { setError(getApiErrorMessage(err, 'Attribute işlemi başarısız. Kategori aktifse backend bu işlemi engeller.')); }
    finally { setSaving(false); }
  };

  const editAttribute = (attr: CategoryAttributeDto) => {
    setEditingAttributeId(attr.id);
    setAttributeForm({ name: attr.name, code: attr.code, dataType: String(attr.dataType), target: String(attr.target), isRequired: attr.isRequired });
  };

  const deleteAttribute = async (attributeId: string) => {
    if (!selected || !confirm('Attribute soft-delete edilecek. Devam edilsin mi?')) return;
    setSaving(true); setError('');
    try { await adminApi.deleteCategoryAttribute(selected.id, attributeId); setMessage('Attribute silindi.'); await open(selected.id); load(); }
    catch (err) { setError(getApiErrorMessage(err, 'Attribute silinemedi.')); }
    finally { setSaving(false); }
  };

  const addOption = async (attributeId: string) => {
    if (!selected || !optionValue[attributeId]?.trim()) return;
    setSaving(true); setError('');
    try { await adminApi.addCategoryAttributeOption(selected.id, attributeId, optionValue[attributeId].trim()); setOptionValue(p => ({ ...p, [attributeId]: '' })); setMessage('Seçim opsiyonu eklendi.'); await open(selected.id); }
    catch (err) { setError(getApiErrorMessage(err, 'Option eklenemedi. Sadece SelectList attribute için geçerlidir.')); }
    finally { setSaving(false); }
  };

  const updateOption = async (attributeId: string, optionId: string, currentValue: string) => {
    if (!selected) return;
    const next = prompt('Yeni option değeri', currentValue);
    if (!next?.trim()) return;
    setSaving(true); setError('');
    try { await adminApi.updateCategoryAttributeOption(selected.id, attributeId, optionId, next.trim()); setMessage('Option güncellendi.'); await open(selected.id); }
    catch (err) { setError(getApiErrorMessage(err, 'Option güncellenemedi.')); }
    finally { setSaving(false); }
  };

  const deleteOption = async (attributeId: string, optionId: string) => {
    if (!selected || !confirm('Option soft-delete edilecek. Devam edilsin mi?')) return;
    setSaving(true); setError('');
    try { await adminApi.deleteCategoryAttributeOption(selected.id, attributeId, optionId); setMessage('Option silindi.'); await open(selected.id); }
    catch (err) { setError(getApiErrorMessage(err, 'Option silinemedi.')); }
    finally { setSaving(false); }
  };

  const filtered = categories.filter(c => {
    const bySearch = c.name.toLowerCase().includes(search.toLowerCase());
    const byActive = filterActive === null ? true : c.isActive === filterActive;
    return bySearch && byActive;
  });

  return <div><div className="page-header"><div><h1 className="page-title">Kategoriler</h1><p className="page-subtitle">Category dynamic attribute altyapısının temelidir; ProductLevel ve ItemLevel ayrımı backend şemasına göre tutulur.</p></div><button className="btn btn-primary" onClick={() => { setCategoryForm({ name: '', description: '', isActive: true }); setShowAdd(true); }}><Plus size={16} /> Yeni Kategori</button></div>{error && <div className="error-banner" style={{ marginBottom: 16 }}>{error}</div>}{message && <div style={{ color: '#6ee7b7', marginBottom: 16 }}>{message}</div>}<div style={{ display: 'flex', gap: 12, marginBottom: 24, flexWrap: 'wrap' }}><div className="search-bar" style={{ minWidth: 280 }}><Search size={15} /><input placeholder="Kategori ara..." value={search} onChange={e => setSearch(e.target.value)} /></div>{([null, true, false] as const).map(value => <button key={String(value)} className="btn btn-ghost" onClick={() => setFilterActive(value)}>{value === null ? 'Tümü' : value ? 'Aktif' : 'Pasif'}</button>)}</div>{loading ? <Empty text="Kategoriler yükleniyor..." /> : filtered.length === 0 ? <Empty text="Kategori bulunamadı." /> : <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>{filtered.map(cat => <div key={cat.id} className="glass-card" style={{ padding: 20 }}><div style={{ display: 'flex', alignItems: 'center', gap: 14 }}><Tag size={18} color="#a78bfa" /><div style={{ flex: 1 }}><strong>{cat.name}</strong><div style={{ color: 'var(--text-muted)', fontSize: '0.82rem' }}>{cat.description || 'Açıklama yok'} · {cat.attributes?.length ?? 0} attribute</div></div>{cat.isActive ? <span className="badge badge-green"><CheckCircle2 size={11} /> Aktif</span> : <span className="badge badge-amber"><XCircle size={11} /> Pasif</span>}<button className="btn btn-ghost" onClick={() => open(cat.id)}>Detay <ChevronRight size={14} /></button><button className="btn btn-ghost" disabled={saving} onClick={() => toggleCategory(cat)}>{cat.isActive ? 'Deaktif Et' : 'Aktif Et'}</button></div></div>)}</div>}{showAdd && <CategoryModal title="Yeni Kategori" saving={saving} form={categoryForm} setForm={setCategoryForm} close={() => setShowAdd(false)} submit={create} />}{selected && <div className="modal-overlay" onClick={() => setSelected(null)}><div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 900, width: '100%', maxHeight: '90vh', overflowY: 'auto' }}><Header title="Kategori Detayı" close={() => setSelected(null)} /><div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 18, marginBottom: 20 }}><div className="glass-card" style={{ padding: 16 }}><h3 style={{ marginBottom: 12 }}>Temel Bilgi</h3><Input label="Kategori adı" value={categoryForm.name} onChange={v => setCategoryForm(p => ({ ...p, name: v }))} /><Input label="Açıklama" value={categoryForm.description} onChange={v => setCategoryForm(p => ({ ...p, description: v }))} /><div style={{ display: 'flex', gap: 10, marginTop: 12 }}><button className="btn btn-primary" disabled={saving} onClick={updateCategory}><Check size={16} /> Güncelle</button><button className="btn btn-ghost" disabled={saving} onClick={() => toggleCategory(selected)}>{selected.isActive ? 'Deaktif Et' : 'Aktif Et'}</button></div></div><div className="glass-card" style={{ padding: 16 }}><h3 style={{ marginBottom: 12 }}>{editingAttributeId ? 'Attribute Güncelle' : 'Attribute Ekle'}</h3><Input label="Ad" value={attributeForm.name} onChange={v => setAttributeForm(p => ({ ...p, name: v }))} /><Input label="Kod" value={attributeForm.code} onChange={v => setAttributeForm(p => ({ ...p, code: v.toLowerCase().replace(/\s+/g, '_') }))} /><Select label="Tip" value={attributeForm.dataType} onChange={v => setAttributeForm(p => ({ ...p, dataType: v }))} options={typeOptions} /><Select label="Target" value={attributeForm.target} onChange={v => setAttributeForm(p => ({ ...p, target: v }))} options={targetOptions} /><label style={{ display: 'flex', gap: 8, alignItems: 'center', marginTop: 8 }}><input type="checkbox" checked={attributeForm.isRequired} onChange={e => setAttributeForm(p => ({ ...p, isRequired: e.target.checked }))} /> Zorunlu</label><div style={{ display: 'flex', gap: 10, marginTop: 12 }}><button className="btn btn-primary" disabled={saving} onClick={saveAttribute}><Plus size={16} /> {editingAttributeId ? 'Güncelle' : 'Ekle'}</button>{editingAttributeId && <button className="btn btn-ghost" onClick={() => { setEditingAttributeId(null); setAttributeForm(emptyAttribute); }}>Vazgeç</button>}</div><p style={{ color: 'var(--text-muted)', fontSize: '0.78rem', marginTop: 10 }}>Backend kuralı: Attribute/option schema değişiklikleri aktif kategoride reddedilebilir.</p></div></div><div style={{ display: 'grid', gap: 12 }}>{selected.attributes.map(attr => <div key={attr.id} className="glass-card" style={{ padding: 16 }}><div style={{ display: 'flex', alignItems: 'center', gap: 10, flexWrap: 'wrap' }}><Layers size={15} /><strong>{attr.name}</strong><code style={{ color: '#a78bfa' }}>{attr.code}</code><span className="badge badge-blue">{attributeDataTypeLabel[String(attr.dataType)] ?? String(attr.dataType)}</span><span className="badge badge-purple">{attributeTargetLabel[String(attr.target)] ?? String(attr.target)}</span>{attr.isRequired ? <span className="badge badge-green">Zorunlu</span> : <span className="badge badge-amber">Opsiyonel</span>}<div style={{ marginLeft: 'auto', display: 'flex', gap: 8 }}><button className="btn btn-ghost" disabled={saving} onClick={() => editAttribute(attr)}>Düzenle</button><button className="btn btn-ghost" disabled={saving} style={{ color: '#f87171' }} onClick={() => deleteAttribute(attr.id)}><Trash2 size={14} /> Sil</button></div></div>{String(attr.dataType) === '5' && <div style={{ marginTop: 14 }}><div style={{ display: 'flex', gap: 8, marginBottom: 10 }}><input className="form-input" placeholder="Yeni option" value={optionValue[attr.id] ?? ''} onChange={e => setOptionValue(p => ({ ...p, [attr.id]: e.target.value }))} /><button className="btn btn-primary" disabled={saving} onClick={() => addOption(attr.id)}>Option Ekle</button></div><div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>{attr.options.map(opt => <span key={opt.id} className="badge badge-blue" style={{ gap: 8 }}>{opt.value}<button className="btn btn-ghost" style={{ padding: '2px 6px' }} disabled={saving} onClick={() => updateOption(attr.id, opt.id, opt.value)}>Düzenle</button><button className="btn btn-ghost" style={{ padding: '2px 6px', color: '#f87171' }} disabled={saving} onClick={() => deleteOption(attr.id, opt.id)}>Sil</button></span>)}</div></div>}</div>)}</div></div></div>}</div>;
}

function CategoryModal({ title, saving, form, setForm, close, submit }: { title: string; saving: boolean; form: { name: string; description: string; isActive: boolean }; setForm: Dispatch<SetStateAction<{ name: string; description: string; isActive: boolean }>>; close: () => void; submit: () => void }) { return <div className="modal-overlay" onClick={close}><div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 520 }}><Header title={title} close={close} /><Input label="Kategori adı" value={form.name} onChange={v => setForm(p => ({ ...p, name: v }))} /><Input label="Açıklama" value={form.description} onChange={v => setForm(p => ({ ...p, description: v }))} /><label style={{ display: 'flex', gap: 8, alignItems: 'center', marginTop: 12 }}><input type="checkbox" checked={form.isActive} onChange={e => setForm(p => ({ ...p, isActive: e.target.checked }))} /> Aktif oluştur</label><div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12, marginTop: 24 }}><button className="btn btn-ghost" onClick={close}>İptal</button><button className="btn btn-primary" disabled={saving} onClick={submit}><Check size={16} /> Kaydet</button></div></div></div>; }
function Empty({ text }: { text: string }) { return <div style={{ textAlign: 'center', padding: 70, color: 'var(--text-muted)' }}><Tag size={34} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>; }
function Header({ title, close }: { title: string; close: () => void }) { return <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}><h2>{title}</h2><button className="btn btn-ghost" style={{ padding: 4 }} onClick={close}><X size={18} /></button></div>; }
function Input({ label, value, onChange }: { label: string; value: string; onChange: (v: string) => void }) { return <div className="form-group" style={{ marginBottom: 10 }}><label className="form-label">{label}</label><input className="form-input" value={value} onChange={e => onChange(e.target.value)} /></div>; }
function Select({ label, value, onChange, options }: { label: string; value: string; onChange: (v: string) => void; options: Array<[string, string]> }) { return <div className="form-group" style={{ marginBottom: 10 }}><label className="form-label">{label}</label><select className="form-input" style={{ appearance: 'auto' }} value={value} onChange={e => onChange(e.target.value)}>{options.map(([valueKey, labelText]) => <option key={valueKey} value={valueKey}>{labelText}</option>)}</select></div>; }
