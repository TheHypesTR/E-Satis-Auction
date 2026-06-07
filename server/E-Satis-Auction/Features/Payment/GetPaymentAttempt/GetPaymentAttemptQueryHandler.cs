using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Payment.GetPaymentAttempt;

public sealed class GetPaymentAttemptQueryHandler : IQueryHandler<GetPaymentAttemptQuery, PaymentAttemptDto>
{
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPaymentAttemptQueryHandler(IPaymentAttemptRepository paymentAttemptRepository, ICurrentUserService currentUserService)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PaymentAttemptDto> Handle(GetPaymentAttemptQuery query, CancellationToken cancellationToken)
    {
        Models.Commerce.PaymentAttempt? payment = await _paymentAttemptRepository.GetByIdForUserAsync(query.PaymentAttemptId, _currentUserService.UserId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(payment, ErrorMessages.Payment.EntityName, query.PaymentAttemptId);

        return CommerceDtoMapper.ToPaymentAttemptDto(payment!);
    }
}
