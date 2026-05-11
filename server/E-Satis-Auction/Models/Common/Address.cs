using E_Satis_Auction.Common.Entities;

namespace E_Satis_Auction.Models.Common;

public sealed class Address : BaseEntity
{
    public string Title { get; private set; }
    public string City { get; private set; }
    public string District { get; private set; }
    public string OpenAddress { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public bool IsTemporary { get; private set; }

    private Address()
    {
        Title = string.Empty;
        City = string.Empty;
        District = string.Empty;
        OpenAddress = string.Empty;
    }

    public static Address Add(
        string title,
        string city,
        string district,
        string openAddress,
        double latitude,
        double longitude,
        bool isTemporary = false)
    {
        return new Address
        {
            Title = title,
            City = city,
            District = district,
            OpenAddress = openAddress,
            Latitude = latitude,
            Longitude = longitude,
            IsTemporary = isTemporary
        };
    }
}