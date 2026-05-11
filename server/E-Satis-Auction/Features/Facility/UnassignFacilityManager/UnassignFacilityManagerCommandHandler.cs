using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Facility.UnassignFacilityManager;

using Models.Facilities;

public class UnassignFacilityManagerCommandHandler : ICommandHandler<UnassignFacilityManagerCommand>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityManagerRepository _facilityManagerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnassignFacilityManagerCommandHandler(
        IFacilityRepository facilityRepository,
        IFacilityManagerRepository facilityManagerRepository,
        IUnitOfWork unitOfWork)
    {
        _facilityRepository = facilityRepository;
        _facilityManagerRepository = facilityManagerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UnassignFacilityManagerCommand command, CancellationToken cancellationToken)
    {
        Facility? facility = await _facilityRepository.GetByIdAsync(command.FacilityId, false, cancellationToken);
        NotFoundException.ThrowIfNull(facility, ErrorMessages.Facility.EntityName, command.FacilityId);

        FacilityManager? facilityManager = await _facilityManagerRepository
            .FindManagerAsync(command.FacilityId, command.UserId, cancellationToken);
        NotFoundException.ThrowIfNull(facilityManager, ErrorMessages.User.EntityName, command.UserId);
        
        facilityManager!.Delete();
        _facilityManagerRepository.Update(facilityManager);
        
        if (facilityManager.IsPrimary)
        {
            await HandlePrimaryManagerReassignmentAsync(command.FacilityId, command.UserId, cancellationToken);
        }
        
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
    
    private async Task HandlePrimaryManagerReassignmentAsync(Guid facilityId, string excludedUserId, CancellationToken cancellationToken)
    {
        FacilityManager? oldestManager = await _facilityManagerRepository
            .GetOldestManagerAsync(facilityId, excludedUserId, cancellationToken);

        if (oldestManager is not null)
        {
            oldestManager.PromoteToPrimary();
            _facilityManagerRepository.Update(oldestManager);
        }
    }
}