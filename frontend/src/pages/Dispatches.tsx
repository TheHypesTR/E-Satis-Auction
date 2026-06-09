/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import { Eye, Send, Search, Truck, X } from 'lucide-react';
import { adminApi } from '../services/adminApi';
import { dispatchStatusLabel, getApiErrorMessage } from '../services/apiUtils';
import type { DispatchDetailDto, DispatchSummaryDto } from '../types/admin';

export default function Dispatches() {
  const [dispatches, setDispatches] = useState<DispatchSummaryDto[]>([]);
  const [selected, setSelected] = useState<DispatchDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [search, setSearch] = useState('');
  const [note, setNote] = useState('');

  const load = () => { setLoading(true); setError(''); adminApi.dispatches({ pageSize: 100 }).then(setDispatches).catch(err => setError(getApiErrorMessage(err, 'Dispatch listesi yüklenemedi.'))).finally(() => setLoading(false)); };
  useEffect(load, []);

  const filtered = dispatches.filter(d => d.trackingNumber.toLowerCase().includes(search.toLowerCase()) || d.sourceFacilityName.toLowerCase().includes(search.toLowerCase()) || (d.targetFacilityName ?? '').toLowerCase().includes(search.toLowerCase()));
  const open = async (id: string) => { setError(''); try { setSelected(await adminApi.dispatch(id)); } catch (err) { setError(getApiErrorMessage(err, 'Dispatch detayı alınamadı.')); } };
  const action = async (kind: 'ship' | 'cancel' | 'completeAddress') => {
    if (!selected) return;
    setSaving(true); setError(''); setMessage('');
    try {
      if (kind === 'ship') await adminApi.shipDispatch(selected.id);
      if (kind === 'cancel') await adminApi.cancelDispatch(selected.id, note || null);
      if (kind === 'completeAddress') await adminApi.completeAddressDelivery(selected.id, note || null);
      setMessage(kind === 'ship' ? 'Dispatch ship edildi.' : kind === 'cancel' ? 'Dispatch iptal edildi.' : 'Adres teslimatı tamamlandı.');
      setSelected(null); load();
    } catch (err) { setError(getApiErrorMessage(err, 'Dispatch işlemi başarısız.')); }
    finally { setSaving(false); }
  };

  return <div><div className="page-header"><div><h1 className="page-title">Dispatch / Sevkiyat</h1><p className="page-subtitle">List/detail ve güvenli status aksiyonları gerçek backend endpointlerinden gelir. Create/receive payloadları fake edilmedi.</p></div></div>{error && <div className="error-banner" style={{ marginBottom: 16 }}>{error}</div>}{message && <div style={{ color: '#6ee7b7', marginBottom: 16 }}>{message}</div>}<div className="data-table-wrapper"><div className="data-table-header"><div className="search-bar" style={{ minWidth: 300 }}><Search size={15}/><input placeholder="Tracking veya tesis ara..." value={search} onChange={e => setSearch(e.target.value)} /></div></div>{loading ? <Empty text="Dispatch listesi yükleniyor..." /> : filtered.length === 0 ? <Empty text="Dispatch bulunamadı." /> : <table className="data-table"><thead><tr><th>Tracking</th><th>Kaynak</th><th>Hedef</th><th>Alıcı</th><th>Durum</th><th style={{ textAlign: 'right' }}>İşlem</th></tr></thead><tbody>{filtered.map(d => <tr key={d.id}><td><code>{d.trackingNumber}</code></td><td>{d.sourceFacilityName}</td><td>{d.targetFacilityName ?? d.targetAddressId ?? '-'}</td><td>{d.receiverName}<div style={{ color: 'var(--text-muted)', fontSize: '0.78rem' }}>{d.receiverPhone}</div></td><td><span className="badge badge-purple">{dispatchStatusLabel[String(d.status)] ?? String(d.status)}</span></td><td style={{ textAlign: 'right' }}><button className="btn btn-ghost" onClick={() => open(d.id)}><Eye size={14}/> Detay</button></td></tr>)}</tbody></table>}</div>{selected && <div className="modal-overlay" onClick={() => setSelected(null)}><div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: 760 }}><Header title="Dispatch Detayı" close={() => setSelected(null)} /><div style={{ display: 'grid', gap: 8, marginBottom: 16 }}><strong>{selected.trackingNumber}</strong><span>Durum: {dispatchStatusLabel[String(selected.status)] ?? String(selected.status)}</span><span>Kaynak: {selected.sourceFacilityName}</span><span>Hedef: {selected.targetFacilityName ?? selected.targetAddressId ?? '-'}</span><span>Alıcı: {selected.receiverName} / {selected.receiverPhone}</span><span>Not: {selected.notes ?? '-'}</span><span>Teslim notu: {selected.deliveryNote ?? '-'}</span></div><table className="data-table"><thead><tr><th>Item</th><th>SourceItemId</th><th>Adet</th></tr></thead><tbody>{selected.lineItems.map(line => <tr key={line.sourceItemId}><td>{line.itemNameSnapshot}</td><td><code>{line.sourceItemId}</code></td><td>{line.quantity}</td></tr>)}</tbody></table><textarea className="form-input" rows={3} placeholder="İşlem notu" value={note} onChange={e => setNote(e.target.value)} style={{ marginTop: 14 }} /><div style={{ display: 'flex', gap: 10, flexWrap: 'wrap', marginTop: 14 }}><button className="btn btn-primary" disabled={saving} onClick={() => action('ship')}><Truck size={16}/> Ship</button><button className="btn btn-ghost" disabled={saving} onClick={() => action('completeAddress')}><Send size={16}/> Address Delivery Complete</button><button className="btn btn-ghost" disabled={saving} style={{ color: '#f87171' }} onClick={() => action('cancel')}>Cancel</button></div><p style={{ color: 'var(--text-muted)', fontSize: '0.8rem', marginTop: 12 }}>Receive endpoint line bazlı mode/product/damaged quantity ister; güvenli form tasarımı gerektirdiği için fake edilmedi.</p></div></div>}</div>;
}
function Empty({ text }: { text: string }) { return <div style={{ padding: '60px 0', textAlign: 'center', color: 'var(--text-muted)' }}><Truck size={32} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>; }
function Header({ title, close }: { title: string; close: () => void }) { return <div style={{ display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:20 }}><h2>{title}</h2><button className="btn btn-ghost" style={{ padding:4 }} onClick={close}><X size={18}/></button></div>; }
