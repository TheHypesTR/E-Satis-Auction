using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Dispatch.ShipDispatch;

public sealed class ShipDispatchCommandValidator : AbstractValidator<ShipDispatchCommand>
{
    public ShipDispatchCommandValidator()
    {
        RuleFor(x => x.DispatchId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}