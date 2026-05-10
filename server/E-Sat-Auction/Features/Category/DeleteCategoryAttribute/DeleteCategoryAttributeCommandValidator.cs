using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Category.DeleteCategoryAttribute;

public sealed class DeleteCategoryAttributeCommandValidator : AbstractValidator<DeleteCategoryAttributeCommand>
{
    public DeleteCategoryAttributeCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.AttributeId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}