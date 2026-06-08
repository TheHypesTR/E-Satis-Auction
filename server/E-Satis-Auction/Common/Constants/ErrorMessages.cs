namespace E_Satis_Auction.Common.Constants;

public static class ErrorMessages
{
    public static class Address
    {
        public const string EntityName = "Address.EntityName";
    }
    
    public static class Auth
    {
        public const string AccountLocked = "Auth.AccountLocked";
        public const string AccountResigned = "Auth.AccountResigned";
        public const string AccountSuspended = "Auth.AccountSuspended";
        public const string EmailAlreadyRegistered = "Auth.EmailAlreadyRegistered";
        public const string EmailNotVerified = "Auth.EmailNotVerified";
        public const string InvalidCredentials = "Auth.InvalidCredentials";
        public const string InvalidToken = "Auth.InvalidToken";
        public const string TCNAlreadyRegistered = "Auth.TCNAlreadyRegistered";
        public const string TokenGenerationFailed = "Auth.TokenGenerationFailed";
        public const string UnauthorizedAccess = "Auth.UnauthorizedAccess";
    }

    public static class Category
    {
        public const string AlreadyActive = "Category.AlreadyActive";
        public const string AlreadyInactive = "Category.AlreadyInactive";
        public const string AttributeCodeInvalidCharacters = "Category.AttributeCodeInvalidCharacters";
        public const string AttributeCodeMaxLength = "Category.AttributeCodeMaxLength";
        public const string AttributeEntityName = "CategoryAttribute.EntityName";
        public const string AttributeNameMaxLength = "Category.AttributeNameMaxLength";
        public const string AttributeCategoryRequired = "Category.AttributeCategoryRequired";
        public const string AttributeCodeRequired = "Category.AttributeCodeRequired";
        public const string AttributeNameRequired = "Category.AttributeNameRequired";
        public const string AttributeMutationBlockedByDependentData = "Category.AttributeMutationBlockedByDependentData";
        public const string AttributeMutationRequiresInactiveCategory = "Category.AttributeMutationRequiresInactiveCategory";
        public const string AttributeRequired = "Category.AttributeRequired";
        public const string CannotDeleteLastOption = "Category.CannotDeleteLastOption";
        public const string DescriptionMaxLength = "Category.DescriptionMaxLength";
        public const string DuplicateAttributeCode = "Category.DuplicateAttributeCode";
        public const string DuplicateAttributeCodeInRequest = "Category.DuplicateAttributeCodeInRequest";
        public const string DuplicateOptionValue = "Category.DuplicateOptionValue";
        public const string EntityName = "Category.EntityName";
        public const string NameAlreadyExists = "Category.NameAlreadyExists";
        public const string NameMaxLength = "Category.NameMaxLength";
        public const string NameRequired = "Category.NameRequired";
        public const string MustBeActiveToAddProduct = "Category.MustBeActiveToAddProduct";
        public const string OptionEntityName = "CategoryAttributeOption.EntityName";
        public const string OptionOnlyForSelectList = "Category.OptionOnlyForSelectList";
        public const string OptionValueMaxLength = "Category.OptionValueMaxLength";
        public const string OptionValueRequired = "Category.OptionValueRequired";
    }

    public static class Commerce
    {
        public const string EntityName = "Commerce.EntityName";
    }

    public static class Cart
    {
        public const string EntityName = "Cart.EntityName";
        public const string NotActive = "Cart.NotActive";
        public const string PricePreviewInvalid = "Cart.PricePreviewInvalid";
    }

