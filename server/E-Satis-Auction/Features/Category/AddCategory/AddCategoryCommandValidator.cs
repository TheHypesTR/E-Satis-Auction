using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Enums;
using FluentValidation;

namespace E_Satis_Auction.Features.Category.AddCategory;

public class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand>
{
    public AddCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.Category.NameRequired)
            .MaximumLength(128).WithMessage(ErrorMessages.Category.NameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage(ErrorMessages.Category.DescriptionMaxLength);

        When(x => x.Attributes is not null && x.Attributes.Count is not 0, () =>
        {
            RuleFor(x => x.Attributes)
                .Must(attributes => attributes!
                    .Select(a => a.Code.ToSemanticCode())
                    .Distinct()
                    .Count() == attributes!.Count)
                .WithMessage(ErrorMessages.Category.DuplicateAttributeCodeInRequest);

            RuleForEach(x => x.Attributes).ChildRules(attribute =>
            {
                attribute.RuleFor(a => a.Name)
                    .NotEmpty().WithMessage(ErrorMessages.Category.AttributeNameRequired)
                    .MaximumLength(128).WithMessage(ErrorMessages.Category.AttributeNameMaxLength);

                attribute.RuleFor(a => a.Code)
                    .NotEmpty().WithMessage(ErrorMessages.Category.AttributeCodeRequired)
                    .MaximumLength(128).WithMessage(ErrorMessages.Category.AttributeCodeMaxLength)
                    .Matches("^[a-zA-Z0-9_ğüşöçİĞÜŞÖÇ ]+$").WithMessage(ErrorMessages.Category.AttributeCodeInvalidCharacters);

                attribute.RuleFor(a => a.DataType)
                    .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

                attribute.RuleFor(a => a.Target)
                    .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
                
                attribute.RuleFor(a => a.Options)
                    .NotEmpty().WithMessage(ErrorMessages.Category.OptionValueRequired)
                    .When(a => a.DataType == AttributeDataType.SelectList);

                attribute.When(a => a.Options is not null && a.Options.Count is not 0, () =>
                {
                    attribute.RuleForEach(a => a.Options)
                        .NotEmpty().WithMessage(ErrorMessages.Category.OptionValueRequired)
                        .MaximumLength(256).WithMessage(ErrorMessages.Category.OptionValueMaxLength);
                });
            });
        });
    }
}