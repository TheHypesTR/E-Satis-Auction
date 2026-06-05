namespace E_Satis_Auction.Dtos.Commerce;

public sealed record OrderShippingInfoDto(
    string CarrierName,
    string TrackingNumber,
    string? TrackingUrl,
    string? Notes,
    DateTimeOffset ShippedAt);
