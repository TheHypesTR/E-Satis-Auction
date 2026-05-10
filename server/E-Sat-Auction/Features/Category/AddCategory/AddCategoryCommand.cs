using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.Category.AddCategory;

public sealed record AddCategoryCommand(
    string Name, string? Description, bool IsActive, List<CategoryAttributeCommandModel>? Attributes) : IAuditableCommand<Guid>;
    
public sealed record CategoryAttributeCommandModel(
    string Name, string Code, AttributeDataType DataType, AttributeTarget Target, bool IsRequired, List<string>? Options);