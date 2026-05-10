using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Category.UpdateCategory;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.Category.NameRequired)
            .MaximumLength(128).WithMessage(ErrorMessages.Category.NameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage(ErrorMessages.Category.DescriptionMaxLength);
    }
}