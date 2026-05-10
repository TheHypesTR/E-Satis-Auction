namespace e_Sat_Auction.Common.Helpers;

public static class RoleHierarchyHelper
{
    private static readonly Dictionary<string, int> RoleLevels = new()
    {
        { AppRoles.GeneralAdmin, 1 },
        { AppRoles.NGOAdmin, 2 },
        { AppRoles.WarehouseManager, 3 },
        { AppRoles.Volunteer, 4 },
        { AppRoles.User, 5 }
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