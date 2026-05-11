namespace E_Satis_Auction.Common;

public static class AppRoles
{
    public const string GeneralAdmin = "GeneralAdmin";
    public const string WarehouseManager = "WarehouseManager";
    public const string User = "User";

    public static readonly string[] AllRoles =
    {
        GeneralAdmin,
        WarehouseManager,
        User
    };
}