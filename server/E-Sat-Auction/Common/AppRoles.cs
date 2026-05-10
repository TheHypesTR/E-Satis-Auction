namespace e_Sat_Auction.Common;

public static class AppRoles
{
    public const string GeneralAdmin = "GeneralAdmin";
    public const string NGOAdmin = "NGOAdmin";
    public const string WarehouseManager = "WarehouseManager";
    public const string Volunteer = "Volunteer";
    public const string User = "User";

    public static readonly string[] AllRoles =
    {
        GeneralAdmin,
        NGOAdmin,
        WarehouseManager,
        Volunteer,
        User
    };
}