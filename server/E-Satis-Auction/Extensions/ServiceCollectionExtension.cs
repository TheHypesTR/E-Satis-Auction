using E_Satis_Auction.Common.Interceptors;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Data;
using E_Satis_Auction.Data.Repositories;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;
using E_Satis_Auction.Services;

namespace E_Satis_Auction.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<DispatchDomainEventsInterceptor>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<IFacilityManagerRepository, FacilityManagerRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryAttributeRepository, CategoryAttributeRepository>();
        services.AddScoped<ICategoryAttributeOptionRepository, CategoryAttributeOptionRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IDispatchRepository, DispatchRepository>();
        services.AddScoped<IProductListingRepository, ProductListingRepository>();
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<IPaymentAttemptRepository, PaymentAttemptRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        services.AddScoped<IAuctionBidRepository, AuctionBidRepository>();
        services.AddScoped<IReturnRequestRepository, ReturnRequestRepository>();
        services.AddScoped<IUserSaleRequestRepository, UserSaleRequestRepository>();
        services.AddScoped<IPartSaleOperationRepository, PartSaleOperationRepository>();
        
        services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
        
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICommerceWorkflowService, CommerceWorkflowService>();
        services.AddScoped<IAuctionWorkflowService, AuctionWorkflowService>();
        services.AddScoped<IAuctionRealtimeNotifier, AuctionRealtimeNotifier>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserInvitationService, UserInvitationService>();
        services.AddHostedService<PaymentReservationExpirationService>();
        services.AddHostedService<AuctionLifecycleService>();

        return services;
    }
}
