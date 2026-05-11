namespace E_Satis_Auction.Common.Helpers;

public static class RoleHierarchyHelper
{
    private static readonly Dictionary<string, int> RoleLevels = new()
    {
        { AppRoles.GeneralAdmin, 1 },
        { AppRoles.WarehouseManager, 2 },
        { AppRoles.User, 3 }
    };

    public static bool CanAssignRole(IList<string> inviterRoles, string targetRole)
    {
        if (!RoleLevels.TryGetValue(targetRole, out int targetLevel))
            return false;

        int inviterHighestLevel = inviterRoles
            .Where(r => RoleLevels.ContainsKey(r))
            .Select(r => RoleLevels[r])
            .DefaultIfEmpty(99)
            .Min();

        return inviterHighestLevel <= targetLevel;
    }

    public static bool HasHigherOrEqualRole(IList<string> targetUserRoles, string targetRole)
    {
        if (!RoleLevels.TryGetValue(targetRole, out int targetLevel))
            return false;

        int targetUserHighestLevel = targetUserRoles
            .Where(r => RoleLevels.ContainsKey(r))
            .Select(r => RoleLevels[r])
            .DefaultIfEmpty(99)
            .Min();

        return targetUserHighestLevel <= targetLevel;
    }
}