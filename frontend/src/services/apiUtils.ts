import type { AxiosError } from 'axios';

export function itemsOf<T>(payload: unknown): T[] {
  const value = payload as { items?: T[]; data?: T[] } | T[] | null | undefined;
  if (Array.isArray(value)) return value;
  return value?.items ?? value?.data ?? [];
}

export const MAX_PAGE_SIZE = 100;

export function clampPageSize(value: unknown, fallback = 10): number {
  const parsed = Number(value ?? fallback);
  if (!Number.isFinite(parsed)) return fallback;
  return Math.min(Math.max(Math.trunc(parsed), 1), MAX_PAGE_SIZE);
}

export function normalizePaginationParams(params: Record<string, unknown> = {}): Record<string, unknown> {
  const {
    pageSize,
    PageSize,
    pageNumber,
    PageNumber,
    ...rest
  } = params;

  const normalizedPageNumber = Number(pageNumber ?? PageNumber ?? 1);

  return {
    ...rest,
    PageNumber: Number.isFinite(normalizedPageNumber) && normalizedPageNumber >= 1 ? Math.trunc(normalizedPageNumber) : 1,
    PageSize: clampPageSize(pageSize ?? PageSize),
  };
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
  Suspended: 'Askıda',
  Archived: 'Arşiv',
  '1': 'Taslak',
  '2': 'Satışta',
  '3': 'Askıda',
  '4': 'Arşiv',
};

export const orderStatusLabel: Record<string, string> = {
  PaymentPending: 'Ödeme bekleniyor',
  PendingApproval: 'Admin onayı bekliyor',
  Approved: 'Onaylandı',
  Rejected: 'Reddedildi',
  Shipped: 'Kargoya verildi',
  Delivered: 'Teslim edildi',
  Cancelled: 'İptal edildi',
  '1': 'Admin onayı bekliyor',
  '2': 'Onaylandı',
  '3': 'Reddedildi',
  '4': 'Kargoya verildi',
  '5': 'Teslim edildi',
  '6': 'İptal edildi',
  '7': 'Ödeme bekleniyor',
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
  Ended: 'Bitti',
  '1': 'Taslak',
  '2': 'Planlandı',
  '3': 'Aktif',
  '4': 'Bitti',
  '5': 'Kazanan ödemesi bekleniyor',
  '6': 'Tamamlandı',
  '7': 'İptal edildi',
  '8': 'Ödeme süresi doldu',
  '9': 'Başarısız',
  '10': 'Yeniden listelenebilir',
};

export const returnStatusLabel: Record<string, string> = {
  Pending: 'Bekliyor',
  Approved: 'Onaylandı',
  Rejected: 'Reddedildi',
  Received: 'Teslim alındı',
  Cancelled: 'İptal edildi',
  '1': 'Bekliyor',
  '2': 'Onaylandı',
  '3': 'Reddedildi',
  '4': 'Teslim alındı',
  '5': 'İptal edildi',
};

export const campaignStatusLabel: Record<string, string> = {
  Draft: 'Taslak',
  Active: 'Aktif',
  Suspended: 'Askıda',
  Expired: 'Süresi doldu',
  '1': 'Taslak',
  '2': 'Aktif',
  '3': 'Askıda',
  '4': 'Süresi doldu',
};

export const userSaleRequestStatusLabel: Record<string, string> = {
  Pending: 'Bekliyor',
  Approved: 'Onaylandı',
  Rejected: 'Reddedildi',
  IntakeCreated: 'Intake oluşturuldu',
  '1': 'Bekliyor',
  '2': 'Onaylandı',
  '3': 'Reddedildi',
  '4': 'Intake oluşturuldu',
};

export const itemStatusLabel: Record<string, string> = {
  Available: 'Satılabilir',
  Reserved: 'Rezerve',
  InTransit: 'Transferde',
  Damaged: 'Hasarlı',
  Expired: 'Süresi doldu',
  Archived: 'Arşiv',
  '1': 'Satılabilir',
  '2': 'Rezerve',
  '3': 'Transferde',
  '4': 'Hasarlı',
  '5': 'Süresi doldu',
  '6': 'Arşiv',
};

export const itemModeLabel: Record<string, string> = {
  Standardized: 'Standardized',
  AdHoc: 'AdHoc',
  '1': 'Standardized',
  '2': 'AdHoc',
};

export const unitOfMeasureLabel: Record<string, string> = {
  Piece: 'Adet',
  Kg: 'Kg',
  Liter: 'Litre',
  Box: 'Kutu',
  Pack: 'Paket',
  Pallet: 'Palet',
  '1': 'Adet',
  '2': 'Kg',
  '3': 'Litre',
  '4': 'Kutu',
  '5': 'Paket',
  '6': 'Palet',
};

export const attributeDataTypeLabel: Record<string, string> = {
  Text: 'Metin',
  Number: 'Sayı',
  Date: 'Tarih',
  Boolean: 'Boolean',
  SelectList: 'Seçim listesi',
  '1': 'Metin',
  '2': 'Sayı',
  '3': 'Tarih',
  '4': 'Boolean',
  '5': 'Seçim listesi',
};

export const attributeTargetLabel: Record<string, string> = {
  ProductLevel: 'ProductLevel',
  ItemLevel: 'ItemLevel',
  '1': 'ProductLevel',
  '2': 'ItemLevel',
};

export const inventoryTransactionTypeLabel: Record<string, string> = {
  '1': 'AdHoc oluşturuldu',
  '2': 'Standardized oluşturuldu',
  '3': 'Rezerve',
  '4': 'Dispatch',
  '5': 'Receive',
  '6': 'Cancel',
  '7': 'Damaged',
  '8': 'Archived',
  '9': 'Adjusted',
  '10': 'Purchase reserved',
  '11': 'Purchase released',
  '12': 'Purchase shipped',
  '13': 'Purchase returned',
  '14': 'Part sale consumed',
  '15': 'Part sale created',
  '16': 'Auction reserved',
  '17': 'Auction released',
  '18': 'Auction sold',
};

export const dispatchStatusLabel: Record<string, string> = {
  Pending: 'Bekliyor',
  InTransit: 'Transferde',
  Completed: 'Tamamlandı',
  Cancelled: 'İptal edildi',
  '1': 'Bekliyor',
  '2': 'Transferde',
  '3': 'Tamamlandı',
  '4': 'İptal edildi',
};

export const isActiveListing = (status: string | number) => ['Active', '2'].includes(normalizeStatus(status));
export const isActiveAuction = (status: string | number) => ['Active', '3'].includes(normalizeStatus(status));
export const isScheduledAuction = (status: string | number) => ['Scheduled', '2'].includes(normalizeStatus(status));
export const isAuctionPaymentPending = (status: string | number) => ['PaymentPending', '5'].includes(normalizeStatus(status));

