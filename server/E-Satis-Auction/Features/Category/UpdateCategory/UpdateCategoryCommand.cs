using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Category.Requests;

namespace E_Satis_Auction.Features.Category.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid Id, string Name, string? Description) : IAuditableCommand
{
    public UpdateCategoryCommand(Guid id, UpdateCategoryRequest request) : this(id, request.Name, request.Description)
    {
    }
}