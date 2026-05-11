using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Facility;

namespace E_Satis_Auction.Features.Facility.GetFacilityById;

public record GetFacilityByIdQuery(Guid Id) : IQuery<FacilityDetailDto>
{
}