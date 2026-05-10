using e_Sat_Auction.Common.Interceptors;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Data;
using e_Sat_Auction.Data.Repositories;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Services;

namespace e_Sat_Auction.Extensions;

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
        
        services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
        
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserInvitationService, UserInvitationService>();

        return services;
    }
}