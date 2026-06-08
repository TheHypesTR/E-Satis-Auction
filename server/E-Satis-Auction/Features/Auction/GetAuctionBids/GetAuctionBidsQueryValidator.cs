using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.Auction.GetAuctionBids;

public sealed class GetAuctionBidsQueryValidator : PaginatedQueryValidator<GetAuctionBidsQuery>
{
    public GetAuctionBidsQueryValidator()
    {
        RuleFor(query => query.AuctionId)
            .NotEmpty().WithMessage(ErrorMessages.Auction.EntityName);
    }
}