    public static class Auction
    {
        public const string AlreadyEnded = "Auction.AlreadyEnded";
        public const string BidAmountInvalid = "Auction.BidAmountInvalid";
        public const string BidBelowMinimumNextBid = "Auction.BidBelowMinimumNextBid";
        public const string BidConcurrencyConflict = "Auction.BidConcurrencyConflict";
        public const string CannotUpdateAfterBids = "Auction.CannotUpdateAfterBids";
        public const string CannotUpdateInCurrentStatus = "Auction.CannotUpdateInCurrentStatus";
        public const string CurrencyRequired = "Auction.CurrencyRequired";
        public const string EntityName = "Auction.EntityName";
        public const string InvalidDateRange = "Auction.InvalidDateRange";
        public const string InvalidReservedInventoryState = "Auction.InvalidReservedInventoryState";
        public const string InvalidStatusTransition = "Auction.InvalidStatusTransition";
        public const string MinimumBidIncrementInvalid = "Auction.MinimumBidIncrementInvalid";
        public const string NoActiveReservation = "Auction.NoActiveReservation";
        public const string NotActive = "Auction.NotActive";
        public const string NotEnded = "Auction.NotEnded";
        public const string NotStarted = "Auction.NotStarted";
        public const string PaymentAlreadyStarted = "Auction.PaymentAlreadyStarted";
        public const string PaymentNotAvailable = "Auction.PaymentNotAvailable";
        public const string ProductListingAlreadyInAuction = "Auction.ProductListingAlreadyInAuction";
        public const string ProductListingRequired = "Auction.ProductListingRequired";
        public const string ProductRequired = "Auction.ProductRequired";
        public const string QuantityInvalid = "Auction.QuantityInvalid";
        public const string SellerCannotBid = "Auction.SellerCannotBid";
        public const string StartingPriceInvalid = "Auction.StartingPriceInvalid";
        public const string WinnerRequired = "Auction.WinnerRequired";
        public const string WinnerOnly = "Auction.WinnerOnly";
    }

    public static class ProductListing
    {
        public const string AlreadyActive = "ProductListing.AlreadyActive";
        public const string AlreadyArchived = "ProductListing.AlreadyArchived";
        public const string AlreadyInactive = "ProductListing.AlreadyInactive";
        public const string AlreadySuspended = "ProductListing.AlreadySuspended";
        public const string CannotDeleteWithOrders = "ProductListing.CannotDeleteWithOrders";
        public const string CurrencyRequired = "ProductListing.CurrencyRequired";
        public const string DuplicateActiveListing = "ProductListing.DuplicateActiveListing";
        public const string EntityName = "ProductListing.EntityName";
        public const string InvalidCurrency = "ProductListing.InvalidCurrency";
        public const string InvalidActiveDateRange = "ProductListing.InvalidActiveDateRange";
        public const string NoAvailableStock = "ProductListing.NoAvailableStock";
        public const string NotSellable = "ProductListing.NotSellable";
        public const string PriceMustBePositive = "ProductListing.PriceMustBePositive";
        public const string ProductRequired = "ProductListing.ProductRequired";
        public const string SourceFacilityRequired = "ProductListing.SourceFacilityRequired";
    }

    public static class Campaign
    {
        public const string AlreadyActive = "Campaign.AlreadyActive";
        public const string AlreadySuspended = "Campaign.AlreadySuspended";
        public const string CouponCodeMaxLength = "Campaign.CouponCodeMaxLength";
        public const string CouponCodeRequired = "Campaign.CouponCodeRequired";
        public const string CurrencyMismatch = "Campaign.CurrencyMismatch";
        public const string CurrencyRequiredForFixedDiscount = "Campaign.CurrencyRequiredForFixedDiscount";
        public const string DiscountValueMustBePositive = "Campaign.DiscountValueMustBePositive";
        public const string EntityName = "Campaign.EntityName";
        public const string FreeShippingDiscountMustBeZero = "Campaign.FreeShippingDiscountMustBeZero";
        public const string InvalidDateRange = "Campaign.InvalidDateRange";
        public const string MinimumOrderAmountInvalid = "Campaign.MinimumOrderAmountInvalid";
        public const string NameRequired = "Campaign.NameRequired";
        public const string NotApplicable = "Campaign.NotApplicable";
        public const string PercentageDiscountInvalid = "Campaign.PercentageDiscountInvalid";
        public const string ProductAlreadyAssigned = "Campaign.ProductAlreadyAssigned";
        public const string ProductNotAssigned = "Campaign.ProductNotAssigned";
        public const string ProductRequired = "Campaign.ProductRequired";
    }

