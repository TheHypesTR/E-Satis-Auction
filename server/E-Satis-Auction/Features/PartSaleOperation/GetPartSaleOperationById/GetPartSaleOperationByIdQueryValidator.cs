using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.PartSaleOperation.GetPartSaleOperationById;

public sealed class GetPartSaleOperationByIdQueryValidator : AbstractValidator<GetPartSaleOperationByIdQuery>
{
    public GetPartSaleOperationByIdQueryValidator()
    {
        RuleFor(query => query.OperationId).NotEmpty().WithMessage(ErrorMessages.PartSaleOperation.EntityName);
    }
}
