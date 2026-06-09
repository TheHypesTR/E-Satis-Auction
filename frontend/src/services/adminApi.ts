import api from '../api/axios';
import type { AuctionDetailDto, CategoryDto, UserSaleRequestDto } from '../types/commerce';
import type {
  AdminAuctionSummaryDto,
  AdminCampaignDto,
  AdminOrderDetailDto,
  AdminOrderSummaryDto,
  AdminProductListingDetailDto,
  AdminProductListingSummaryDto,
  AdminReturnRequestDetailDto,
  AdminReturnRequestSummaryDto,
  FacilityDto,
  ItemSummaryDto,
  ProductDetailDto,
  ProductSummaryDto,
} from '../types/admin';
import { itemsOf, normalizePaginationParams } from './apiUtils';

export const adminApi = {
  async products(params: Record<string, unknown> = {}) { const { data } = await api.get('/Product', { params: normalizePaginationParams(params) }); return itemsOf<ProductSummaryDto>(data); },
  async product(id: string) { const { data } = await api.get<ProductDetailDto>(`/Product/${id}`); return data; },
  async createProduct(payload: { sku: string; barcode?: string | null; name: string; categoryId: string; unitOfMeasure: number; baseAttributes?: Record<string, string> }) { const { data } = await api.post<{ id: string }>('/Product', payload); return data; },
  async activateProduct(id: string) { await api.put(`/Product/${id}/activate`); },
  async deactivateProduct(id: string) { await api.put(`/Product/${id}/deactivate`); },

  async categories(params: Record<string, unknown> = {}) { const { data } = await api.get('/Category', { params: normalizePaginationParams(params) }); return itemsOf<CategoryDto>(data); },

  async facilities(params: Record<string, unknown> = {}) { const { data } = await api.get('/Facility', { params: normalizePaginationParams(params) }); return itemsOf<FacilityDto>(data); },
  async createFacility(payload: { name: string; description: string; isVisibleOnMap: boolean; capacityM3: number; criticalThresholdM3: number; addressTitle: string; city: string; district: string; openAddress: string; latitude: number; longitude: number }) { const { data } = await api.post<{ id: string }>('/Facility', payload); return data; },
  async deleteFacility(id: string) { await api.delete(`/Facility/${id}`); },

  async items(params: Record<string, unknown> = {}) { const { data } = await api.get('/Item', { params: normalizePaginationParams(params) }); return itemsOf<ItemSummaryDto>(data); },
  async addStandardizedItem(payload: { productId: string; facilityId: string; quantity: number; unitOfMeasure: number; status: number; dynamicAttributes?: Record<string, string> }) { const { data } = await api.post<{ id: string }>('/Item/standardized', payload); return data; },
  async addAdHocItem(payload: { categoryId: string; facilityId: string; name: string; quantity: number; unitOfMeasure: number; status: number; dynamicAttributes?: Record<string, string> }) { const { data } = await api.post<{ id: string }>('/Item/adhoc', payload); return data; },

  async listings(params: Record<string, unknown> = {}) { const { data } = await api.get('/AdminProductListing', { params: normalizePaginationParams(params) }); return itemsOf<AdminProductListingSummaryDto>(data); },
  async createListing(payload: { productId: string; sourceFacilityId: string; price: number; currency: string; activeFrom?: string | null; activeUntil?: string | null }) { const { data } = await api.post<AdminProductListingDetailDto>('/AdminProductListing', payload); return data; },
  async updateListing(id: string, payload: { price: number; currency: string; activeFrom?: string | null; activeUntil?: string | null }) { const { data } = await api.put<AdminProductListingDetailDto>(`/AdminProductListing/${id}`, payload); return data; },
  async activateListing(id: string) { const { data } = await api.put<AdminProductListingDetailDto>(`/AdminProductListing/${id}/activate`); return data; },
  async deactivateListing(id: string) { const { data } = await api.put<AdminProductListingDetailDto>(`/AdminProductListing/${id}/deactivate`); return data; },
  async deleteListing(id: string) { await api.delete(`/AdminProductListing/${id}`); },

  async campaigns(params: Record<string, unknown> = {}) { const { data } = await api.get('/AdminCampaign', { params: normalizePaginationParams(params) }); return itemsOf<AdminCampaignDto>(data); },
  async createCampaign(payload: CampaignPayload) { const { data } = await api.post<AdminCampaignDto>('/AdminCampaign', payload); return data; },
  async updateCampaign(id: string, payload: CampaignPayload) { const { data } = await api.put<AdminCampaignDto>(`/AdminCampaign/${id}`, payload); return data; },
  async activateCampaign(id: string) { const { data } = await api.put<AdminCampaignDto>(`/AdminCampaign/${id}/activate`); return data; },
  async deactivateCampaign(id: string) { const { data } = await api.put<AdminCampaignDto>(`/AdminCampaign/${id}/deactivate`); return data; },

  async orders(params: Record<string, unknown> = {}) { const { data } = await api.get('/AdminPurchaseOrder', { params: normalizePaginationParams(params) }); return itemsOf<AdminOrderSummaryDto>(data); },
  async order(id: string) { const { data } = await api.get<AdminOrderDetailDto>(`/AdminPurchaseOrder/${id}`); return data; },
  async approveOrder(id: string, note?: string | null) { await api.put(`/AdminPurchaseOrder/${id}/approve`, { note }); },
  async rejectOrder(id: string, reason: string) { await api.put(`/AdminPurchaseOrder/${id}/reject`, { reason }); },
  async shipOrder(id: string, payload: { carrierName: string; trackingNumber: string; shippedAt?: string | null; trackingUrl?: string | null; notes?: string | null }) { await api.put(`/AdminPurchaseOrder/${id}/ship`, payload); },

  async returns(params: Record<string, unknown> = {}) { const { data } = await api.get('/AdminReturnRequest', { params: normalizePaginationParams(params) }); return itemsOf<AdminReturnRequestSummaryDto>(data); },
  async returnRequest(id: string) { const { data } = await api.get<AdminReturnRequestDetailDto>(`/AdminReturnRequest/${id}`); return data; },
  async approveReturn(id: string, note?: string | null) { await api.put(`/AdminReturnRequest/${id}/approve`, { note }); },
  async rejectReturn(id: string, reason: string) { await api.put(`/AdminReturnRequest/${id}/reject`, { reason }); },
  async receiveReturn(id: string, payload: { note?: string | null; targetFacilityId?: string | null; lines?: Array<{ returnRequestLineId: string; receivedQuantity: number; restockQuantity: number; note?: string | null }> }) { const { data } = await api.put<AdminReturnRequestDetailDto>(`/AdminReturnRequest/${id}/receive`, payload); return data; },

  async userSaleRequests(params: Record<string, unknown> = {}) { const { data } = await api.get('/AdminUserSaleRequest', { params: normalizePaginationParams(params) }); return itemsOf<UserSaleRequestDto>(data); },
  async approveUserSaleRequest(id: string, payload: { acquisitionPrice: number; targetResalePrice: number; adminNote?: string | null }) { const { data } = await api.put<UserSaleRequestDto>(`/AdminUserSaleRequest/${id}/approve`, payload); return data; },
  async rejectUserSaleRequest(id: string, reason: string) { const { data } = await api.put<UserSaleRequestDto>(`/AdminUserSaleRequest/${id}/reject`, { reason }); return data; },
  async intakeUserSaleRequest(id: string, adminNote?: string | null) { const { data } = await api.put<UserSaleRequestDto>(`/AdminUserSaleRequest/${id}/intake`, { adminNote }); return data; },

  async auctions(params: Record<string, unknown> = {}) { const { data } = await api.get('/AdminAuction', { params: normalizePaginationParams(params) }); return itemsOf<AdminAuctionSummaryDto>(data); },
  async createAuction(payload: AuctionPayload) { const { data } = await api.post<AuctionDetailDto>('/AdminAuction', payload); return data; },
  async scheduleAuction(id: string, payload: { startsAt: string; endsAt: string }) { const { data } = await api.put<AuctionDetailDto>(`/AdminAuction/${id}/schedule`, payload); return data; },
  async activateAuction(id: string) { const { data } = await api.put<AuctionDetailDto>(`/AdminAuction/${id}/activate`); return data; },
  async finalizeAuction(id: string) { const { data } = await api.post<AuctionDetailDto>(`/AdminAuction/${id}/finalize`); return data; },
  async cancelAuction(id: string) { const { data } = await api.put<AuctionDetailDto>(`/AdminAuction/${id}/cancel`); return data; },
  async relistAuction(id: string, payload: { startsAt: string; endsAt: string }) { const { data } = await api.post<AuctionDetailDto>(`/AdminAuction/${id}/relist`, payload); return data; },
};

export interface CampaignPayload {
  name: string;
  description?: string | null;
  couponCode?: string | null;
  scope: number;
  discountType: number;
  discountValue: number;
  minimumOrderAmount?: number | null;
  productListingId?: string | null;
  categoryId?: string | null;
  currency?: string | null;
  startsAt?: string | null;
  endsAt?: string | null;
}

export interface AuctionPayload {
  productListingId: string;
  sellerUserId?: string | null;
  startingPrice: number;
  minimumBidIncrement: number;
  startsAt: string;
  endsAt: string;
  quantity: number;
  currency: string;
}
