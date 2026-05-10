using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Item.AddAdHocItem;

public class AddAdHocItemCommandValidator : AbstractValidator<AddAdHocItemCommand>
{
    public AddAdHocItemCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
        
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.Item.NameRequiredForAdHoc)
            .MaximumLength(256).WithMessage(ErrorMessages.Item.NameMaxLength);
        
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