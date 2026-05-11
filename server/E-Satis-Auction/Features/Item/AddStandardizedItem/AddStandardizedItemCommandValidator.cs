using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Item.AddStandardizedItem;

public class AddStandardizedItemCommandValidator : AbstractValidator<AddStandardizedItemCommand>
{
    public AddStandardizedItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
        
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
        
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.Item.QuantityCannotBeNegative);
        
        RuleFor(x => x.UnitOfMeasure)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
        
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        When(x => x.DynamicAttributes is not null && x.DynamicAttributes.Count is not 0, () =>
        {
            RuleForEach(x => x.DynamicAttributes).ChildRules(attributes =>
            {
                attributes.RuleFor(a => a.Key)
                    .NotEmpty().WithMessage(ErrorMessages.Item.DynamicAttributeKeyRequired);
                
                attributes.RuleFor(a => a.Value)
                    .NotEmpty().WithMessage(ErrorMessages.Item.DynamicAttributeValueRequired);
            });
        });
    }
}