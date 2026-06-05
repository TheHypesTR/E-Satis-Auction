using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;

namespace E_Satis_Auction.Models.Commerce;

public sealed class OrderShippingInfo
{
    public string CarrierName { get; private set; }
    public string TrackingNumber { get; private set; }
    public string? TrackingUrl { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset ShippedAt { get; private set; }

    private OrderShippingInfo()
    {
        CarrierName = string.Empty;
        TrackingNumber = string.Empty;
    }

    public static OrderShippingInfo Create(
        string carrierName,
        string trackingNumber,
        DateTimeOffset shippedAt,
        string? trackingUrl = null,
        string? notes = null)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            carrierName,
            ErrorMessages.PurchaseOrder.CarrierNameRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            trackingNumber,
            ErrorMessages.PurchaseOrder.TrackingNumberRequired,
            ErrorMessages.Exception.CommerceTitle);

        return new OrderShippingInfo
        {
            CarrierName = carrierName.Trim(),
            TrackingNumber = trackingNumber.Trim(),
            TrackingUrl = string.IsNullOrWhiteSpace(trackingUrl) ? null : trackingUrl.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            ShippedAt = shippedAt
        };
    }
}