    public static class Dispatch
    {
        public const string DeliveryNoteMaxLength = "Dispatch.DeliveryNoteMaxLength";
        public const string DispatchRequired = "Dispatch.DispatchRequired";
        public const string DuplicateItemsNotAllowed = "Dispatch.DuplicateItemsNotAllowed";
        public const string EntityName = "Dispatch.EntityName";
        public const string ExclusiveTargetRequired = "Dispatch.ExclusiveTargetRequired";
        public const string InsufficientStock = "Dispatch.InsufficientStock";
        public const string ItemNameRequired = "Dispatch.ItemNameRequired";
        public const string ItemsRequired = "Dispatch.ItemsRequired";
        public const string ItemStatusInvalid = "Dispatch.ItemStatusInvalid";
        public const string NotesMaxLength = "Dispatch.NotesMaxLength";
        public const string QuantityMustBePositive = "Dispatch.QuantityMustBePositive";
        public const string ReceiptItemMissing = "Dispatch.ReceiptItemMissing";
        public const string ReceiptQuantityInvalid = "Dispatch.ReceiptQuantityInvalid";
        public const string ReceiptQuantityMismatch = "Dispatch.ReceiptQuantityMismatch";
        public const string ReceiverNameMaxLength = "Dispatch.ReceiverNameMaxLength";
        public const string ReceiverNameRequired = "Dispatch.ReceiverNameRequired";
        public const string ReceiverPhoneRequired = "Dispatch.ReceiverPhoneRequired";
        public const string SourceFacilityRequired = "Dispatch.SourceFacilityRequired";
        public const string SourceItemRequired = "Dispatch.SourceItemRequired";
        public const string StandardizedItemCannotBeAdHoc = "Dispatch.StandardizedItemCannotBeAdHoc";
        public const string StatusNotPending = "Dispatch.StatusNotPending";
        public const string StatusNotInTransit = "Dispatch.StatusNotInTransit";
        public const string TargetAddressInvalid = "Dispatch.TargetAddressInvalid";
        public const string TargetAddressRequiredForDelivery = "Dispatch.TargetAddressRequiredForDelivery";
        public const string TargetFacilityInvalid = "Dispatch.TargetFacilityInvalid";
        public const string TargetFacilityMustBeNullForDelivery = "Dispatch.TargetFacilityMustBeNullForDelivery";
        public const string TargetFacilityRequiredForReceipt = "Dispatch.TargetFacilityRequiredForReceipt";
    }

    public static class Exception
    {
        public const string AccountTitle = "Exception.AccountTitle";
        public const string AddressTitle = "Exception.AddressTitle";
        public const string BusinessErrorTitle = "Exception.BusinessErrorTitle";
        public const string CategoryTitle = "Exception.CategoryTitle";
        public const string CommerceTitle = "Exception.CommerceTitle";
        public const string CredentialsTitle = "Exception.CredentialsTitle";
        public const string DispatchTitle = "Exception.DispatchTitle";
        public const string InventoryTitle = "Exception.InventoryTitle";
        public const string InvitationTitle = "Exception.InvitationTitle";
        public const string NotFoundMessage = "Exception.NotFoundMessage";
        public const string PayloadTitle = "Exception.PayloadTitle";
        public const string ProductTitle = "Exception.ProductTitle";
        public const string RegistrationTitle = "Exception.RegistrationTitle";
        public const string ResourceNotFoundTitle = "Exception.ResourceNotFoundTitle";
        public const string RoleAssignmentTitle = "Exception.RoleAssignmentTitle";
        public const string SecurityTitle = "Exception.SecurityTitle";
        public const string TokenTitle = "Exception.TokenTitle";
        public const string TooManyRequestsDetail = "Exception.TooManyRequestsDetail";
        public const string TooManyRequestsTitle = "Exception.TooManyRequestsTitle";
        public const string UnauthorizedAccess = "Exception.UnauthorizedAccess";
        public const string ValidationErrorDetail = "Exception.ValidationErrorDetail";
        public const string ValidationErrorTitle = "Exception.ValidationErrorTitle";
        public const string VerificationTitle = "Exception.VerificationTitle";
    }

    public static class Facility
    {
        public const string AlreadyPrimaryManager = "Facility.AlreadyPrimaryManager";
        public const string EntityName = "Facility.EntityName";
        public const string ManagerAlreadyExists = "Facility.ManagerAlreadyExists";
        public const string MustBeApproved = "Facility.MustBeApproved";
        public const string UnauthorizedFacilityAccess = "Facility.UnauthorizedFacilityAccess";
        public const string UnknownFacility = "Facility.Unknown";
    }
    
