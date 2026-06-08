using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.UserSaleRequest.ApproveUserSaleRequest;

public sealed record ApproveUserSaleRequestCommand(Guid RequestId, ApproveUserSaleRequestRequest Payload) : ICommand<UserSaleRequestDto>;
