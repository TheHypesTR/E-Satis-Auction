using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.UserSaleRequest.RejectUserSaleRequest;

public sealed class RejectUserSaleRequestCommandValidator : AbstractValidator<RejectUserSaleRequestCommand>
{
    public RejectUserSaleRequestCommandValidator()
    {
        RuleFor(command => command.RequestId).NotEmpty().WithMessage(ErrorMessages.UserSaleRequest.EntityName);
        RuleFor(command => command.Payload.Reason).NotEmpty().WithMessage(ErrorMessages.UserSaleRequest.RejectionReasonRequired).MaximumLength(1024);
    }
}
