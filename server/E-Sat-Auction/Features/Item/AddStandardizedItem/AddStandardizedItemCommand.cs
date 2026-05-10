using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.Item.AddStandardizedItem;

public sealed record AddStandardizedItemCommand(
    Guid ProductId,
    Guid FacilityId,
    int Quantity,
    UnitOfMeasure UnitOfMeasure,
    ItemStatus Status,
    Dictionary<string, string>? DynamicAttributes) : IAuditableCommand<Guid>;