    public static class Item
    {
        public const string CategoryRequired = "Item.CategoryRequired";
        public const string DuplicateDynamicAttributeKey = "Item.DuplicateDynamicAttributeKey";
        public const string DynamicAttributeKeyRequired = "Item.DynamicAttributeKeyRequired";
        public const string DecreaseAmountMustBePositive = "Item.DecreaseAmountMustBePositive";
        public const string DynamicAttributeValueRequired = "Item.DynamicAttributeValueRequired";
        public const string EntityName = "Item.EntityName";
        public const string FacilityRequired = "Item.FacilityRequired";
        public const string IncreaseAmountMustBePositive = "Item.IncreaseAmountMustBePositive";
        public const string InvalidAttributeKey = "Item.InvalidAttributeKey";
        public const string InvalidAttributeValue = "Item.InvalidAttributeValue";
        public const string NameMaxLength = "Item.NameMaxLength";
        public const string NameRequiredForAdHoc = "Item.NameRequiredForAdHoc";
        public const string QuantityCannotBeNegative = "Item.QuantityCannotBeNegative";
        public const string ProductIdRequiredForStandardized = "Item.ProductIdRequiredForStandardized";
        public const string ProductIdMustBeNullForAdHoc = "Item.ProductIdMustBeNullForAdHoc";
        public const string RequiredAttributeMissing = "Item.RequiredAttributeMissing";
        public const string UnknownItem = "Item.Unknown";
    }

    public static class Product
    {
        public const string AlreadyActive = "Product.AlreadyActive";
        public const string AlreadyInactive = "Product.AlreadyInactive";
        public const string AttributeKeyRequired = "Product.AttributeKeyRequired";
        public const string AttributeValueRequired = "Product.AttributeValueRequired";
        public const string CategoryRequired = "Product.CategoryRequired";
        public const string EntityName = "Product.EntityName";
        public const string InactiveProductUpdateNotAllowed = "Product.InactiveProductUpdateNotAllowed";
        public const string InvalidAttributeKey = "Product.InvalidAttributeKey";
        public const string InvalidAttributeValue = "Product.InvalidAttributeValue";
        public const string NameMaxLength = "Product.NameMaxLength";
        public const string NameRequired = "Product.NameRequired";
        public const string ProductNotEligibleForInventory = "Product.ProductNotEligibleForInventory";
        public const string ProductNotAvailable = "Product.ProductNotAvailable";
        public const string RequiredAttributeMissing = "Product.RequiredAttributeMissing";
        public const string SkuAlreadyExists = "Product.SkuAlreadyExists";
        public const string SkuInvalidFormat = "Product.SkuInvalidFormat";
        public const string SkuMaxLength = "Product.SkuMaxLength";
        public const string SkuRequired = "Product.SkuRequired";
        public const string UnknownProduct = "Product.Unknown";
    }

    public static class PurchaseOrder
    {
        public const string AlreadyShipped = "PurchaseOrder.AlreadyShipped";
        public const string AccessDenied = "PurchaseOrder.AccessDenied";
        public const string AmountInvalid = "PurchaseOrder.AmountInvalid";
        public const string CannotMutateSubmittedOrder = "PurchaseOrder.CannotMutateSubmittedOrder";
        public const string CarrierNameRequired = "PurchaseOrder.CarrierNameRequired";
        public const string CarrierNameMaxLength = "PurchaseOrder.CarrierNameMaxLength";
        public const string CurrencyMismatch = "PurchaseOrder.CurrencyMismatch";
        public const string CurrencyRequired = "PurchaseOrder.CurrencyRequired";
        public const string DiscountedPriceInvalid = "PurchaseOrder.DiscountedPriceInvalid";
        public const string EntityName = "PurchaseOrder.EntityName";
        public const string InsufficientStock = "PurchaseOrder.InsufficientStock";
        public const string InvalidReservedInventoryState = "PurchaseOrder.InvalidReservedInventoryState";
        public const string LineRequired = "PurchaseOrder.LineRequired";
        public const string LinesRequired = "PurchaseOrder.LinesRequired";
        public const string NoteMaxLength = "PurchaseOrder.NoteMaxLength";
        public const string OriginalItemRequired = "PurchaseOrder.OriginalItemRequired";
        public const string ProductListingRequired = "PurchaseOrder.ProductListingRequired";
        public const string ProductNameSnapshotRequired = "PurchaseOrder.ProductNameSnapshotRequired";
        public const string ProductRequired = "PurchaseOrder.ProductRequired";
        public const string QuantityMustBePositive = "PurchaseOrder.QuantityMustBePositive";
        public const string RejectionReasonRequired = "PurchaseOrder.RejectionReasonRequired";
        public const string ReservedItemRequired = "PurchaseOrder.ReservedItemRequired";
        public const string ShippingInfoRequired = "PurchaseOrder.ShippingInfoRequired";
        public const string SkuSnapshotRequired = "PurchaseOrder.SkuSnapshotRequired";
        public const string StatusMustBeApproved = "PurchaseOrder.StatusMustBeApproved";
        public const string StatusMustBePendingApproval = "PurchaseOrder.StatusMustBePendingApproval";
        public const string StatusMustBePaymentPending = "PurchaseOrder.StatusMustBePaymentPending";
        public const string TrackingNumberRequired = "PurchaseOrder.TrackingNumberRequired";
        public const string TrackingNumberMaxLength = "PurchaseOrder.TrackingNumberMaxLength";
        public const string TrackingUrlMaxLength = "PurchaseOrder.TrackingUrlMaxLength";
        public const string UnitPriceMustBePositive = "PurchaseOrder.UnitPriceMustBePositive";
        public const string UserRequired = "PurchaseOrder.UserRequired";
    }

