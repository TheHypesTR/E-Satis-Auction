using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.User;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace E_Satis_Auction.Features.User.GetUserById;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly UserManager<AppUser> _userManager;

    public GetUserByIdQueryHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByIdAsync(query.Id.ToString());
        NotFoundException.ThrowIfNull(user, ErrorMessages.User.EntityName, query.Id);

        IList<string> roles = await _userManager.GetRolesAsync(user!);
        
        return new UserDto(
            user!.Id,
            user.FirstName,
            user.LastName,
            user.Email!,
            user.PhoneNumber!,
            user.TCNumber,
            roles,
            user.UserStatus.ToString(),
            user.Gender.ToString(),
            user.BirthDate);
    }
}