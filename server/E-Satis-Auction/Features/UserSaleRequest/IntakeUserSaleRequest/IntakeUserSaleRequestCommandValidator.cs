using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.UserSaleRequest.IntakeUserSaleRequest;

public sealed class IntakeUserSaleRequestCommandValidator : AbstractValidator<IntakeUserSaleRequestCommand>
{
    public IntakeUserSaleRequestCommandValidator()
    {
        RuleFor(command => command.RequestId).NotEmpty().WithMessage(ErrorMessages.UserSaleRequest.EntityName);
        RuleFor(command => command.Payload.AdminNote).MaximumLength(1024);
    }
}
