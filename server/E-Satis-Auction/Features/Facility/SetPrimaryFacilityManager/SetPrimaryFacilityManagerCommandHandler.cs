using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Facility.SetPrimaryFacilityManager;

using Models.Facilities;

public class SetPrimaryFacilityManagerCommandHandler : ICommandHandler<SetPrimaryFacilityManagerCommand>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityManagerRepository _facilityManagerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetPrimaryFacilityManagerCommandHandler(
        IFacilityRepository facilityRepository,
        IFacilityManagerRepository facilityManagerRepository,
        IUnitOfWork unitOfWork)
    {
        _facilityRepository = facilityRepository;
        _facilityManagerRepository = facilityManagerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetPrimaryFacilityManagerCommand command, CancellationToken cancellationToken)
    {
        Facility? facility = await _facilityRepository.GetByIdAsync(command.FacilityId, false, cancellationToken);
        NotFoundException.ThrowIfNull(facility, ErrorMessages.Facility.EntityName, command.FacilityId);
        
        FacilityManager? targetManager = await _facilityManagerRepository
            .FindManagerAsync(command.FacilityId, command.UserId, cancellationToken);
        NotFoundException.ThrowIfNull(targetManager, ErrorMessages.User.EntityName, command.UserId);
        
        BusinessException.ThrowIfTrue(
            targetManager!.IsPrimary, 
            ErrorMessages.Facility.AlreadyPrimaryManager, 
            ErrorMessages.Exception.RoleAssignmentTitle);
        
        FacilityManager? currentPrimary = await _facilityManagerRepository.GetPrimaryManagerAsync(command.FacilityId, cancellationToken);
        if (currentPrimary is not null)
        {
            currentPrimary.DemoteFromPrimary();
            _facilityManagerRepository.Update(currentPrimary);
        }
        
        targetManager.PromoteToPrimary();
        _facilityManagerRepository.Update(targetManager);
        
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}