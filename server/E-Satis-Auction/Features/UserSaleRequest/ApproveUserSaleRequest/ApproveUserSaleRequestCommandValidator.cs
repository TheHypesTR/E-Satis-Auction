using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.UserSaleRequest.ApproveUserSaleRequest;

public sealed class ApproveUserSaleRequestCommandValidator : AbstractValidator<ApproveUserSaleRequestCommand>
{
    public ApproveUserSaleRequestCommandValidator()
    {
        RuleFor(command => command.RequestId).NotEmpty().WithMessage(ErrorMessages.UserSaleRequest.EntityName);
        RuleFor(command => command.Payload.AcquisitionPrice).GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.UserSaleRequest.AmountInvalid);
        RuleFor(command => command.Payload.TargetResalePrice).GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.UserSaleRequest.AmountInvalid);
        RuleFor(command => command.Payload.AdminNote).MaximumLength(1024);
    }
}
