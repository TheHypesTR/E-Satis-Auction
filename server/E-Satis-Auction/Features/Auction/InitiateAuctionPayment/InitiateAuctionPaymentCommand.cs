using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Auction.Requests;

namespace E_Satis_Auction.Features.Auction.InitiateAuctionPayment;

public sealed record InitiateAuctionPaymentCommand(Guid AuctionId, InitiateAuctionPaymentRequest Payload) : ICommand<PaymentInitiationDto>;
