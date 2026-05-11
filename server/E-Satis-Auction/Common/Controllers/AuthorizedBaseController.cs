using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace E_Satis_Auction.Common.Controllers;

[Authorize]
public class AuthorizedBaseController : BaseController
{
    protected Guid CurrentUserId
    {
        get
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Guid.Empty;
            }

            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
        }
    }
}