using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.User;

namespace e_Sat_Auction.Features.Auth.GetMe;

public record GetMeQuery : IQuery<UserDto>;