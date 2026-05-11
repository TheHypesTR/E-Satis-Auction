using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.Category.AddCategory;

public sealed record AddCategoryCommand(
    string Name, string? Description, bool IsActive, List<CategoryAttributeCommandModel>? Attributes) : IAuditableCommand<Guid>;
    
public sealed record CategoryAttributeCommandModel(
    string Name, string Code, AttributeDataType DataType, AttributeTarget Target, bool IsRequired, List<string>? Options);