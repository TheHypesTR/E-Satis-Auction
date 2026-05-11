using E_Satis_Auction.Common;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Seed;

public static class SeedData
{
    private const string DefaultAdminEmail = "admin@esatisauction.com";
    private const string DefaultAdminPassword = "Admin123.";

    private static readonly string[] Roles =
    {
        AppRoles.GeneralAdmin,
        AppRoles.WarehouseManager,
        AppRoles.User
    };

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        AppDbContext dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();

        RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<AppUser> userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        foreach (string role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (await userManager.FindByEmailAsync(DefaultAdminEmail) is null)
        {
            AppUser adminUser = AppUser.Add(
                "Admin",
                "e-Satis",
                DefaultAdminEmail,
                "+905441230000",
                null,
                Gender.PreferNotToSay,
                new DateTime(2000, 1, 1));

            adminUser.UserName = DefaultAdminEmail;
            adminUser.EmailConfirmed = true;

            IdentityResult result = await userManager.CreateAsync(adminUser, DefaultAdminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, AppRoles.GeneralAdmin);
            }
        }
    }
}