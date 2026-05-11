using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Common;

namespace E_Satis_Auction.Features.Facility.AddFacility;

public class AddFacilityCommandHandler : ICommandHandler<AddFacilityCommand, Guid>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddFacilityCommandHandler(
        IFacilityRepository facilityRepository,
        IAddressRepository addressRepository,
        IUnitOfWork unitOfWork)
    {
        _facilityRepository = facilityRepository;
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddFacilityCommand command, CancellationToken cancellationToken)
    {
        Address address = Address.Add(
            command.AddressTitle,
            command.City,
            command.District,
            command.OpenAddress,
            command.Latitude,
            command.Longitude);

        Models.Facilities.Facility depot = Models.Facilities.Facility.Add(
            command.Name,
            command.Description,
            ApprovalStatus.Approved,
            command.CapacityM3,
            command.CriticalThresholdM3,
            address.Id);

        await _addressRepository.AddAsync(address, cancellationToken);
        await _facilityRepository.AddAsync(depot, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return depot.Id;
    }
}