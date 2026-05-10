using Microsoft.AspNetCore.Authorization;

namespace e_Sat_Auction.Common.Attributes;

public class RequireRolesAttribute : AuthorizeAttribute
{
    public RequireRolesAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}