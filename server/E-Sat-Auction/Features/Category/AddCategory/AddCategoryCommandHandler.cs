using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.Categories;
using e_Sat_Auction.Common.Extensions;

namespace e_Sat_Auction.Features.Category.AddCategory;

public class AddCategoryCommandHandler : ICommandHandler<AddCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Guid> Handle(AddCategoryCommand command, CancellationToken cancellationToken)
    {
        string normalizedName = command.Name.ToSemanticCode();
        bool categoryExists = await _categoryRepository.AnyAsync(c => c.NormalizedName == normalizedName, cancellationToken);
        
        BusinessException.ThrowIfTrue(
            categoryExists, 
            ErrorMessages.Category.NameAlreadyExists, 
            ErrorMessages.Exception.CategoryTitle);
        
        Models.Categories.Category category = Models.Categories.Category.Create(
            command.Name, 
            command.Description, 
            command.IsActive);
        
        if (command.Attributes is not null && command.Attributes.Count is not 0)
        {
            AddAttributesToCategory(category, command.Attributes);
        }
        
        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return category.Id;
    }
    
    private static void AddAttributesToCategory(Models.Categories.Category category, List<CategoryAttributeCommandModel> attributes)
    {
        foreach (CategoryAttributeCommandModel attrModel in attributes)
        {
            CategoryAttribute attribute = CategoryAttribute.Create(
                category.Id, 
                attrModel.Name, 
                attrModel.Code, 
                attrModel.DataType, 
                attrModel.Target,
                attrModel.IsRequired);

            if (attrModel.Options is not null && attrModel.Options.Count is not 0)
            {
                foreach (string optionValue in attrModel.Options)
                {
                    attribute.AddOption(optionValue);
                }
            }

            category.AddAttribute(attribute);
        }
    }
}