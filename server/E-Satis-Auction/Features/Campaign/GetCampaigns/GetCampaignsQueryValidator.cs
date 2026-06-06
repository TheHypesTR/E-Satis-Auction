using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.Campaign.GetCampaigns;

public sealed class GetCampaignsQueryValidator : PaginatedQueryValidator<GetCampaignsQuery>
{
    public GetCampaignsQueryValidator()
    {
        RuleFor(query => query.Status).IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier).When(query => query.Status.HasValue);
        RuleFor(query => query.Scope).IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier).When(query => query.Scope.HasValue);
    }
}
