using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.UserSaleRequest.CreateUserSaleRequest;

public sealed class CreateUserSaleRequestCommandValidator : AbstractValidator<CreateUserSaleRequestCommand>
{
    public CreateUserSaleRequestCommandValidator()
    {
        RuleFor(command => command.Payload.Title).NotEmpty().WithMessage(ErrorMessages.UserSaleRequest.TitleRequired).MaximumLength(160);
        RuleFor(command => command.Payload.Description).NotEmpty().WithMessage(ErrorMessages.UserSaleRequest.DescriptionRequired).MaximumLength(2000);
        RuleFor(command => command.Payload.CategoryId).NotEmpty().WithMessage(ErrorMessages.Category.EntityName);
        RuleFor(command => command.Payload.UserEstimatedValue).GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.UserSaleRequest.AmountInvalid);
    }
}
