using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Address;
using E_Satis_Auction.Dtos.Dispatch;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Dispatch.GetDispatchById;

using Models.Common;
using Models.Dispatches;

public sealed class GetDispatchByIdQueryHandler : IQueryHandler<GetDispatchByIdQuery, DispatchDetailDto>
{
    private readonly IDispatchRepository _dispatchRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDispatchByIdQueryHandler(
        IDispatchRepository dispatchRepository,
        IFacilityRepository facilityRepository,
        IAddressRepository addressRepository,
        ICurrentUserService currentUserService)
    {
        _dispatchRepository = dispatchRepository;
        _facilityRepository = facilityRepository;
        _addressRepository = addressRepository;
        _currentUserService = currentUserService;
    }

    public async Task<DispatchDetailDto> Handle(GetDispatchByIdQuery query, CancellationToken cancellationToken)
    {
        Dispatch? dispatch = await _dispatchRepository.GetByIdWithLineItemsAsync(query.Id, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(dispatch, ErrorMessages.Dispatch.EntityName, query.Id);

        await EnsureAuthorizedAsync(dispatch!, cancellationToken);

        Dictionary<Guid, string> facilityNames = await _facilityRepository.GetFacilityNamesByIdsAsync(GetFacilityIds(dispatch!), cancellationToken);

        AddressDto? addressDto = null;
        if (dispatch!.TargetAddressId.HasValue)
        {
            Address? address = await _addressRepository.GetByIdAsync(dispatch.TargetAddressId.Value, enableTracking: false, cancellationToken);
            NotFoundException.ThrowIfNull(address, ErrorMessages.Address.EntityName, dispatch.TargetAddressId.Value);
            
            addressDto = MapAddress(address!);
        }

        IReadOnlyCollection<DispatchLineItemDto> lineItems = dispatch.LineItems
            .Select(li => new DispatchLineItemDto(li.SourceItemId, li.ItemNameSnapshot, li.Quantity))
            .ToList();

        return MapToDetailDto(dispatch, facilityNames, addressDto, lineItems);
    }

    private async Task EnsureAuthorizedAsync(Dispatch dispatch, CancellationToken cancellationToken)
    {
        if (_currentUserService.IsGeneralAdmin)
        {
            return;
        }

        bool hasSourceAccess = await _currentUserService.HasFacilityAccess(dispatch.SourceFacilityId, cancellationToken);
        bool hasTargetAccess = dispatch.TargetFacilityId.HasValue &&
            await _currentUserService.HasFacilityAccess(dispatch.TargetFacilityId.Value, cancellationToken);

        ForbiddenAccessException.ThrowIfFalse(
            hasSourceAccess || hasTargetAccess,
            ErrorMessages.Facility.UnauthorizedFacilityAccess,
            ErrorMessages.Exception.UnauthorizedAccess);
    }

    private static IEnumerable<Guid> GetFacilityIds(Dispatch dispatch)
    {
        if (dispatch.TargetFacilityId.HasValue)
        {
            return [dispatch.SourceFacilityId, dispatch.TargetFacilityId.Value];
        }

        return [dispatch.SourceFacilityId];
    }

    private static AddressDto MapAddress(Address address)
    {
        return new AddressDto(
            address.Title,
            address.City,
            address.District,
            address.OpenAddress,
            address.Latitude,
            address.Longitude,
            address.IsTemporary);
    }

    private static DispatchDetailDto MapToDetailDto(Dispatch dispatch, Dictionary<Guid, string> facilityNames, AddressDto? addressDto, IReadOnlyCollection<DispatchLineItemDto> lineItems)
    {
        return new DispatchDetailDto(
            dispatch.Id,
            dispatch.TrackingNumber,
            dispatch.Status,
            dispatch.SourceFacilityId,
            facilityNames.GetValueOrDefault(dispatch.SourceFacilityId, string.Empty),
            dispatch.TargetFacilityId,
            dispatch.TargetFacilityId.HasValue ? facilityNames.GetValueOrDefault(dispatch.TargetFacilityId.Value, string.Empty) : null,
            dispatch.TargetAddressId,
            addressDto,
            dispatch.ReceiverName,
            dispatch.ReceiverPhone,
            dispatch.Notes,
            dispatch.DeliveryNote,
            dispatch.DispatchDate,
            lineItems,
            dispatch.CreatedAt,
            dispatch.UpdatedAt);
    }
}