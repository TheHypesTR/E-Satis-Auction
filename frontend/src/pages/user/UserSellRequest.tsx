import { useEffect, useState } from 'react';
import { Handshake, Package, Send, AlertCircle, CheckCircle2 } from 'lucide-react';
import { commerceApi } from '../../services/commerceApi';
import { getApiErrorMessage, requireAuth } from '../../services/apiUtils';
import { useNavigate } from 'react-router-dom';
import type { CategoryDto, UserSaleRequestDto } from '../../types/commerce';

export default function UserSellRequest() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [requests, setRequests] = useState<UserSaleRequestDto[]>([]);
  const [form, setForm] = useState({ title: '', description: '', categoryId: '', userEstimatedValue: '' });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');

  useEffect(() => {
    commerceApi.getCategories({ pageSize: 100 }).then(setCategories).catch(() => setCategories([]));
    if (localStorage.getItem('token')) commerceApi.getMyUserSaleRequests({ pageSize: 20 }).then(setRequests).catch(() => undefined);
  }, []);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!requireAuth(navigate)) return;
    setLoading(true); setError(''); setMessage('');
    try {
      await commerceApi.createUserSaleRequest({ title: form.title, description: form.description, categoryId: form.categoryId, userEstimatedValue: Number(form.userEstimatedValue) });
      setMessage('Talebiniz alındı. Ürün değerlendirme süreci için admin kontrolüne gönderildi.');
      setForm({ title: '', description: '', categoryId: '', userEstimatedValue: '' });
      setRequests(await commerceApi.getMyUserSaleRequests({ pageSize: 20 }));
    } catch (err) { setError(getApiErrorMessage(err, 'Ürün satış talebi oluşturulamadı.')); }
    finally { setLoading(false); }
  };

  return <div className="user-page"><div style={{ marginBottom: 32 }}><div className="user-section-label">Platforma Sat</div><h1 style={{ fontSize: '1.9rem', fontWeight: 700 }}>Ürününü Değerlendirmeye Gönder</h1><p style={{ color: 'var(--text-muted)', marginTop: 4, fontSize: '0.9rem' }}>Bu akış ürününüzü otomatik satışa çıkarmaz; admin inceleme ve intake sürecine gönderir.</p></div><div className="user-checkout-grid"><form className="glass-card" style={{ padding: 28 }} onSubmit={submit}><h2 style={{ display: 'flex', alignItems: 'center', gap: 10, fontSize: '1.2rem', marginBottom: 20 }}><Handshake size={20} color="#a78bfa" /> Talep Formu</h2>{error && <div className="error-banner" style={{ marginBottom: 16 }}><AlertCircle size={15} /> {error}</div>}{message && <div style={{ marginBottom: 16, padding: '12px 16px', color: '#6ee7b7', background: 'rgba(16,185,129,0.08)', borderRadius: 10, border: '1px solid rgba(16,185,129,0.15)' }}><CheckCircle2 size={15} /> {message}</div>}<div className="form-group"><label className="form-label">Ürün Başlığı</label><input className="form-input" value={form.title} onChange={e => setForm(p => ({ ...p, title: e.target.value }))} required /></div><div className="form-group"><label className="form-label">Kategori</label><select className="form-input" style={{ appearance: 'auto' }} value={form.categoryId} onChange={e => setForm(p => ({ ...p, categoryId: e.target.value }))} required><option value="">Seçiniz</option>{categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}</select></div><div className="form-group"><label className="form-label">Tahmini Değer</label><input className="form-input" type="number" value={form.userEstimatedValue} onChange={e => setForm(p => ({ ...p, userEstimatedValue: e.target.value }))} required /></div><div className="form-group"><label className="form-label">Açıklama / Kondisyon</label><textarea className="form-input" rows={5} style={{ resize: 'vertical' }} value={form.description} onChange={e => setForm(p => ({ ...p, description: e.target.value }))} required /></div><button className="btn btn-primary" disabled={loading} type="submit"><Send size={16} /> {loading ? 'Gönderiliyor...' : 'Talebi Gönder'}</button></form><div className="user-order-summary"><h3 style={{ fontSize: '1rem', fontWeight: 700, marginBottom: 16 }}>Taleplerim</h3>{requests.length === 0 ? <p style={{ color: 'var(--text-muted)' }}>Henüz talebiniz yok.</p> : requests.map(r => <div key={r.id} style={{ padding: '12px 0', borderBottom: '1px solid var(--glass-border)' }}><strong>{r.title}</strong><div style={{ color: 'var(--text-muted)', fontSize: '0.8rem', marginTop: 4 }}>Durum: {String(r.status)}</div></div>)}<div style={{ marginTop: 18, color: 'var(--text-muted)', fontSize: '0.8rem' }}><Package size={14} /> Fotoğraf yükleme endpoint’i bulunmadığı için bu form backend’in mevcut alanlarıyla sınırlıdır.</div></div></div></div>;
}
