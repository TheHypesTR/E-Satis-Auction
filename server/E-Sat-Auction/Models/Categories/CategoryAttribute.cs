using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Entities;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Enums;
using e_Sat_Auction.Common.Extensions;

namespace e_Sat_Auction.Models.Categories;

public sealed class CategoryAttribute : BaseEntity
{
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public AttributeDataType DataType { get; private set; }
    public AttributeTarget Target { get; private set; }
    public bool IsRequired { get; private set; }

    private List<CategoryAttributeOption> _options = [];
    public IReadOnlyCollection<CategoryAttributeOption> Options => _options.AsReadOnly();
    public Category Category { get; private set; } = null!;
    
    private CategoryAttribute()
    {
        Name = string.Empty;
        Code = string.Empty;
    }

    public static CategoryAttribute Create(
        Guid categoryId,
        string name,
        string code,
        AttributeDataType dataType,
        AttributeTarget target,
        bool isRequired)
    {
        BusinessException.ThrowIfTrue(
            categoryId == Guid.Empty,
            ErrorMessages.Category.AttributeCategoryRequired,
            ErrorMessages.Exception.CategoryTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            name,
            ErrorMessages.Category.AttributeNameRequired,
            ErrorMessages.Exception.CategoryTitle);
        BusinessException.ThrowIfNullOrWhiteSpace(
            code,
            ErrorMessages.Category.AttributeCodeRequired,
            ErrorMessages.Exception.CategoryTitle);

        string semanticCode = code.ToSemanticCode();
        BusinessException.ThrowIfNullOrWhiteSpace(
            semanticCode,
            ErrorMessages.Category.AttributeCodeInvalidCharacters,
            ErrorMessages.Exception.CategoryTitle);

        return new CategoryAttribute
        {
            CategoryId = categoryId,
            Name = name.Trim(),
            Code = semanticCode,
            DataType = dataType,
            Target = target,
            IsRequired = isRequired
        };
    }

    public void AddOption(string value)
    {
        BusinessException.ThrowIfFalse(
            DataType is AttributeDataType.SelectList,
            ErrorMessages.Category.OptionOnlyForSelectList,
            ErrorMessages.Exception.CategoryTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            value,
            ErrorMessages.Category.OptionValueRequired,
            ErrorMessages.Exception.CategoryTitle);

        string normalizedValue = value.Trim();
        bool hasDuplicateOption = Options.Any(o => o.Value.Equals(normalizedValue, StringComparison.OrdinalIgnoreCase));
        BusinessException.ThrowIfTrue(
            hasDuplicateOption,
            ErrorMessages.Category.DuplicateOptionValue,
            ErrorMessages.Exception.CategoryTitle);

        CategoryAttributeOption option = CategoryAttributeOption.Create(Id, normalizedValue);
        _options.Add(option);
    }

    public void UpdateDetails(string name, string code, AttributeDataType dataType, AttributeTarget target, bool isRequired)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            name,
            ErrorMessages.Category.AttributeNameRequired,
            ErrorMessages.Exception.CategoryTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            code,
            ErrorMessages.Category.AttributeCodeRequired,
            ErrorMessages.Exception.CategoryTitle);

        string semanticCode = code.ToSemanticCode();
        BusinessException.ThrowIfNullOrWhiteSpace(
            semanticCode,
            ErrorMessages.Category.AttributeCodeInvalidCharacters,
            ErrorMessages.Exception.CategoryTitle);

        Name = name.Trim();
        Code = semanticCode;
        DataType = dataType;
        Target = target;
        IsRequired = isRequired;

        if (DataType is not AttributeDataType.SelectList)
        {
            foreach (CategoryAttributeOption option in _options)
            {
                option.Delete();
            }
        }
    }

    public void UpdateOption(Guid optionId, string value)
    {
        BusinessException.ThrowIfFalse(
            DataType is AttributeDataType.SelectList,
            ErrorMessages.Category.OptionOnlyForSelectList,
            ErrorMessages.Exception.CategoryTitle);

        CategoryAttributeOption option = GetOption(optionId);
        
        string normalizedValue = value.Trim();
        bool hasDuplicateOption = _options.Any(o => o.Id != optionId && o.Value.Equals(normalizedValue, StringComparison.OrdinalIgnoreCase));
        BusinessException.ThrowIfTrue(
            hasDuplicateOption,
            ErrorMessages.Category.DuplicateOptionValue,
            ErrorMessages.Exception.CategoryTitle);

        option.UpdateValue(normalizedValue);
    }

    public void DeleteOption(Guid optionId)
    {
        CategoryAttributeOption option = GetOption(optionId);
        BusinessException.ThrowIfTrue(
            DataType is AttributeDataType.SelectList && _options.Count <= 1,
            ErrorMessages.Category.CannotDeleteLastOption,
            ErrorMessages.Exception.CategoryTitle);
        
        option.Delete();
    }

    public void DeleteWithOptions()
    {
        foreach (CategoryAttributeOption option in _options)
        {
            option.Delete();
        }

        Delete();
    }
    
    private CategoryAttributeOption GetOption(Guid optionId)
    {
        CategoryAttributeOption? option = _options.FirstOrDefault(o => o.Id == optionId);
        NotFoundException.ThrowIfNull(option, ErrorMessages.Category.OptionEntityName, optionId);
        return option!;
    }
}