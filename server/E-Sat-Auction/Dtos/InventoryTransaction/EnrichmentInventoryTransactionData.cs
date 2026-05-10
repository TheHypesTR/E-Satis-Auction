namespace e_Sat_Auction.Dtos.InventoryTransaction;

using Models.Items;

public sealed record EnrichmentData(
    Dictionary<Guid, string> FacilityNames,
    Dictionary<Guid, Item> ItemLookup,
    Dictionary<Guid, string> ProductNames,
    Dictionary<Guid, string> DispatchTrackingNumbers,
    Dictionary<string, string> UserNames);