using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.Product.AddProduct;

public sealed record AddProductCommand(
    string Sku, string? Barcode, string Name, Guid CategoryId, UnitOfMeasure UnitOfMeasure, Dictionary<string, string>? BaseAttributes)
        : ICommand<Guid>;