using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.User;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace e_Sat_Auction.Features.Auth.GetMe;

public class GetMeQueryHandler : IQueryHandler<GetMeQuery, UserDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public GetMeQueryHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> Handle(GetMeQuery query, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByIdAsync(_currentUserService.UserId);
        NotFoundException.ThrowIfNull(user, ErrorMessages.User.EntityName, _currentUserService.UserId);

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