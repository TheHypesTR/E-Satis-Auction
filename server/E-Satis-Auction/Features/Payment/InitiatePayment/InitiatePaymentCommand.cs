using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.Payment.InitiatePayment;

public sealed record InitiatePaymentCommand(InitiatePaymentRequest Payload) : ICommand<PaymentInitiationDto>;
