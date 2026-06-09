import type {
  AuctionBidDto,
  AuctionDetailDto,
  AuctionSummaryDto,
  CategoryDto,
  OrderLineDto,
  ReturnRequestDetailDto,
  UserSaleRequestDto,
} from './commerce';

export interface ProductSummaryDto {
  id: string;
  sku: string;
  barcode?: string | null;
  name: string;
  categoryName: string;
  unitOfMeasure: string | number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  version: number;
}

export interface ProductDetailDto extends ProductSummaryDto {
  categoryId: string;
  baseAttributes: Record<string, string>;
  facilityStocks: Array<{ facilityId: string; facilityName: string; totalAvailableQuantity: number }>;
}

export interface ManagerDto {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  isPrimary: boolean;
}

export interface FacilityDto {
  id: string;
  name: string;
  status: string;
  city: string;
}

export interface FacilityDetailDto extends FacilityDto {
  description: string;
  address?: { city: string; district: string; openAddress: string } | null;
  managers?: ManagerDto[];
}

export interface ItemSummaryDto {
  id: string;
  displayName: string;
  mode: string | number;
  status: string | number;
  quantity: number;
  unitOfMeasure: string | number;
  facilityId: string;
  facilityName: string;
  categoryId: string;
  categoryName: string;
  productId?: string | null;
  productName?: string | null;
  createdAt: string;
  updatedAt: string;
  version?: number;
}

export interface ItemDetailDto extends ItemSummaryDto {
  dynamicAttributes: Record<string, string>;
}

export interface InventoryTransactionDto {
  id: string;
  itemId: string;
  itemName: string;
  unitOfMeasure: string | number;
  facilityId: string;
  facilityName: string;
  transactionType: string | number;
  quantityChange: number;
  previousQuantity: number;
  newQuantity: number;
  referenceId?: string | null;
  referenceTrackingNumber?: string | null;
  createdByUserId: string;
  createdByUserName: string;
  createdAt: string;
}

export interface AdminProductListingSummaryDto {
  id: string;
  productId: string;
  productName: string;
  sku: string;
  sourceFacilityId: string;
  sourceFacilityName: string;
  price: number;
  currency: string;
  status: string | number;
  createdAt: string;
  updatedAt: string;
  version: number;
}

export interface AdminProductListingDetailDto extends AdminProductListingSummaryDto {
  categoryId: string;
  productIsActive: boolean;
  availableStockQuantity: number;
  activeFrom?: string | null;
  activeUntil?: string | null;
}

export interface AdminOrderSummaryDto {
  id: string;
  orderNumber: string;
  userId: string;
  userDisplayName: string;
  status: string | number;
  shipmentStatus: string | number;
  orderSource: string | number;
  totalAmount: number;
  currency: string;
  createdAt: string;
  updatedAt: string;
  version: number;
}

export interface AdminOrderDetailDto extends AdminOrderSummaryDto {
  subtotalAmount: number;
  discountAmount: number;
  shippingAmount: number;
  appliedCouponCampaignId?: string | null;
  appliedFreeShippingCampaignId?: string | null;
  approvalNote?: string | null;
  rejectionReason?: string | null;
  shippingInfo?: { carrierName: string; trackingNumber: string; shippedAt: string; trackingUrl?: string | null; notes?: string | null } | null;
  lines: OrderLineDto[];
}

export interface AdminReturnRequestSummaryDto {
  id: string;
  purchaseOrderId: string;
  orderNumber: string;
  userId: string;
  userDisplayName: string;
  status: string | number;
  reason: string;
  createdAt: string;
  updatedAt: string;
}

export interface AdminCampaignDto {
  id: string;
  name: string;
  description?: string | null;
  couponCode?: string | null;
  scope: string | number;
  discountType: string | number;
  discountValue: number;
  minimumOrderAmount?: number | null;
  productListingId?: string | null;
  categoryId?: string | null;
  currency?: string | null;
  status: string | number;
  startsAt: string;
  endsAt: string;
  version: number;
}

export interface CategoryAttributeOptionDto {
  id: string;
  value: string;
}

export interface CategoryAttributeDto {
  id: string;
  name: string;
  code: string;
  dataType: string | number;
  target: string | number;
  isRequired: boolean;
  options: CategoryAttributeOptionDto[];
}

export interface CategoryDetailDto extends CategoryDto {
  createdAt: string;
  updatedAt: string;
  attributes: CategoryAttributeDto[];
}

export interface PartSaleOperationDto {
  id: string;
  sourceItemId: string;
  createdPartItemId: string;
  productId: string;
  facilityId: string;
  quantity: number;
  unitOfMeasure: string | number;
  notes?: string | null;
  status: string | number;
  createdAt: string;
}

export interface DispatchSummaryDto {
  id: string;
  trackingNumber: string;
  status: string | number;
  sourceFacilityId: string;
  sourceFacilityName: string;
  targetFacilityId?: string | null;
  targetFacilityName?: string | null;
  targetAddressId?: string | null;
  receiverName: string;
  receiverPhone: string;
  dispatchDate?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface DispatchDetailDto extends DispatchSummaryDto {
  targetAddress?: { city?: string; district?: string; openAddress?: string } | null;
  notes?: string | null;
  deliveryNote?: string | null;
  lineItems: Array<{ sourceItemId: string; itemNameSnapshot: string; quantity: number }>;
}

export type AdminAuctionSummaryDto = AuctionSummaryDto;
export type AdminAuctionDetailDto = AuctionDetailDto;
export type AdminAuctionBidDto = AuctionBidDto;
export type AdminUserSaleRequestDto = UserSaleRequestDto;
export type AdminReturnRequestDetailDto = ReturnRequestDetailDto;
export type AdminCategoryDependencyDto = CategoryDto;
