import { useEffect, useState } from 'react';
import { Building2, Plus, Search, MapPin, Users, MoreVertical, Shield, X, CheckCircle2 } from 'lucide-react';
import api from '../api/axios';

interface Facility {
  id: string;
  name: string;
  status: string;
  city: string;
}



const statusConfig: Record<string, { label: string; badge: string; dot: string }> = {
  Active:      { label: 'Aktif',       badge: 'badge-green',  dot: '#6ee7b7' },
  Passive:     { label: 'Pasif',       badge: 'badge-amber',  dot: '#fcd34d' },
  Maintenance: { label: 'Bakımda',     badge: 'badge-purple', dot: '#a78bfa' },
};

const cityInitials = (city: string) => city.slice(0, 2).toUpperCase();
const cityColors = ['#7c3aed', '#3b82f6', '#ec4899', '#10b981', '#f59e0b', '#06b6d4'];

export default function Facilities() {
  const [facilities, setFacilities] = useState<Facility[]>([]);
  const [loading, setLoading]       = useState(true);
  const [search, setSearch]         = useState('');
  const [view, setView]             = useState<'grid' | 'table'>('grid');

  const [showAddFacilityModal, setShowAddFacilityModal] = useState(false);
  const [actionSaved, setActionSaved] = useState(false);

  // Add Facility form states
  const [newFacName, setNewFacName] = useState('');
  const [newFacDesc, setNewFacDesc] = useState('');
  const [newFacCapacity, setNewFacCapacity] = useState('1000');
  const [newFacCity, setNewFacCity] = useState('');
  const [newFacDistrict, setNewFacDistrict] = useState('');
  const [newFacAddress, setNewFacAddress] = useState('');

  const handleAddFacility = async () => {
    if (!newFacName || !newFacCity || !newFacDistrict) return;
    try {
      await api.post('/Facility', {
        name: newFacName,
        description: newFacDesc || null,
        isVisibleOnMap: true,
        capacityM3: Number(newFacCapacity),
        criticalThresholdM3: Number(newFacCapacity) * 0.8,
        addressTitle: 'Merkez',
        city: newFacCity,
        district: newFacDistrict,
        openAddress: newFacAddress,
        latitude: 39.92077,
        longitude: 32.85411
      });
      setActionSaved(true);
      setTimeout(() => {
        setActionSaved(false);
        setShowAddFacilityModal(false);
        setNewFacName(''); setNewFacDesc(''); setNewFacCity(''); setNewFacDistrict(''); setNewFacAddress(''); setNewFacCapacity('1000');
        void api.get('/Facility').then(res => {
          setFacilities(res.data?.items || res.data?.data || []);
        });
      }, 1000);
    } catch (err) {
      console.error('Failed to add facility', err);
    }
  };

  useEffect(() => {
    void api.get('/Facility').then(res => {
      const data = res.data?.items || res.data?.data || [];
      setFacilities(data); setLoading(false);
    }).catch(() => { setFacilities([]); setLoading(false); });
  }, []);

  const filtered = facilities.filter(f =>
    f.name.toLowerCase().includes(search.toLowerCase()) ||
    f.city.toLowerCase().includes(search.toLowerCase())
  );

  const stats = [
    { label: 'Toplam Tesis',   value: facilities.length, color: '#a78bfa' },
    { label: 'Aktif',          value: facilities.filter(f => f.status === 'Active').length,      color: '#6ee7b7' },
    { label: 'Pasif / Bakım',  value: facilities.filter(f => f.status !== 'Active').length,      color: '#fcd34d' },
  ];

  return (
    <div>
      {/* Header */}
      <div className="page-header">
        <div>
          <h1 className="page-title">Tesisler</h1>
          <p className="page-subtitle">{facilities.length} tesis kayıtlı</p>
        </div>
        <button className="btn btn-primary" onClick={() => setShowAddFacilityModal(true)}>
          <Plus size={16} />
          Yeni Tesis
        </button>
      </div>

      {/* Mini stat strip */}
      <div style={{ display: 'flex', gap: '16px', marginBottom: '28px' }}>
        {stats.map((s, i) => (
          <div key={i} className="animate-fade-up" style={{
            animationDelay: `${i * 0.08}s`,
            display: 'flex', alignItems: 'center', gap: '12px',
            background: 'rgba(255,255,255,0.03)', border: '1px solid var(--glass-border)',
            borderRadius: '12px', padding: '14px 20px',
          }}>
            <span style={{ width: '8px', height: '8px', borderRadius: '50%', background: s.color, boxShadow: `0 0 10px ${s.color}`, flexShrink: 0 }} />
            <div>
              <div style={{ fontSize: '1.4rem', fontWeight: 700, lineHeight: 1, color: s.color, fontFamily: "'Space Grotesk', sans-serif" }}>{s.value}</div>
              <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)', marginTop: '2px' }}>{s.label}</div>
            </div>
          </div>
        ))}
      </div>

      {/* Toolbar */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px', gap: '12px' }}>
        <div className="search-bar" style={{ minWidth: 300 }}>
          <Search size={15} style={{ color: 'var(--text-muted)', flexShrink: 0 }} />
          <input
            type="text"
            placeholder="Tesis adı veya şehir ara..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        <div style={{ display: 'flex', gap: '8px' }}>
          {(['grid', 'table'] as const).map(v => (
            <button
              key={v}
              onClick={() => setView(v)}
              className="btn btn-ghost"
              style={{
                padding: '9px 16px', fontSize: '0.82rem',
                background: view === v ? 'rgba(124,58,237,0.15)' : undefined,
                color:      view === v ? '#a78bfa' : undefined,
                borderColor: view === v ? 'rgba(124,58,237,0.3)' : undefined,
              }}
            >
              {v === 'grid' ? '⊞ Kart' : '☰ Liste'}
            </button>
          ))}
        </div>
      </div>

      {/* Content */}
      {loading ? (
        <div style={{ textAlign: 'center', padding: '80px', color: 'var(--text-muted)' }}>
          <Building2 size={36} style={{ margin: '0 auto 12px', opacity: 0.3 }} />
          <p>Yükleniyor...</p>
        </div>
      ) : view === 'grid' ? (
        /* ── Grid View ── */
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '20px' }}>
          {filtered.map((f, i) => {
            const cfg   = statusConfig[f.status] ?? statusConfig['Passive'];
            const color = cityColors[i % cityColors.length];
            return (
              <div key={f.id} className="glass-card animate-fade-up" style={{ animationDelay: `${i * 0.05}s` }}>
                {/* Card top */}
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '20px' }}>
                  <div style={{
                    width: '52px', height: '52px', borderRadius: '14px',
                    background: `${color}22`, border: `1px solid ${color}44`,
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    fontFamily: "'Space Grotesk', sans-serif", fontWeight: 700,
                    fontSize: '1rem', color: color,
                  }}>
                    {cityInitials(f.city)}
                  </div>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <span className={`badge ${cfg.badge}`}>
                      <span style={{ width: '5px', height: '5px', borderRadius: '50%', background: cfg.dot, display: 'inline-block' }} />
                      {cfg.label}
                    </span>
                    <button className="btn btn-ghost" style={{ width: '30px', height: '30px', padding: 0 }}>
                      <MoreVertical size={14} />
                    </button>
                  </div>
                </div>

                <h3 style={{ fontSize: '1rem', fontWeight: 600, color: 'var(--text-primary)', marginBottom: '8px' }}>{f.name}</h3>

                <div style={{ display: 'flex', alignItems: 'center', gap: '6px', color: 'var(--text-muted)', fontSize: '0.85rem' }}>
                  <MapPin size={13} />
                  {f.city}
                </div>

                <hr style={{ border: 'none', borderTop: '1px solid var(--glass-border)', margin: '16px 0' }} />

                <div style={{ display: 'flex', gap: '8px' }}>
                  <button className="btn btn-ghost" style={{ flex: 1, padding: '8px', fontSize: '0.82rem' }}>
                    <Users size={13} />
                    Yöneticiler
                  </button>
                  <button className="btn btn-ghost" style={{ flex: 1, padding: '8px', fontSize: '0.82rem' }}>
                    <Shield size={13} />
                    Detay
                  </button>
                </div>
              </div>
            );
          })}
        </div>
      ) : (
        /* ── Table View ── */
        <div className="data-table-wrapper animate-fade-up">
          <table className="data-table">
            <thead>
              <tr>
                <th>Tesis Adı</th>
                <th>Şehir</th>
                <th>Durum</th>
                <th style={{ textAlign: 'right' }}>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map(f => {
                const cfg = statusConfig[f.status] ?? statusConfig['Passive'];
                return (
                  <tr key={f.id}>
                    <td style={{ fontWeight: 500, color: 'var(--text-primary)' }}>{f.name}</td>
                    <td>
                      <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
                        <MapPin size={13} style={{ color: 'var(--text-muted)' }} />
                        {f.city}
                      </div>
                    </td>
                    <td><span className={`badge ${cfg.badge}`}>{cfg.label}</span></td>
                    <td style={{ textAlign: 'right' }}>
                      <button className="btn btn-ghost" style={{ padding: '7px 14px', fontSize: '0.8rem' }}>
                        <MoreVertical size={14} />
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
          <div style={{ padding: '14px 24px', borderTop: '1px solid var(--glass-border)', color: 'var(--text-muted)', fontSize: '0.82rem' }}>
            {filtered.length} kayıt
          </div>
        </div>
      )}

      {/* Add Facility Modal */}
      {showAddFacilityModal && (
        <div className="modal-overlay" onClick={() => setShowAddFacilityModal(false)}>
          <div className="modal-content animate-fade-up" onClick={e => e.stopPropagation()} style={{ maxWidth: 500, width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8 }}>
                <Building2 size={20} color="#a78bfa" /> Yeni Tesis Ekle
              </h2>
              <button className="btn btn-ghost" style={{ padding: 4 }} onClick={() => setShowAddFacilityModal(false)}>
                <X size={18} />
              </button>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 16, marginBottom: 24 }}>
              <div className="form-group">
                <label className="form-label">Tesis Adı</label>
                <input type="text" className="form-input" placeholder="Örn: Ankara Ana Depo" value={newFacName} onChange={e => setNewFacName(e.target.value)} />
              </div>
              <div className="form-group">
                <label className="form-label">Açıklama</label>
                <textarea className="form-input" placeholder="Opsiyonel" rows={2} style={{ resize: 'vertical' }} value={newFacDesc} onChange={e => setNewFacDesc(e.target.value)}></textarea>
              </div>
              <div style={{ display: 'flex', gap: 16 }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label className="form-label">Şehir</label>
                  <input type="text" className="form-input" placeholder="Örn: Ankara" value={newFacCity} onChange={e => setNewFacCity(e.target.value)} />
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label className="form-label">İlçe</label>
                  <input type="text" className="form-input" placeholder="Örn: Çankaya" value={newFacDistrict} onChange={e => setNewFacDistrict(e.target.value)} />
                </div>
              </div>
              <div className="form-group">
                <label className="form-label">Kapasite (m³)</label>
                <input type="number" className="form-input" placeholder="Örn: 1000" value={newFacCapacity} onChange={e => setNewFacCapacity(e.target.value)} />
              </div>
              <div className="form-group">
                <label className="form-label">Açık Adres</label>
                <textarea className="form-input" placeholder="Tam adres" rows={2} style={{ resize: 'vertical' }} value={newFacAddress} onChange={e => setNewFacAddress(e.target.value)}></textarea>
              </div>
            </div>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 12 }}>
              <button className="btn btn-ghost" onClick={() => setShowAddFacilityModal(false)}>İptal</button>
              <button className="btn btn-primary" style={{ gap: 8 }} onClick={handleAddFacility}>
                {actionSaved ? <><CheckCircle2 size={16}/> Eklendi</> : 'Tesis Ekle'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
