using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Dispatch;

public sealed record DispatchSummaryDto(
    Guid Id,
    string TrackingNumber,
    DispatchStatus Status,
    Guid SourceFacilityId,
    string SourceFacilityName,
    Guid? TargetFacilityId,
    string? TargetFacilityName,
    Guid? TargetAddressId,
    string ReceiverName,
    string ReceiverPhone,
    DateTimeOffset? DispatchDate,
    DateTime CreatedAt,
    DateTime UpdatedAt);