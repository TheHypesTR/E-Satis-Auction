using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Interfaces.Repositories;

namespace e_Sat_Auction.Features.Facility.DeleteFacility;

using Models.Facilities;

public class DeleteFacilityCommandHandler : ICommandHandler<DeleteFacilityCommand>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFacilityCommandHandler(
        IFacilityRepository facilityRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _facilityRepository = facilityRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteFacilityCommand command, CancellationToken cancellationToken)
    {
        Facility? facility = await _facilityRepository.GetWithDependentsAsync(command.Id, cancellationToken);
        NotFoundException.ThrowIfNull(facility, ErrorMessages.Facility.EntityName, command.Id);

        ApplySoftDeleteToHierarchy(facility!);

        _facilityRepository.Update(facility!);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }

    private static void ApplySoftDeleteToHierarchy(Facility facility)
    {
        facility.Delete();
        facility.Address.Delete();

        foreach (FacilityManager manager in facility.Managers)
        {
            manager.Delete();
        }
    }
}