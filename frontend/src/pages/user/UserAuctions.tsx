import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Gavel, Package, Search } from 'lucide-react';
import { commerceApi } from '../../services/commerceApi';
import { auctionStatusLabel, formatMoney, getApiErrorMessage, isPublicAuctionStatus } from '../../services/apiUtils';
import type { AuctionSummaryDto } from '../../types/commerce';

export default function UserAuctions() {
  const [auctions, setAuctions] = useState<AuctionSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    commerceApi.getAuctions({ pageSize: 50 })
      .then(setAuctions)
      .catch(err => setError(getApiErrorMessage(err, 'Açık artırmalar yüklenemedi.')))
      .finally(() => setLoading(false));
  }, []);

  const visibleAuctions = auctions.filter(a => isPublicAuctionStatus(a.status));

  return <div className="user-page"><div style={{ marginBottom: 32 }}><div className="user-section-label">Açık Artırma</div><h1 style={{ fontSize: '1.9rem', fontWeight: 700 }}>Açık Artırmalar</h1><p style={{ color: 'var(--text-muted)', marginTop: 4, fontSize: '0.9rem' }}>Teklif verme, minimum teklif ve anti-snipe kuralları backend tarafından uygulanır.</p></div>{loading ? <Empty icon={Package} text="Yükleniyor..." /> : error ? <Empty icon={Search} text={error} /> : visibleAuctions.length === 0 ? <Empty icon={Gavel} text="Açık artırma bulunamadı." /> : <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))', gap: 20 }}>{visibleAuctions.map(a => <Link key={a.id} to={`/user/auctions/${a.id}`} className="glass-card animate-fade-up" style={{ padding: 22, textDecoration: 'none' }}><div className="badge badge-amber" style={{ marginBottom: 12 }}>{auctionStatusLabel[String(a.status)] ?? String(a.status)}</div><h3 style={{ fontSize: '1.05rem', fontWeight: 700, marginBottom: 8 }}>{a.productName}</h3><code style={{ color: 'var(--text-muted)', fontSize: '0.78rem' }}>{a.sku}</code><div style={{ marginTop: 18, display: 'flex', justifyContent: 'space-between', gap: 10 }}><div><div style={{ color: 'var(--text-muted)', fontSize: '0.75rem' }}>Güncel fiyat</div><strong style={{ color: '#a78bfa', fontSize: '1.2rem' }}>{formatMoney(a.currentPrice, a.currency)}</strong></div><div style={{ textAlign: 'right' }}><div style={{ color: 'var(--text-muted)', fontSize: '0.75rem' }}>Minimum teklif</div><strong>{formatMoney(a.minimumNextBid, a.currency)}</strong></div></div><div style={{ marginTop: 14, color: 'var(--text-muted)', fontSize: '0.78rem' }}>Bitiş: {new Date(a.endsAt).toLocaleString('tr-TR')}</div></Link>)}</div>}</div>;
}

function Empty({ icon: Icon, text }: { icon: typeof Package; text: string }) { return <div style={{ textAlign: 'center', padding: '80px 0', color: 'var(--text-muted)' }}><Icon size={42} style={{ margin: '0 auto 12px', opacity: 0.3 }} /><p>{text}</p></div>; }
