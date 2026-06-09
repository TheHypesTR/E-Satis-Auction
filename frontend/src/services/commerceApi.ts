import api from '../api/axios';
import type {
  AuctionBidDto,
  AuctionDetailDto,
  AuctionSummaryDto,
  AuctionWinnerDto,
  CartDto,
  CartPricePreviewDto,
  CategoryDto,
  OrderDetailDto,
  OrderSummaryDto,
  PaymentAttemptDto,
  PaymentInitiationDto,
  ProductListingDetailDto,
  ProductListingSummaryDto,
  ReturnRequestDetailDto,
  UserSaleRequestDto,
} from '../types/commerce';
import { itemsOf } from './apiUtils';

export const commerceApi = {
  async getListings(params: Record<string, unknown> = {}) {
    const { data } = await api.get('/ProductListing', { params });
    return itemsOf<ProductListingSummaryDto>(data);
  },
  async getListing(id: string) {
    const { data } = await api.get<ProductListingDetailDto>(`/ProductListing/${id}`);
    return data;
  },
  async getCategories(params: Record<string, unknown> = {}) {
    const { data } = await api.get('/Category', { params });
    return itemsOf<CategoryDto>(data);
  },
  async updateCart(productListingId: string, quantity: number) {
    const { data } = await api.put<CartDto>('/Cart/listing', { productListingId, quantity });
    return data;
  },
  async clearCart() {
    await api.delete('/Cart');
  },
  async applyCoupon(couponCode: string) {
    const { data } = await api.post<CartDto>('/Cart/apply-coupon', { couponCode });
    return data;
  },
  async removeCoupon() {
    const { data } = await api.delete<CartDto>('/Cart/coupon');
    return data;
  },
  async getCartPreview() {
    const { data } = await api.get<CartPricePreviewDto>('/Cart/price-preview');
    return data;
  },
  async buyNow(productListingId: string, quantity: number, idempotencyKey: string) {
    const { data } = await api.post<OrderDetailDto>('/PurchaseOrder/buy-now', { productListingId, quantity, idempotencyKey });
    return data;
  },
  async initiatePayment(idempotencyKey: string) {
    const { data } = await api.post<PaymentInitiationDto>('/Payments/initiate', { idempotencyKey });
    return data;
  },
  async confirmPayment(paymentId: string, idempotencyKey: string) {
    const { data } = await api.post<PaymentAttemptDto>(`/Payments/${paymentId}/confirm`, { idempotencyKey });
    return data;
  },
  async failPayment(paymentId: string, idempotencyKey: string, reason: string) {
    const { data } = await api.post<PaymentAttemptDto>(`/Payments/${paymentId}/fail`, { idempotencyKey, reason });
    return data;
  },
  async getOrders(params: Record<string, unknown> = {}) {
    const { data } = await api.get('/PurchaseOrder', { params });
    return itemsOf<OrderSummaryDto>(data);
  },
  async getOrder(id: string) {
    const { data } = await api.get<OrderDetailDto>(`/PurchaseOrder/${id}`);
    return data;
  },
  async createReturnRequest(purchaseOrderId: string, reason: string, lines: Array<{ purchaseOrderLineId: string; quantity: number; reason?: string | null }>) {
    const { data } = await api.post<ReturnRequestDetailDto>(`/ReturnRequest/purchase-orders/${purchaseOrderId}`, { reason, lines });
    return data;
  },
  async createUserSaleRequest(payload: { title: string; description: string; categoryId: string; userEstimatedValue: number }) {
    const { data } = await api.post<UserSaleRequestDto>('/UserSaleRequest', payload);
    return data;
  },
  async getMyUserSaleRequests(params: Record<string, unknown> = {}) {
    const { data } = await api.get('/UserSaleRequest', { params });
    return itemsOf<UserSaleRequestDto>(data);
  },
  async getAuctions(params: Record<string, unknown> = {}) {
    const { data } = await api.get('/Auction', { params });
    return itemsOf<AuctionSummaryDto>(data);
  },
  async getAuction(id: string) {
    const { data } = await api.get<AuctionDetailDto>(`/Auction/${id}`);
    return data;
  },
  async getAuctionBids(id: string, pageSize = 10) {
    const { data } = await api.get(`/Auction/${id}/bids`, { params: { pageSize } });
    return itemsOf<AuctionBidDto>(data);
  },
  async placeBid(id: string, amount: number, idempotencyKey: string) {
    const { data } = await api.post<AuctionBidDto>(`/Auction/${id}/bids`, { amount, idempotencyKey });
    return data;
  },
  async getAuctionWinner(id: string) {
    const { data } = await api.get<AuctionWinnerDto>(`/Auction/${id}/winner`);
    return data;
  },
  async initiateAuctionPayment(id: string, idempotencyKey: string) {
    const { data } = await api.post<PaymentInitiationDto>(`/Auction/${id}/payment/initiate`, { idempotencyKey });
    return data;
  },
};
