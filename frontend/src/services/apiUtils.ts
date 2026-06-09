import type { AxiosError } from 'axios';

export function itemsOf<T>(payload: unknown): T[] {
  const value = payload as { items?: T[]; data?: T[] } | T[] | null | undefined;
  if (Array.isArray(value)) return value;
  return value?.items ?? value?.data ?? [];
}

export function getApiErrorMessage(error: unknown, fallback = 'İşlem sırasında bir hata oluştu.'): string {
  const axiosError = error as AxiosError<unknown>;
  const data = axiosError.response?.data as { message?: string; title?: string; detail?: string; errors?: Record<string, string[]> } | string | undefined;
  if (typeof data === 'string') return data;
  if (data?.message) return data.message;
  if (data?.title) return data.title;
  if (data?.detail) return data.detail;
  if (data?.errors && typeof data.errors === 'object') {
    const first = Object.values(data.errors).flat()[0];
    if (typeof first === 'string') return first;
  }
  return fallback;
}

export function makeIdempotencyKey(prefix: string): string {
  const random = crypto?.randomUUID?.() ?? `${Date.now()}-${Math.random().toString(36).slice(2)}`;
  return `${prefix}_${random}`;
}

export function isAuthenticated(): boolean {
  return Boolean(localStorage.getItem('token'));
}

export function requireAuth(navigate: (path: string) => void): boolean {
  if (isAuthenticated()) return true;
  navigate('/login');
  return false;
}

export function formatMoney(amount?: number | null, currency = 'TRY'): string {
  return new Intl.NumberFormat('tr-TR', { style: 'currency', currency }).format(amount ?? 0);
}

export function normalizeStatus(value: string | number | null | undefined): string {
  if (typeof value === 'string') return value;
  if (typeof value !== 'number') return '';
  return String(value);
}

export const listingStatusLabel: Record<string, string> = {
  Draft: 'Taslak',
  Active: 'Satışta',
  Inactive: 'Pasif',
  Archived: 'Arşiv',
  '0': 'Taslak',
  '1': 'Satışta',
  '2': 'Pasif',
  '3': 'Arşiv',
};

export const orderStatusLabel: Record<string, string> = {
  PaymentPending: 'Ödeme bekleniyor',
  PendingApproval: 'Admin onayı bekliyor',
  Approved: 'Onaylandı',
  Rejected: 'Reddedildi',
  Shipped: 'Kargoya verildi',
  Delivered: 'Teslim edildi',
  Cancelled: 'İptal edildi',
  '0': 'Ödeme bekleniyor',
  '1': 'Admin onayı bekliyor',
  '2': 'Onaylandı',
  '3': 'Reddedildi',
  '4': 'Kargoya verildi',
  '5': 'Teslim edildi',
  '6': 'İptal edildi',
};

export const auctionStatusLabel: Record<string, string> = {
  Draft: 'Taslak',
  Scheduled: 'Planlandı',
  Active: 'Aktif',
  PaymentPending: 'Kazanan ödemesi bekleniyor',
  Completed: 'Tamamlandı',
  Cancelled: 'İptal edildi',
  Relistable: 'Yeniden listelenebilir',
  PaymentExpired: 'Ödeme süresi doldu',
  Failed: 'Başarısız',
  '0': 'Taslak',
  '1': 'Planlandı',
  '2': 'Aktif',
  '3': 'Kazanan ödemesi bekleniyor',
  '4': 'Tamamlandı',
  '5': 'İptal edildi',
  '6': 'Yeniden listelenebilir',
  '7': 'Ödeme süresi doldu',
  '8': 'Başarısız',
};

export const isActiveListing = (status: string | number) => ['Active', '1'].includes(normalizeStatus(status));
export const isActiveAuction = (status: string | number) => ['Active', '2'].includes(normalizeStatus(status));
export const isScheduledAuction = (status: string | number) => ['Scheduled', '1'].includes(normalizeStatus(status));
export const isAuctionPaymentPending = (status: string | number) => ['PaymentPending', '3'].includes(normalizeStatus(status));

