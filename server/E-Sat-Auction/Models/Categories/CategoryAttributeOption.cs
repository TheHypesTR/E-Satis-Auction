using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Entities;
using e_Sat_Auction.Common.Exceptions;

namespace e_Sat_Auction.Models.Categories;

public sealed class CategoryAttributeOption : BaseEntity
{
    public Guid CategoryAttributeId { get; private set; }
    public string Value { get; private set; }

    public CategoryAttribute CategoryAttribute { get; private set; } = null!;

    private CategoryAttributeOption()
    {
        Value = string.Empty;
    }

    public static CategoryAttributeOption Create(Guid categoryAttributeId, string value)
    {
        BusinessException.ThrowIfTrue(
            categoryAttributeId == Guid.Empty,
            ErrorMessages.Category.AttributeRequired,
            ErrorMessages.Exception.CategoryTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            value,
            ErrorMessages.Category.OptionValueRequired,
            ErrorMessages.Exception.CategoryTitle);

        return new CategoryAttributeOption
        {
            CategoryAttributeId = categoryAttributeId,
            Value = value.Trim()
        };
    }

    public void UpdateValue(string value)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            value,
            ErrorMessages.Category.OptionValueRequired,
            ErrorMessages.Exception.CategoryTitle);

        Value = value.Trim();
    }
}