using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Enums;
using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.Common;

namespace e_Sat_Auction.Features.Facility.AddFacility;

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