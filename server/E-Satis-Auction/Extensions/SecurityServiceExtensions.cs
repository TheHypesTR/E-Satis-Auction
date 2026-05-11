using E_Satis_Auction.Common.Options;
using Microsoft.AspNetCore.HttpOverrides;

namespace E_Satis_Auction.Extensions;

public static class SecurityServiceExtensions
{
    public const string CorsPolicyName = "CorsPolicy";
    
    public static IServiceCollection AddProxyAndCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });
        
        ClientOptions? clientSettings = configuration.GetSection(ClientOptions.SectionName).Get<ClientOptions>();
        ArgumentException.ThrowIfNullOrWhiteSpace(clientSettings?.Url, nameof(ClientOptions.Url));
        
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                policy.WithOrigins(clientSettings.Url)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); 
            });
        });

        return services;
    }
}