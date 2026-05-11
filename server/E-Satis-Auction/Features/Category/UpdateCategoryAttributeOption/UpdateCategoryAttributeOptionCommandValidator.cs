using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Category.UpdateCategoryAttributeOption;

public sealed class UpdateCategoryAttributeOptionCommandValidator : AbstractValidator<UpdateCategoryAttributeOptionCommand>
{
    public UpdateCategoryAttributeOptionCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.AttributeId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.OptionId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage(ErrorMessages.Category.OptionValueRequired)
            .MaximumLength(256).WithMessage(ErrorMessages.Category.OptionValueMaxLength);
    }
}