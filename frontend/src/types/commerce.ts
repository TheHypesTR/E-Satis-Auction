export interface PaginatedList<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  totalPages?: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

export interface ProductListingSummaryDto {
  id: string;
  productId: string;
  productName: string;
  sku: string;
  sourceFacilityId: string;
  sourceFacilityName: string;
  price: number;
  currency: string;
  status: string | number;
  activeFrom?: string | null;
  activeUntil?: string | null;
  version: number;
}

export interface ProductListingDetailDto extends ProductListingSummaryDto {
  categoryId: string;
  availableStockQuantity: number;
  createdAt: string;
  updatedAt: string;
}

export interface CategoryDto {
  id: string;
  name: string;
  description?: string | null;
  isActive?: boolean;
}

export interface CartDto {
  id: string;
  productListingId: string;
  quantity: number;
  appliedCouponCampaignId?: string | null;
  appliedFreeShippingCampaignId?: string | null;
  previewSubtotalAmount: number;
  previewDiscountAmount: number;
  previewShippingAmount: number;
  previewTotalAmount: number;
  currency: string;
  status: string | number;
  version: number;
}

export interface CartPricePreviewDto {
  productListingId: string;
  quantity: number;
  originalUnitPrice: number;
  discountedUnitPrice: number;
  lineDiscountAmount: number;
  couponDiscountAmount: number;
  subtotalAmount: number;
  discountAmount: number;
  shippingAmount: number;
  totalAmount: number;
  currency: string;
  appliedLineCampaignId?: string | null;
  appliedCouponCampaignId?: string | null;
  appliedFreeShippingCampaignId?: string | null;
}

export interface PaymentAttemptDto {
  id: string;
  purchaseOrderId: string;
  amount: number;
  currency: string;
  status: string | number;
  expiresAt: string;
  failureReason?: string | null;
  createdAt: string;
  updatedAt: string;
  version: number;
}

export interface PaymentInitiationDto {
  payment: PaymentAttemptDto;
  order: OrderDetailDto;
}

export interface OrderSummaryDto {
  id: string;
  orderNumber: string;
  status: string | number;
  shipmentStatus: string | number;
  orderSource: string | number;
  totalAmount: number;
  currency: string;
  createdAt: string;
  updatedAt: string;
  version: number;
}

export interface OrderLineDto {
  id: string;
  productId: string;
  productListingId: string;
  campaignId?: string | null;
  productName: string;
  sku: string;
  unitPrice: number;
  discountedUnitPrice: number;
  quantity: number;
  currency: string;
}

export interface OrderDetailDto extends OrderSummaryDto {
  subtotalAmount: number;
  discountAmount: number;
  shippingAmount: number;
  appliedCouponCampaignId?: string | null;
  appliedFreeShippingCampaignId?: string | null;
  approvalNote?: string | null;
  rejectionReason?: string | null;
  lines: OrderLineDto[];
  returnRequests: ReturnRequestSummaryDto[];
}

export interface ReturnRequestSummaryDto {
  id: string;
  purchaseOrderId: string;
  status: string | number;
  reason: string;
  createdAt: string;
  updatedAt: string;
}

export interface ReturnRequestDetailDto extends ReturnRequestSummaryDto {
  userId: string;
  resolutionNote?: string | null;
  receivedAt?: string | null;
  receivedByUserId?: string | null;
  receiveNote?: string | null;
  lines: Array<{ id: string; purchaseOrderLineId: string; quantity: number; reason?: string | null; receivedQuantity: number; restockedQuantity: number; receiveNote?: string | null }>;
}

export interface UserSaleRequestDto {
  id: string;
  userId: string;
  title: string;
  description: string;
  categoryId: string;
  userEstimatedValue: number;
  acquisitionPrice?: number | null;
  targetResalePrice?: number | null;
  expectedProfit?: number | null;
  status: string | number;
  adminNote?: string | null;
  createdAt: string;
  updatedAt: string;
  version: number;
}

export interface AuctionSummaryDto {
  id: string;
  productListingId: string;
  productId: string;
  productName: string;
  sku: string;
  currentPrice: number;
  minimumNextBid: number;
  minimumBidIncrement: number;
  startsAt: string;
  endsAt: string;
  status: string | number;
  currentWinningBidId?: string | null;
  leadingUserId?: string | null;
  quantity: number;
  currency: string;
  version: number;
}

export interface AuctionDetailDto extends AuctionSummaryDto {
  sellerUserId?: string | null;
  startingPrice: number;
  originalEndsAt: string;
  winningUserId?: string | null;
  winningBidAmount?: number | null;
  purchaseOrderId?: string | null;
  paymentAttemptId?: string | null;
  waitingFeeAmount: number;
  serviceFeeAmount: number;
  sellerPayoutAmount: number;
  platformRevenueAmount: number;
  createdAt: string;
  updatedAt: string;
}

export interface AuctionBidDto {
  id: string;
  auctionId: string;
  bidderUserId: string;
  amount: number;
  status: string | number;
  createdAt: string;
  version: number;
}

export interface AuctionWinnerDto {
  auctionId: string;
  winningBidId?: string | null;
  winningUserId?: string | null;
  winningBidAmount?: number | null;
  purchaseOrderId?: string | null;
  paymentAttemptId?: string | null;
  status: string | number;
}
