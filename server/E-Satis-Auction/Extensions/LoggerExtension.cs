using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Services;
using E_Satis_Auction.Common.Workers;

namespace E_Satis_Auction.Extensions;

public static class LoggerExtension
{
    public static IServiceCollection AddLogger(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IAuditLogQueue, AuditLogQueue>();
        services.AddHostedService<AuditLogWorker>();

        return services;
    }
}