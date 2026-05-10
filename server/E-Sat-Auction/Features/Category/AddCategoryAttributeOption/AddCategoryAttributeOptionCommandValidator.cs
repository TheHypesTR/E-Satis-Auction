using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Category.AddCategoryAttributeOption;

public sealed class AddCategoryAttributeOptionCommandValidator : AbstractValidator<AddCategoryAttributeOptionCommand>
{
    public AddCategoryAttributeOptionCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.AttributeId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage(ErrorMessages.Category.OptionValueRequired)
            .MaximumLength(256).WithMessage(ErrorMessages.Category.OptionValueMaxLength);
    }
}