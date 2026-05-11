using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Dispatch.ShipDispatch;

public sealed class ShipDispatchCommandValidator : AbstractValidator<ShipDispatchCommand>
{
    public ShipDispatchCommandValidator()
    {
        RuleFor(x => x.DispatchId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}