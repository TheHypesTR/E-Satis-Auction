using E_Satis_Auction.Common.Options;
using Microsoft.AspNetCore.HttpOverrides;

namespace E_Satis_Auction.Extensions;

public static class SecurityServiceExtensions
{
    public const string CorsPolicyName = "CorsPolicy";
    private const string CorsAllowedOriginsSection = "Cors:AllowedOrigins";
    
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

        string[] allowedOrigins = configuration
            .GetSection(CorsAllowedOriginsSection)
            .Get<string[]>() ?? [];

        allowedOrigins = allowedOrigins
            .Append(clientSettings.Url)
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); 
            });
        });

        return services;
    }
}
