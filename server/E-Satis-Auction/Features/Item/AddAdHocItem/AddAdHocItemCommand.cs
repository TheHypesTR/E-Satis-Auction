using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.Item.AddAdHocItem;

public sealed record AddAdHocItemCommand(
    Guid CategoryId,
    Guid FacilityId,
    string Name,
    int Quantity,
    UnitOfMeasure UnitOfMeasure,
    ItemStatus Status,
    Dictionary<string, string>? DynamicAttributes) : IAuditableCommand<Guid>;