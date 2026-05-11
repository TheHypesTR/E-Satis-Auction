using E_Satis_Auction.Models.Users;

namespace E_Satis_Auction.Interfaces;

public interface IUserInvitationService
{
    Task<AppUser> GetOrAddInvitedUserAsync(string email, string firstName, string lastName, string targetRole);
}