using E_Satis_Auction.Common.Options;

namespace E_Satis_Auction.Extensions;

public static class OptionsConfigurationExtension
{
    public static IServiceCollection ConfigureCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<ClientOptions>(configuration.GetSection(ClientOptions.SectionName));
        services.Configure<SmtpMailOptions>(configuration.GetSection(SmtpMailOptions.SectionName));

        return services;
    }
}