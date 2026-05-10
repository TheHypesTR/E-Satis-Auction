using e_Sat_Auction.Common.Options;

namespace e_Sat_Auction.Extensions;

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