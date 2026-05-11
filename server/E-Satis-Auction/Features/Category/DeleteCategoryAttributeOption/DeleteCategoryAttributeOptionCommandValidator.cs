using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Category.DeleteCategoryAttributeOption;

public sealed class DeleteCategoryAttributeOptionCommandValidator : AbstractValidator<DeleteCategoryAttributeOptionCommand>
{
    public DeleteCategoryAttributeOptionCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.AttributeId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.OptionId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}