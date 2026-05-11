using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.User;

namespace E_Satis_Auction.Features.Auth.GetMe;

public record GetMeQuery : IQuery<UserDto>;