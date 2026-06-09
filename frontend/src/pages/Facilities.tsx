/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import { Building2, Check, Eye, Plus, Search, Trash2, X } from 'lucide-react';
import { adminApi } from '../services/adminApi';
import { getApiErrorMessage } from '../services/apiUtils';
import type { FacilityDetailDto, FacilityDto } from '../types/admin';

export default function Facilities() {
  const [facilities, setFacilities] = useState<FacilityDto[]>([]);
  const [selected, setSelected] = useState<FacilityDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [search, setSearch] = useState('');
  const [showAdd, setShowAdd] = useState(false);
  const [form, setForm] = useState({ name: '', description: '', capacityM3: '100', criticalThresholdM3: '20', addressTitle: 'Depo', city: '', district: '', openAddress: '', latitude: '0', longitude: '0' });
  const [managerForm, setManagerForm] = useState({ email: '', firstName: '', lastName: '', isPrimary: false });

  const load = () => { setLoading(true); setError(''); adminApi.facilities({ pageSize: 100 }).then(setFacilities).catch(err => setError(getApiErrorMessage(err, 'Tesisler yüklenemedi.'))).finally(() => setLoading(false)); };
  useEffect(load, []);
  const filtered = facilities.filter(f => f.name.toLowerCase().includes(search.toLowerCase()) || f.city.toLowerCase().includes(search.toLowerCase()));

  const open = async (id: string) => { setError(''); try { setSelected(await adminApi.facility(id)); } catch (err) { setError(getApiErrorMessage(err, 'Tesis detayı alınamadı.')); } };

  const create = async () => {
    if (!form.name || !form.city || !form.district || !form.openAddress) return;
    setSaving(true); setError(''); setMessage('');
    try {
      await adminApi.createFacility({ name: form.name, description: form.description || form.name, isVisibleOnMap: true, capacityM3: Number(form.capacityM3), criticalThresholdM3: Number(form.criticalThresholdM3), addressTitle: form.addressTitle, city: form.city, district: form.district, openAddress: form.openAddress, latitude: Number(form.latitude), longitude: Number(form.longitude) });
      setMessage('Facility stok lokasyonu oluşturuldu.'); setShowAdd(false); load();
    } catch (err) { setError(getApiErrorMessage(err, 'Tesis oluşturulamadı.')); }
    finally { setSaving(false); }
  };

  const remove = async (id: string) => {
    if (!confirm('Bu tesis soft-delete edilecek. Devam edilsin mi?')) return;
    setSaving(true); setError(''); setMessage('');
    try { await adminApi.deleteFacility(id); setMessage('Tesis silindi.'); setSelected(null); load(); }
    catch (err) { setError(getApiErrorMessage(err, 'Tesis silinemedi.')); }
    finally { setSaving(false); }
  };

  const assignManager = async () => {
    if (!selected || !managerForm.email || !managerForm.firstName || !managerForm.lastName) return;
    setSaving(true); setError('');
    try { await adminApi.assignFacilityManager(selected.id, managerForm); setMessage('Facility manager atandı.'); setManagerForm({ email: '', firstName: '', lastName: '', isPrimary: false }); await open(selected.id); }
    catch (err) { setError(getApiErrorMessage(err, 'Manager atanamadı.')); }
    finally { setSaving(false); }
  };

  const setPrimary = async (userId: string) => { if (!selected) return; setSaving(true); try { await adminApi.setPrimaryFacilityManager(selected.id, userId); setMessage('Primary manager güncellendi.'); await open(selected.id); } catch (err) { setError(getApiErrorMessage(err, 'Primary manager güncellenemedi.')); } finally { setSaving(false); } };
  const unassign = async (userId: string) => { if (!selected) return; setSaving(true); try { await adminApi.unassignFacilityManager(selected.id, userId); setMessage('Manager kaldırıldı.'); await open(selected.id); } catch (err) { setError(getApiErrorMessage(err, 'Manager kaldırılamadı.')); } finally { setSaving(false); } };

  return <div><div className="page-header"><div><h1 className="page-title">Tesisler</h1><p className="page-subtitle">Facility fiziksel stok lokasyonudur. Backend’de Facility update endpoint’i yoktur.</p></div><button className="btn btn-primary" onClick={() => setShowAdd(true)}><Plus size={16} /> Yeni Tesis</button></div>{error && <div className="error-banner" style={{ marginBottom: 16 }}>{error}</div>}{message && <div style={{ marginBottom: 16, color: '#6ee7b7' }}>{message}</div>}<div className="data-table-wrapper animate-fade-up"><div className="data-table-header"><div className="search-bar" style={{ minWidth: 300 }}><Search size={15} /><input placeholder="Tesis veya şehir ara..." value={search} onChange={e => setSearch(e.target.value)} /></div></div>{loading ? <Empty text="Tesisler yükleniyor..." /> : filtered.length === 0 ? <Empty text="Tesis bulunamadı." /> : <table className="data-table"><thead><tr><th>Tesis</th><th>Şehir</th><th>Durum</th><th style={{ textAlign: 'right' }}>İşlem</th></tr></thead><tbody>{filtered.map(f => <tr key={f.id}><td><strong>{f.name}</strong></td><td>{f.city}</td><td><span className="badge badge-blue">{f.status}</span></td><td style={{ textAlign: 'right' }}><button className="btn btn-ghost" onClick={() => open(f.id)}><Eye size={14}/> Detay</button><button className="btn btn-ghost" disabled={saving} style={{ color: '#f87171' }} onClick={() => remove(f.id)}><Trash2 size={14} /> Sil</button></td></tr>)}</tbody></table>}</div>{showAdd && <div className="modal-overlay" onClick={() => setShowAdd(false)}><div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 620 }}><Header title="Yeni Tesis" close={() => setShowAdd(false)} /><div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 14 }}><Input label="Ad" value={form.name} onChange={v => setForm(p => ({ ...p, name: v }))} /><Input label="Adres başlığı" value={form.addressTitle} onChange={v => setForm(p => ({ ...p, addressTitle: v }))} /><Input label="Şehir" value={form.city} onChange={v => setForm(p => ({ ...p, city: v }))} /><Input label="İlçe" value={form.district} onChange={v => setForm(p => ({ ...p, district: v }))} /><Input label="Kapasite m3" value={form.capacityM3} onChange={v => setForm(p => ({ ...p, capacityM3: v }))} /><Input label="Kritik eşik m3" value={form.criticalThresholdM3} onChange={v => setForm(p => ({ ...p, criticalThresholdM3: v }))} /><Input label="Latitude" value={form.latitude} onChange={v => setForm(p => ({ ...p, latitude: v }))} /><Input label="Longitude" value={form.longitude} onChange={v => setForm(p => ({ ...p, longitude: v }))} /><div className="form-group" style={{ gridColumn: 'span 2' }}><label className="form-label">Açık adres</label><input className="form-input" value={form.openAddress} onChange={e => setForm(p => ({ ...p, openAddress: e.target.value }))} /></div><div className="form-group" style={{ gridColumn: 'span 2' }}><label className="form-label">Açıklama</label><input className="form-input" value={form.description} onChange={e => setForm(p => ({ ...p, description: e.target.value }))} /></div></div><div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12, marginTop: 24 }}><button className="btn btn-ghost" onClick={() => setShowAdd(false)}>İptal</button><button className="btn btn-primary" disabled={saving} onClick={create}><Check size={16} /> Kaydet</button></div></div></div>}{selected && <div className="modal-overlay" onClick={() => setSelected(null)}><div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 760 }}><Header title="Facility Detayı" close={() => setSelected(null)} /><div style={{ display: 'grid', gap: 8, marginBottom: 16 }}><strong>{selected.name}</strong><span>Durum: {selected.status}</span><span>Açıklama: {selected.description}</span><span>Adres: {selected.address ? `${selected.address.city} / ${selected.address.district} - ${selected.address.openAddress}` : '-'}</span></div><h3 style={{ marginBottom: 10 }}>Managerlar</h3>{!selected.managers?.length ? <p style={{ color: 'var(--text-muted)' }}>Manager yok.</p> : <table className="data-table"><thead><tr><th>Ad</th><th>Email</th><th>Primary</th><th>İşlem</th></tr></thead><tbody>{selected.managers.map(m => <tr key={m.userId}><td>{m.firstName} {m.lastName}</td><td>{m.email}</td><td>{m.isPrimary ? 'Evet' : 'Hayır'}</td><td><button className="btn btn-ghost" disabled={saving} onClick={() => setPrimary(m.userId)}>Primary</button><button className="btn btn-ghost" disabled={saving} style={{ color: '#f87171' }} onClick={() => unassign(m.userId)}>Kaldır</button></td></tr>)}</tbody></table>}<div className="glass-card" style={{ padding: 16, marginTop: 16 }}><h3 style={{ marginBottom: 12 }}>Manager Ata</h3><div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 10 }}><Input label="Email" value={managerForm.email} onChange={v => setManagerForm(p => ({ ...p, email: v }))} /><Input label="Ad" value={managerForm.firstName} onChange={v => setManagerForm(p => ({ ...p, firstName: v }))} /><Input label="Soyad" value={managerForm.lastName} onChange={v => setManagerForm(p => ({ ...p, lastName: v }))} /><label style={{ display: 'flex', alignItems: 'center', gap: 8 }}><input type="checkbox" checked={managerForm.isPrimary} onChange={e => setManagerForm(p => ({ ...p, isPrimary: e.target.checked }))} /> Primary</label></div><button className="btn btn-primary" disabled={saving} style={{ marginTop: 12 }} onClick={assignManager}>Manager Ata</button></div></div></div>}</div>;
}
function Empty({ text }: { text: string }) { return <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}><Building2 size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>; }
function Input({ label, value, onChange }: { label: string; value: string; onChange: (v: string) => void }) { return <div className="form-group"><label className="form-label">{label}</label><input className="form-input" value={value} onChange={e => onChange(e.target.value)} /></div>; }
function Header({ title, close }: { title: string; close: () => void }) { return <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}><h2 style={{ fontSize: '1.2rem', fontWeight: 700 }}>{title}</h2><button className="btn btn-ghost" style={{ padding: 4 }} onClick={close}><X size={18} /></button></div>; }
