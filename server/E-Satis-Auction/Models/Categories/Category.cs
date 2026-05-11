using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;

namespace E_Satis_Auction.Models.Categories;

public sealed class Category : BaseEntity
{
    public string Name { get; private set; }
    public string NormalizedName { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private List<CategoryAttribute> _attributes = [];
    public IReadOnlyCollection<CategoryAttribute> Attributes => _attributes.AsReadOnly();

    private Category()
    {
        Name = string.Empty;
        NormalizedName = string.Empty;
        IsActive = true;
    }

    public static Category Create(string name, string? description, bool isActive = true)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            name,
            ErrorMessages.Category.NameRequired,
            ErrorMessages.Exception.CategoryTitle);

        string trimmedName = name.Trim();
        BusinessException.ThrowIfTrue(
            trimmedName.Length > 128,
            ErrorMessages.Category.NameMaxLength,
            ErrorMessages.Exception.CategoryTitle);
        
        string? trimmedDescription = description?.Trim();
        BusinessException.ThrowIfTrue(
            trimmedDescription is { Length: > 500 },
            ErrorMessages.Category.DescriptionMaxLength,
            ErrorMessages.Exception.CategoryTitle);

        return new Category
        {
            Name = trimmedName,
            NormalizedName = trimmedName.ToSemanticCode(),
            Description = string.IsNullOrWhiteSpace(trimmedDescription) ? null : trimmedDescription,
            IsActive = isActive
        };
    }

    public void UpdateDetails(string name, string? description)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            name,
            ErrorMessages.Category.NameRequired,
            ErrorMessages.Exception.CategoryTitle);

        string trimmedName = name.Trim();
        BusinessException.ThrowIfTrue(
            trimmedName.Length > 128,
            ErrorMessages.Category.NameMaxLength,
            ErrorMessages.Exception.CategoryTitle);

        string? trimmedDescription = description?.Trim();
        BusinessException.ThrowIfTrue(
            trimmedDescription is { Length: > 500 },
            ErrorMessages.Category.DescriptionMaxLength,
            ErrorMessages.Exception.CategoryTitle);

        Name = trimmedName;
        NormalizedName = trimmedName.ToSemanticCode();
        Description = string.IsNullOrWhiteSpace(trimmedDescription) ? null : trimmedDescription;
    }

    public void EnsureCanMutateSchema(bool hasDependentData)
    {
        BusinessException.ThrowIfTrue(
            IsActive,
            ErrorMessages.Category.AttributeMutationRequiresInactiveCategory,
            ErrorMessages.Exception.CategoryTitle);

        BusinessException.ThrowIfTrue(
            hasDependentData,
            ErrorMessages.Category.AttributeMutationBlockedByDependentData,
            ErrorMessages.Exception.CategoryTitle);
    }

    public void Activate()
    {
        BusinessException.ThrowIfTrue(
            IsActive,
            ErrorMessages.Category.AlreadyActive,
            ErrorMessages.Exception.CategoryTitle);

        IsActive = true;
    }

    public void Deactivate()
    {
        BusinessException.ThrowIfTrue(
            !IsActive,
            ErrorMessages.Category.AlreadyInactive,
            ErrorMessages.Exception.CategoryTitle);

        IsActive = false;
    }
    
    public void AddAttribute(CategoryAttribute attribute)
    {
        BusinessException.ThrowIfNull(
            attribute,
            ErrorMessages.Category.AttributeRequired,
            ErrorMessages.Exception.CategoryTitle);
        
        BusinessException.ThrowIfTrue(
            attribute.CategoryId != Id,
            ErrorMessages.Category.AttributeCategoryRequired,
            ErrorMessages.Exception.CategoryTitle);
        
        bool hasDuplicateCode = Attributes.Any(a => a.Code.Equals(attribute.Code, StringComparison.OrdinalIgnoreCase));
        BusinessException.ThrowIfTrue(
            hasDuplicateCode,
            ErrorMessages.Category.DuplicateAttributeCode,
            ErrorMessages.Exception.CategoryTitle);
        
        _attributes.Add(attribute);
    }
}