namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record ShipOrderRequest(
    string CarrierName,
    string TrackingNumber,
    DateTimeOffset? ShippedAt = null,
    string? TrackingUrl = null,
    string? Notes = null);
