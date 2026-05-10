using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Services;
using e_Sat_Auction.Common.Workers;

namespace e_Sat_Auction.Extensions;

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