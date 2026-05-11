using E_Satis_Auction.Common.Interceptors;
using E_Satis_Auction.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace E_Satis_Auction.Extensions;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string 'DefaultConnection' is not found.");
        }

        NpgsqlDataSourceBuilder dataSourceBuilder = new(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        
        NpgsqlDataSource dataSource = dataSourceBuilder.Build();
        
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            DispatchDomainEventsInterceptor domainEventsInterceptor = sp.GetRequiredService<DispatchDomainEventsInterceptor>();
            
            options.UseNpgsql(dataSource).AddInterceptors(domainEventsInterceptor);
        });
        
        services.AddApplicationServices();
        
        return services;
    }
}