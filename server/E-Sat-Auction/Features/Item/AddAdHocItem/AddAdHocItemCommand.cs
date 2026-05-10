using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.Item.AddAdHocItem;

public sealed record AddAdHocItemCommand(
    Guid CategoryId,
    Guid FacilityId,
    string Name,
    int Quantity,
    UnitOfMeasure UnitOfMeasure,
    ItemStatus Status,
    Dictionary<string, string>? DynamicAttributes) : IAuditableCommand<Guid>;