    public static class Payment
    {
        public const string AmountInvalid = "Payment.AmountInvalid";
        public const string EntityName = "Payment.EntityName";
        public const string Expired = "Payment.Expired";
        public const string FailureReasonMaxLength = "Payment.FailureReasonMaxLength";
        public const string FailureReasonRequired = "Payment.FailureReasonRequired";
        public const string IdempotencyConflict = "Payment.IdempotencyConflict";
        public const string IdempotencyKeyMaxLength = "Payment.IdempotencyKeyMaxLength";
        public const string IdempotencyKeyRequired = "Payment.IdempotencyKeyRequired";
        public const string InvalidStateTransition = "Payment.InvalidStateTransition";
        public const string NotExpired = "Payment.NotExpired";
    }

    public static class ReturnRequest
    {
        public const string AccessDenied = "ReturnRequest.AccessDenied";
        public const string CannotMutateSubmittedRequest = "ReturnRequest.CannotMutateSubmittedRequest";
        public const string AlreadyReceived = "ReturnRequest.AlreadyReceived";
        public const string CannotReceivePending = "ReturnRequest.CannotReceivePending";
        public const string CannotReceiveRejected = "ReturnRequest.CannotReceiveRejected";
        public const string DuplicateLinesNotAllowed = "ReturnRequest.DuplicateLinesNotAllowed";
        public const string EntityName = "ReturnRequest.EntityName";
        public const string InvalidReceiveQuantity = "ReturnRequest.InvalidReceiveQuantity";
        public const string InvalidQuantity = "ReturnRequest.InvalidQuantity";
        public const string InvalidRestockQuantity = "ReturnRequest.InvalidRestockQuantity";
        public const string LineNotFound = "ReturnRequestLine.NotFound";
        public const string LinesRequired = "ReturnRequest.LinesRequired";
        public const string NotApproved = "ReturnRequest.NotApproved";
        public const string NotEligible = "ReturnRequest.NotEligible";
        public const string PurchaseOrderLineRequired = "ReturnRequest.PurchaseOrderLineRequired";
        public const string PurchaseOrderRequired = "ReturnRequest.PurchaseOrderRequired";
        public const string QuantityMustBePositive = "ReturnRequest.QuantityMustBePositive";
        public const string ReasonRequired = "ReturnRequest.ReasonRequired";
        public const string ReasonMaxLength = "ReturnRequest.ReasonMaxLength";
        public const string ResolutionNoteRequired = "ReturnRequest.ResolutionNoteRequired";
        public const string ResolutionNoteMaxLength = "ReturnRequest.ResolutionNoteMaxLength";
        public const string ReceiveNotAllowed = "ReturnRequest.ReceiveNotAllowed";
        public const string StatusMustBePending = "ReturnRequest.StatusMustBePending";
        public const string UserRequired = "ReturnRequest.UserRequired";
    }

    public static class UserSaleRequest
    {
        public const string AccessDenied = "UserSaleRequest.AccessDenied";
        public const string AmountInvalid = "UserSaleRequest.AmountInvalid";
        public const string DescriptionRequired = "UserSaleRequest.DescriptionRequired";
        public const string EntityName = "UserSaleRequest.EntityName";
        public const string RejectionReasonRequired = "UserSaleRequest.RejectionReasonRequired";
        public const string StatusMustBeApproved = "UserSaleRequest.StatusMustBeApproved";
        public const string StatusMustBePending = "UserSaleRequest.StatusMustBePending";
        public const string TitleRequired = "UserSaleRequest.TitleRequired";
    }

