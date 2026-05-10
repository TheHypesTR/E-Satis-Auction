using System.Threading.RateLimiting;
using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace e_Sat_Auction.Extensions;

public static class RateLimitingExtension
{
    public const string STRICT_AUTH_POLICY = "StrictAuthPolicy";

    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddPolicy(STRICT_AUTH_POLICY, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(3),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }));

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                IStringLocalizer<SharedResource> localizer =
                    context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();

                ProblemDetails problemDetails = new()
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = localizer[ErrorMessages.Exception.TooManyRequestsTitle].Value,
                    Detail = localizer[ErrorMessages.Exception.TooManyRequestsDetail].Value
                };

                await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            };
        });

        return services;
    }
}