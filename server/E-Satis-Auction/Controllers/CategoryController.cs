using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Attributes;
using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Category;
using E_Satis_Auction.Dtos.Category.Requests;
using E_Satis_Auction.Features.Category.ActivateCategory;
using E_Satis_Auction.Features.Category.AddCategory;
using E_Satis_Auction.Features.Category.AddCategoryAttribute;
using E_Satis_Auction.Features.Category.AddCategoryAttributeOption;
using E_Satis_Auction.Features.Category.DeactivateCategory;
using E_Satis_Auction.Features.Category.DeleteCategoryAttribute;
using E_Satis_Auction.Features.Category.DeleteCategoryAttributeOption;
using E_Satis_Auction.Features.Category.GetAllCategories;
using E_Satis_Auction.Features.Category.GetCategoryById;
using E_Satis_Auction.Features.Category.UpdateCategory;
using E_Satis_Auction.Features.Category.UpdateCategoryAttribute;
using E_Satis_Auction.Features.Category.UpdateCategoryAttributeOption;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace E_Satis_Auction.Controllers;

public class CategoryController : AuthorizedBaseController
{
    /// <summary>
    /// Retrieves a paginated list of categories with optional search and status filtering.
    /// </summary>
    /// <param name="query">Pagination, search term, and status filters</param>
    /// <returns>A paginated list of categories</returns>
    /// <response code="200">Returns the requested page of categories</response>
    [ProducesResponseType(typeof(PaginatedList<CategoryDto>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllCategories([FromQuery] GetAllCategoriesQuery query)
    {
        PaginatedList<CategoryDto> result = await Mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the details of a specific category by its unique ID.
    /// </summary>
    /// <param name="id">The unique identifier of the category</param>
    /// <returns>Category details with attributes and options</returns>
    /// <response code="200">Category found and returned</response>
    /// <response code="404">Category with the specified ID does not exist</response>
    [ProducesResponseType(typeof(CategoryDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        CategoryDetailDto result = await Mediator.Send(new GetCategoryByIdQuery(id));

        return Ok(result);
    }
    
    /// <summary>
    /// Add a new Category along with its dynamic attributes and options.
    /// Restricted to users with the GeneralAdmin role.
    /// </summary>
    /// <param name="command">Category details, active status, and optional dynamic attributes structure</param>
    /// <returns>The unique identifier (Guid) of the newly created category</returns>
    /// <response code="201">Category successfully created.</response>
    /// <response code="400">Validation error or category name already exists.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] AddCategoryCommand command)
    {
        Guid categoryId = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetCategoryById), new { id = categoryId }, new { id = categoryId });
    }

    /// <summary>
    /// Updates an existing category's basic fields (Name, Description, etc.).
    /// </summary>
    /// <param name="id">The unique identifier of the category to update</param>
    /// <param name="request">Category update payload containing new values</param>
    /// <response code="204">Category successfully updated.</response>
    /// <response code="400">Validation error or duplicate category name.</response>
    /// <response code="404">Category not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        await Mediator.Send(new UpdateCategoryCommand(id, request));

        return NoContent();
    }
        
    /// <summary>
    /// Activates an existing category.
    /// </summary>
    /// <param name="id">The unique identifier of the category</param>
    /// <response code="204">Category successfully activated.</response>
    /// <response code="404">Category not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> ActivateCategory(Guid id)
    {
        await Mediator.Send(new ActivateCategoryCommand(id));

        return NoContent();
    }
    
    /// <summary>
    /// Deactivates an existing category.
    /// </summary>
    /// <param name="id">The unique identifier of the category</param>
    /// <response code="204">Category successfully deactivated.</response>
    /// <response code="404">Category not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateCategory(Guid id)
    {
        await Mediator.Send(new DeactivateCategoryCommand(id));

        return NoContent();
    }

    /// <summary>
    /// Adds a new dynamic attribute to an existing category. 
    /// Note: The category must be inactive to perform this operation.
    /// </summary>
    /// <param name="id">The unique identifier of the parent category</param>
    /// <param name="request">The attribute details to add</param>
    /// <returns>The unique identifier (Guid) of the newly created attribute</returns>
    /// <response code="201">Attribute successfully added to the category.</response>
    /// <response code="400">Validation error, duplicate code, or category is active.</response>
    /// <response code="404">Parent category not found.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPost("{id:guid}/attributes")]
    public async Task<IActionResult> AddCategoryAttribute(Guid id, [FromBody] AddCategoryAttributeRequest request)
    {
        Guid attributeId = await Mediator.Send(new AddCategoryAttributeCommand(id, request));

        return CreatedAtAction(nameof(GetCategoryById), new { id }, new { id = attributeId });
    }

    /// <summary>
    /// Updates an existing attribute of a category.
    /// Note: The category must be inactive to perform this operation.
    /// </summary>
    /// <param name="id">The unique identifier of the parent category</param>
    /// <param name="attributeId">The unique identifier of the attribute to update</param>
    /// <param name="request">The updated attribute details</param>
    /// <response code="204">Attribute successfully updated.</response>
    /// <response code="400">Validation error or category is active.</response>
    /// <response code="404">Category or Attribute not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPut("{id:guid}/attributes/{attributeId:guid}")]
    public async Task<IActionResult> UpdateCategoryAttribute(Guid id, Guid attributeId, [FromBody] UpdateCategoryAttributeRequest request)
    {
        await Mediator.Send(new UpdateCategoryAttributeCommand(id, attributeId, request));

        return NoContent();
    }

    /// <summary>
    /// Soft deletes a category attribute and all its associated options.
    /// Note: The category must be inactive to perform this operation.
    /// </summary>
    /// <param name="id">The unique identifier of the parent category</param>
    /// <param name="attributeId">The unique identifier of the attribute to delete</param>
    /// <response code="204">Attribute successfully deleted.</response>
    /// <response code="404">Category or Attribute not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpDelete("{id:guid}/attributes/{attributeId:guid}")]
    public async Task<IActionResult> DeleteCategoryAttribute(Guid id, Guid attributeId)
    {
        await Mediator.Send(new DeleteCategoryAttributeCommand(id, attributeId));

        return NoContent();
    }

    /// <summary>
    /// Adds a new dropdown option to an existing category attribute.
    /// Only applicable if the attribute's DataType is SelectList.
    /// </summary>
    /// <param name="id">The unique identifier of the parent category</param>
    /// <param name="attributeId">The unique identifier of the parent attribute</param>
    /// <param name="request">The option value to add</param>
    /// <returns>The unique identifier (Guid) of the newly created option</returns>
    /// <response code="201">Option successfully added.</response>
    /// <response code="400">Validation error, duplicate option, or attribute is not a SelectList.</response>
    /// <response code="404">Category or Attribute not found.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPost("{id:guid}/attributes/{attributeId:guid}/options")]
    public async Task<IActionResult> AddCategoryAttributeOption(Guid id, Guid attributeId, [FromBody] AddCategoryAttributeOptionRequest request)
    {
        Guid optionId = await Mediator.Send(new AddCategoryAttributeOptionCommand(id, attributeId, request));

        return CreatedAtAction(nameof(GetCategoryById), new { id }, new { id = optionId });
    }

    /// <summary>
    /// Updates an existing option of a category attribute.
    /// </summary>
    /// <param name="id">The unique identifier of the parent category</param>
    /// <param name="attributeId">The unique identifier of the parent attribute</param>
    /// <param name="optionId">The unique identifier of the option to update</param>
    /// <param name="request">The new option value</param>
    /// <response code="204">Option successfully updated.</response>
    /// <response code="400">Validation error or duplicate option value.</response>
    /// <response code="404">Category, Attribute, or Option not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPut("{id:guid}/attributes/{attributeId:guid}/options/{optionId:guid}")]
    public async Task<IActionResult> UpdateCategoryAttributeOption(
        Guid id, Guid attributeId, Guid optionId, [FromBody] UpdateCategoryAttributeOptionRequest request)
    {
        await Mediator.Send(new UpdateCategoryAttributeOptionCommand(id, attributeId, optionId, request));

        return NoContent();
    }

    /// <summary>
    /// Soft deletes a specific option from a category attribute.
    /// </summary>
    /// <param name="id">The unique identifier of the parent category</param>
    /// <param name="attributeId">The unique identifier of the parent attribute</param>
    /// <param name="optionId">The unique identifier of the option to delete</param>
    /// <response code="204">Option successfully deleted.</response>
    /// <response code="404">Category, Attribute, or Option not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpDelete("{id:guid}/attributes/{attributeId:guid}/options/{optionId:guid}")]
    public async Task<IActionResult> DeleteCategoryAttributeOption(Guid id, Guid attributeId, Guid optionId)
    {
        await Mediator.Send(new DeleteCategoryAttributeOptionCommand(id, attributeId, optionId));

        return NoContent();
    }
}