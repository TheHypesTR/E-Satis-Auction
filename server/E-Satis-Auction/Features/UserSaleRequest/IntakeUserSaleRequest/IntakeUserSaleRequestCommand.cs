using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.UserSaleRequest.IntakeUserSaleRequest;

public sealed record IntakeUserSaleRequestCommand(Guid RequestId, IntakeUserSaleRequestRequest Payload) : ICommand<UserSaleRequestDto>;
