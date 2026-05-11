using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.Product.AddProduct;

public sealed record AddProductCommand(
    string Sku, string? Barcode, string Name, Guid CategoryId, UnitOfMeasure UnitOfMeasure, Dictionary<string, string>? BaseAttributes)
        : ICommand<Guid>;