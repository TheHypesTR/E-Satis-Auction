using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Facility;

namespace e_Sat_Auction.Features.Facility.GetFacilityById;

public record GetFacilityByIdQuery(Guid Id) : IQuery<FacilityDetailDto>
{
}