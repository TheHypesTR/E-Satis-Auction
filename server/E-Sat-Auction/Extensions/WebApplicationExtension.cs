using e_Sat_Auction.Data.Seed;

namespace e_Sat_Auction.Extensions;

public static class WebApplicationExtension
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        try
        {
            await SeedData.InitializeAsync(services);
        }
        catch (Exception ex)
        {
            ILogger logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}