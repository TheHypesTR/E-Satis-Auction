/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect, useState } from 'react';
import { Activity, Building2, Gavel, Package, RefreshCcw, ShoppingCart, Tag, Users } from 'lucide-react';
import { adminApi } from '../services/adminApi';
import { auctionStatusLabel, getApiErrorMessage, itemStatusLabel, listingStatusLabel, orderStatusLabel, returnStatusLabel, userSaleRequestStatusLabel } from '../services/apiUtils';

interface StatCard { label: string; value: string; sub: string; icon: typeof Package; color: string; }
interface Breakdown { title: string; rows: Array<{ label: string; value: number }> }

function countByStatus<T extends { status: string | number }>(items: T[], labels: Record<string, string>) {
  const map = new Map<string, number>();
  items.forEach(item => {
    const label = labels[String(item.status)] ?? String(item.status);
    map.set(label, (map.get(label) ?? 0) + 1);
  });
  return Array.from(map.entries()).map(([label, value]) => ({ label, value }));
}

export default function Dashboard() {
  const [stats, setStats] = useState<StatCard[]>([]);
  const [breakdowns, setBreakdowns] = useState<Breakdown[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    setLoading(true);
    Promise.all([
      adminApi.products({ pageSize: 100 }),
      adminApi.facilities({ pageSize: 100 }),
      adminApi.items({ pageSize: 100 }),
      adminApi.listings({ pageSize: 100 }),
      adminApi.orders({ pageSize: 100 }),
      adminApi.returns({ pageSize: 100 }),
      adminApi.auctions({ pageSize: 100 }),
      adminApi.userSaleRequests({ pageSize: 100 }),
    ])
      .then(([products, facilities, items, listings, orders, returns, auctions, saleRequests]) => {
        setStats([
          { label: 'Katalog Ürünleri', value: String(products.length), sub: 'Product master data', icon: Package, color: '#a78bfa' },
          { label: 'Tesisler', value: String(facilities.length), sub: 'Stok lokasyonları', icon: Building2, color: '#93c5fd' },
          { label: 'Fiziksel Item', value: String(items.length), sub: `${items.reduce((sum, item) => sum + item.quantity, 0)} toplam adet`, icon: Activity, color: '#6ee7b7' },
          { label: 'Satış Listingleri', value: String(listings.length), sub: 'ProductListing yüzeyi', icon: Tag, color: '#fcd34d' },
          { label: 'Siparişler', value: String(orders.length), sub: 'Admin onay/kargo akışı', icon: ShoppingCart, color: '#fb7185' },
          { label: 'İade Talepleri', value: String(returns.length), sub: 'Restock ayrı aksiyon', icon: RefreshCcw, color: '#38bdf8' },
          { label: 'Açık Artırmalar', value: String(auctions.length), sub: 'Schedule/active/finalize', icon: Gavel, color: '#f59e0b' },
          { label: 'Alım Talepleri', value: String(saleRequests.length), sub: 'Approve/reject/intake', icon: Users, color: '#c084fc' },
        ]);
        setBreakdowns([
          { title: 'Item Durumları', rows: countByStatus(items, itemStatusLabel) },
          { title: 'Listing Durumları', rows: countByStatus(listings, listingStatusLabel) },
          { title: 'Sipariş Durumları', rows: countByStatus(orders, orderStatusLabel) },
          { title: 'İade Durumları', rows: countByStatus(returns, returnStatusLabel) },
          { title: 'Auction Durumları', rows: countByStatus(auctions, auctionStatusLabel) },
          { title: 'UserSaleRequest Durumları', rows: countByStatus(saleRequests, userSaleRequestStatusLabel) },
        ]);
      })
      .catch(err => setError(getApiErrorMessage(err, 'Dashboard verileri yüklenemedi.')))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">Dashboard</h1>
          <p className="page-subtitle">Hazır analytics endpoint yok; özetler mevcut operasyonel endpointlerden türetilir.</p>
        </div>
      </div>
      {error && <div className="error-banner" style={{ marginBottom: 16 }}>{error}</div>}
      {loading ? <Empty text="Dashboard verileri yükleniyor..." /> : (
        <>
          <div className="stat-grid">
            {stats.map((s, i) => { const Icon = s.icon; return <div className={`stat-card animate-fade-up animate-fade-up-${Math.min(i + 1, 4)}`} key={s.label}><div className="stat-card-top"><div className="stat-icon" style={{ background: `${s.color}22` }}><Icon size={22} color={s.color} /></div></div><div><div className="stat-value">{s.value}</div><div className="stat-label">{s.label}</div><div style={{ color: 'var(--text-muted)', fontSize: '0.78rem', marginTop: 6 }}>{s.sub}</div></div></div>; })}
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))', gap: 20 }}>
            {breakdowns.map(section => <div className="data-table-wrapper" key={section.title}><div className="data-table-header"><strong>{section.title}</strong></div><div style={{ padding: 20, display: 'flex', flexDirection: 'column', gap: 10 }}>{section.rows.length === 0 ? <span style={{ color: 'var(--text-muted)' }}>Veri yok.</span> : section.rows.map(row => <div key={row.label} style={{ display: 'flex', justifyContent: 'space-between', borderBottom: '1px solid var(--glass-border)', paddingBottom: 8 }}><span>{row.label}</span><strong>{row.value}</strong></div>)}</div></div>)}
          </div>
        </>
      )}
    </div>
  );
}

function Empty({ text }: { text: string }) { return <div style={{ padding: '70px 0', textAlign: 'center', color: 'var(--text-muted)' }}><Activity size={34} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>; }
