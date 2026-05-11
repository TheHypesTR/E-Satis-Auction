using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.Item.AddStandardizedItem;

public sealed record AddStandardizedItemCommand(
    Guid ProductId,
    Guid FacilityId,
    int Quantity,
    UnitOfMeasure UnitOfMeasure,
    ItemStatus Status,
    Dictionary<string, string>? DynamicAttributes) : IAuditableCommand<Guid>;