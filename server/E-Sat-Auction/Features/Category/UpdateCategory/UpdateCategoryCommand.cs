using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Category.Requests;

namespace e_Sat_Auction.Features.Category.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid Id, string Name, string? Description) : IAuditableCommand
{
    public UpdateCategoryCommand(Guid id, UpdateCategoryRequest request) : this(id, request.Name, request.Description)
    {
    }
}