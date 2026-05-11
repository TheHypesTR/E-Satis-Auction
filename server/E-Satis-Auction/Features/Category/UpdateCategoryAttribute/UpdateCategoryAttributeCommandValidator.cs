using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Category.UpdateCategoryAttribute;

public sealed class UpdateCategoryAttributeCommandValidator : AbstractValidator<UpdateCategoryAttributeCommand>
{
    public UpdateCategoryAttributeCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.AttributeId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.Category.AttributeNameRequired)
            .MaximumLength(128).WithMessage(ErrorMessages.Category.AttributeNameMaxLength);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(ErrorMessages.Category.AttributeCodeRequired)
            .MaximumLength(128).WithMessage(ErrorMessages.Category.AttributeCodeMaxLength)
            .Matches("^[a-zA-Z0-9_ğüşöçİĞÜŞÖÇ ]+$").WithMessage(ErrorMessages.Category.AttributeCodeInvalidCharacters);

        RuleFor(a => a.DataType)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(a => a.Target)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}