    public static class PartSaleOperation
    {
        public const string EntityName = "PartSaleOperation.EntityName";
    }

    public static class User
    {
        public const string CannotInviteSelf = "User.CannotInviteSelf";
        public const string EntityName = "User.EntityName";
        public const string InvitationFailed = "User.InvitationFailed";
        public const string NotInvited = "User.NotInvited";
        public const string PasswordSetFailed = "User.PasswordSetFailed";
        public const string SystemUser = "User.SystemUser";
        public const string TargetHasHigherOrEqualRole = "User.TargetHasHigherOrEqualRole";
        public const string UnauthorizedRoleAssignment = "User.UnauthorizedRoleAssignment";
        public const string UnknownUser = "User.UnknownUser";
        public const string UpdateFailed = "User.UpdateFailed";
    }

    public static class Validation
    {
        public const string InvalidDateRange = "Validation.InvalidDateRange";
        public const string AddressTitleLength = "Validation.AddressTitleLength";
        public const string AddressTitleRequired = "Validation.AddressTitleRequired";
        public const string BirthDateInPast = "Validation.BirthDateInPast";
        public const string BirthDateInvalid = "Validation.BirthDateInvalid";
        public const string BirthDateRequired = "Validation.BirthDateRequired";
        public const string CapacityInvalid = "Validation.CapacityInvalid";
        public const string CityLength = "Validation.CityLength";
        public const string CityRequired = "Validation.CityRequired";
        public const string ConfirmPassword = "Validation.ConfirmPassword";
        public const string CriticalThresholdInvalid = "Validation.CriticalThresholdInvalid";
        public const string DescriptionLength = "Validation.DescriptionLength";
        public const string DescriptionRequired = "Validation.DescriptionRequired";
        public const string DistrictLength = "Validation.DistrictLength";
        public const string DistrictRequired = "Validation.DistrictRequired";
        public const string EmailRequired = "Validation.EmailRequired";
        public const string FacilityNameLength = "Validation.FacilityNameLength";
        public const string FacilityNameRequired = "Validation.FacilityNameRequired";
        public const string FirstNameLength = "Validation.FirstNameLength";
        public const string FirstNameRequired = "Validation.FirstNameRequired";
        public const string GenderRequired = "Validation.GenderRequired";
        public const string IdentifierRequired = "Validation.IdentifierRequired";
        public const string InvalidCoordinates = "Validation.InvalidCoordinates";
        public const string InvalidEmail = "Validation.InvalidEmail";
        public const string InvalidGender = "Validation.InvalidGender";
        public const string InvalidIdentifier = "Validation.InvalidIdentifier";
        public const string InvalidInvitationLink = "Validation.InvalidInvitationLink";
        public const string InvalidPageNumber = "Validation.InvalidPageNumber";
        public const string InvalidPageSize = "Validation.InvalidPageSize";
        public const string InvalidPhone = "Validation.InvalidPhone";
        public const string InvalidResetLink = "Validation.InvalidResetLink";
        public const string InvalidRole = "Validation.InvalidRole";
        public const string InvalidUserIdentifier = "Validation.InvalidUserIdentifier";
        public const string InvalidVerificationLink = "Validation.InvalidVerificationLink";
        public const string InvalidTC = "Validation.InvalidTC";
        public const string LastNameLength = "Validation.LastNameLength";
        public const string LastNameRequired = "Validation.LastNameRequired";
        public const string OpenAddressLength = "Validation.OpenAddressLength";
        public const string OpenAddressRequired = "Validation.OpenAddressRequired";
        public const string PageSizeExceeded = "Validation.PageSizeExceeded";
        public const string PasswordLower = "Validation.PasswordLower";
        public const string PasswordMinLength = "Validation.PasswordMinLength";
        public const string PasswordNumber = "Validation.PasswordNumber";
        public const string PasswordRequired = "Validation.PasswordRequired";
        public const string PasswordUpper = "Validation.PasswordUpper";
        public const string PhoneRequired = "Validation.PhoneRequired";
        public const string RefreshTokenRequired = "Validation.RefreshTokenRequired";
        public const string SearchTermLength = "Validation.SearchTermLength";
        public const string TargetRoleRequired = "Validation.TargetRoleRequired";
    }
}
