using e_Sat_Auction.Models.Users;

namespace e_Sat_Auction.Interfaces;

public interface IUserInvitationService
{
    Task<AppUser> GetOrAddInvitedUserAsync(string email, string firstName, string lastName, string targetRole);
}