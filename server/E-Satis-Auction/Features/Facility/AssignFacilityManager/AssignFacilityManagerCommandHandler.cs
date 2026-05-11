using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Options;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Users;
using Microsoft.Extensions.Options;

namespace E_Satis_Auction.Features.Facility.AssignFacilityManager;

using Models.Facilities;

public class AssignFacilityManagerCommandHandler : ICommandHandler<AssignFacilityManagerCommand>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityManagerRepository _facilityManagerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserInvitationService _userInvitationService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ClientOptions _clientOptions;
    
    private const string ADMIN_DASHBOARD = "Admin";
    
    public AssignFacilityManagerCommandHandler(
        IFacilityRepository facilityRepository,
        IFacilityManagerRepository facilityManagerRepository,
        ICurrentUserService currentUserService,
        IUserInvitationService userInvitationService,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IOptions<ClientOptions> clientOptions)
    {
        _facilityRepository = facilityRepository;
        _facilityManagerRepository = facilityManagerRepository;
        _currentUserService = currentUserService;
        _userInvitationService = userInvitationService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _clientOptions = clientOptions.Value;
    }

    public async Task Handle(AssignFacilityManagerCommand command, CancellationToken cancellationToken)
    {
        Facility? facility = await _facilityRepository.GetByIdAsync(command.FacilityId, false, cancellationToken);
        NotFoundException.ThrowIfNull(facility, ErrorMessages.Facility.EntityName, command.FacilityId);
        
        AppUser user = await _userInvitationService.GetOrAddInvitedUserAsync(
            command.Email,
            command.FirstName,
            command.LastName,
            AppRoles.WarehouseManager);
        
        bool isAlreadyManager = await _facilityManagerRepository.IsManagerExistsAsync(command.FacilityId, user.Id, cancellationToken);
        BusinessException.ThrowIfTrue(
            isAlreadyManager,
            ErrorMessages.Facility.ManagerAlreadyExists,
            ErrorMessages.Exception.RoleAssignmentTitle);
        
        bool isPrimary = await DetermineAndHandlePrimaryStatusAsync(facility!.Id, command.IsPrimary, cancellationToken);
        
        FacilityManager facilityManager = FacilityManager.Create(facility.Id, user.Id, isPrimary);
        await _facilityManagerRepository.AddAsync(facilityManager, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        string adminDashboardUrl = $"{_clientOptions.Url}/{ADMIN_DASHBOARD}";
        if (user.UserStatus is UserStatus.Active)
        {
            await _emailService.SendFacilityAssignedEmailAsync(user.Email!, user.FirstName, facility.Name, adminDashboardUrl);
        }
    }
    
    private async Task<bool> DetermineAndHandlePrimaryStatusAsync(Guid facilityId, bool requestedIsPrimary, CancellationToken cancellationToken)
    {
        FacilityManager? currentPrimaryManager = await _facilityManagerRepository.GetPrimaryManagerAsync(facilityId, cancellationToken);
        if (currentPrimaryManager is null)
        {
            return true;
        }

        if (requestedIsPrimary)
        {
            currentPrimaryManager.DemoteFromPrimary();
            _facilityManagerRepository.Update(currentPrimaryManager);
            
            return true;
        }

        return false;
    }
}