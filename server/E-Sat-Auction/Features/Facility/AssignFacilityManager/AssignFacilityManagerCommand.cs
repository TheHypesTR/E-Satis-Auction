using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Manager.Requests;

namespace e_Sat_Auction.Features.Facility.AssignFacilityManager;

public record AssignFacilityManagerCommand(
    Guid FacilityId,
    string Email,
    string FirstName,
    string LastName,
    bool IsPrimary = false) : IAuditableCommand
{
    public AssignFacilityManagerCommand(Guid facilityId, AssignManagerRequest request)
        : this(facilityId, request.Email, request.FirstName, request.LastName, request.IsPrimary)
    {
    }
};