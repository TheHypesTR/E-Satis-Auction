using E_Satis_Auction.Dtos.Address;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Dispatch;

public sealed record DispatchDetailDto(
    Guid Id,
    string TrackingNumber,
    DispatchStatus Status,
    Guid SourceFacilityId,
    string SourceFacilityName,
    Guid? TargetFacilityId,
    string? TargetFacilityName,
    Guid? TargetAddressId,
    AddressDto? TargetAddress,
    string ReceiverName,
    string ReceiverPhone,
    string? Notes,
    string? DeliveryNote,
    DateTimeOffset? DispatchDate,
    IReadOnlyCollection<DispatchLineItemDto> LineItems,
    DateTime CreatedAt,
    DateTime UpdatedAt);