using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Product.AddProduct;

public sealed class AddProductCommandValidator : AbstractValidator<AddProductCommand>
{
    public AddProductCommandValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage(ErrorMessages.Product.SkuRequired)
            .MaximumLength(64).WithMessage(ErrorMessages.Product.SkuMaxLength)
            .Matches("^[A-Z0-9\\-]+$").WithMessage(ErrorMessages.Product.SkuInvalidFormat);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.Product.NameRequired)
            .MaximumLength(128).WithMessage(ErrorMessages.Product.NameMaxLength);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.UnitOfMeasure)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        When(x => x.BaseAttributes is not null && x.BaseAttributes.Count is not 0, () =>
        {
            RuleForEach(x => x.BaseAttributes).ChildRules(attributes =>
            {
                attributes.RuleFor(a => a.Key)
                    .NotEmpty().WithMessage(ErrorMessages.Product.AttributeKeyRequired);
                
                attributes.RuleFor(a => a.Value)
                    .NotEmpty().WithMessage(ErrorMessages.Product.AttributeValueRequired);
            });
        });
    }
}