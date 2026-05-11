using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Facility.AddFacility;

public class AddFacilityCommandValidator : AbstractValidator<AddFacilityCommand>
{
    public AddFacilityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.Validation.FacilityNameRequired)
            .Length(2, 255).WithMessage(ErrorMessages.Validation.FacilityNameLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorMessages.Validation.DescriptionRequired)
            .Length(10, 2000).WithMessage(ErrorMessages.Validation.DescriptionLength);

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.CapacityM3)
            .GreaterThan(0).WithMessage(ErrorMessages.Validation.CapacityInvalid);

        RuleFor(x => x.CriticalThresholdM3)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.Validation.CriticalThresholdInvalid);

        RuleFor(x => x.AddressTitle)
            .NotEmpty().WithMessage(ErrorMessages.Validation.AddressTitleRequired)
            .Length(2, 100).WithMessage(ErrorMessages.Validation.AddressTitleLength);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage(ErrorMessages.Validation.CityRequired)
            .Length(2, 100).WithMessage(ErrorMessages.Validation.CityLength);

        RuleFor(x => x.District)
            .NotEmpty().WithMessage(ErrorMessages.Validation.DistrictRequired)
            .Length(2, 100).WithMessage(ErrorMessages.Validation.DistrictLength);

        RuleFor(x => x.OpenAddress)
            .NotEmpty().WithMessage(ErrorMessages.Validation.OpenAddressRequired)
            .Length(10, 500).WithMessage(ErrorMessages.Validation.OpenAddressLength);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage(ErrorMessages.Validation.InvalidCoordinates);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage(ErrorMessages.Validation.InvalidCoordinates);
    }
}