using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace E_Satis_Auction.Hubs;

[Authorize]
public sealed class AuctionHub : Hub
{
    public async Task JoinAuction(Guid auctionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(auctionId));
    }

    public async Task LeaveAuction(Guid auctionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(auctionId));
    }

    public static string GetGroupName(Guid auctionId)
    {
        return $"auction:{auctionId}";
    }
}
