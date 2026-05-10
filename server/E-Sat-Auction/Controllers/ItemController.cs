using e_Sat_Auction.Common;
using e_Sat_Auction.Common.Attributes;
using e_Sat_Auction.Common.Controllers;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.Item;
using e_Sat_Auction.Features.Item.AddAdHocItem;
using e_Sat_Auction.Features.Item.AddStandardizedItem;
using e_Sat_Auction.Features.Item.GetAllItems;
using e_Sat_Auction.Features.Item.GetItemById;
using Microsoft.AspNetCore.Mvc;

namespace e_Sat_Auction.Controllers;

public class ItemController : AuthorizedBaseController
{
    /// <summary>
    /// Gets a paginated list of inventory items. 
    /// Results are strictly scoped to facilities the user is authorized to access.
    /// </summary>
    /// <param name="query">Pagination and filtering options.</param>
    /// <returns>A paginated list of inventory items.</returns>
    /// <response code="200">Returns the paginated and authorized list of items.</response>
    [ProducesResponseType(typeof(PaginatedList<ItemSummaryDto>), StatusCodes.Status200OK)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.NGOAdmin, AppRoles.WarehouseManager)]
    [HttpGet]
    public async Task<IActionResult> GetAllItems([FromQuery] GetAllItemsQuery query)
    {
        PaginatedList<ItemSummaryDto> result = await Mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves details of a specific inventory item (Standardized or AdHoc).
    /// Requires authorization over the facility where the item is located.
    /// </summary>
    /// <param name="id">The unique identifier of the item.</param>
    /// <returns>The details of the specified inventory item.</returns>
    /// <response code="200">Returns the item details.</response>
    /// <response code="403">User is not authorized to view the facility's inventory.</response>
    /// <response code="404">Item not found.</response>
    [ProducesResponseType(typeof(ItemDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.NGOAdmin, AppRoles.WarehouseManager)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetItemById(Guid id)
    {
        ItemDetailDto result = await Mediator.Send(new GetItemByIdQuery(id));

        return Ok(result);
    }
    
    /// <summary>
    /// Add a Standardized Item linked to a Master Product Catalog.
    /// Used when adding inventory that exists in the predefined product catalog.
    /// Requires the user to have authorization over the specified Facility.
    /// </summary>
    /// <param name="command">Inventory details including ProductId, FacilityId, Quantity, and item-level batch attributes.</param>
    /// <returns>The unique identifier (Guid) of the newly created inventory item.</returns>
    /// <response code="201">Standardized inventory item successfully created.</response>
    /// <response code="400">Validation error, negative quantity, or attribute schema mismatch.</response>
    /// <response code="403">User is not authorized to add items to the specified facility.</response>
    /// <response code="404">Specified Product, Category, or Facility was not found.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.NGOAdmin, AppRoles.WarehouseManager)]
    [HttpPost("standardized")]
    public async Task<IActionResult> AddStandardizedItem([FromBody] AddStandardizedItemCommand command)
    {
        Guid itemId = await Mediator.Send(command);
        
        return CreatedAtAction(nameof(GetItemById), new { id = itemId }, new { id = itemId });
    }

    /// <summary>
    /// Add an AdHoc Item that is independent of the Master Product Catalog.
    /// Typically used for rapid entry of donated goods or emergency supplies that do not have a predefined product SKU.
    /// Requires the user to have authorization over the specified Facility.
    /// </summary>
    /// <param name="command">Inventory details including CategoryId, FacilityId, custom Name, and dynamic attributes.</param>
    /// <returns>The unique identifier (Guid) of the newly created ad-hoc inventory item.</returns>
    /// <response code="201">Ad-hoc inventory item successfully created.</response>
    /// <response code="400">Validation error, negative quantity, missing name, or attribute schema mismatch.</response>
    /// <response code="403">User is not authorized to add items to the specified facility.</response>
    /// <response code="404">Specified Category or Facility was not found.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.NGOAdmin, AppRoles.WarehouseManager)]
    [HttpPost("adhoc")]
    public async Task<IActionResult> AddAdHocItem([FromBody] AddAdHocItemCommand command)
    {
        Guid itemId = await Mediator.Send(command);
        
        return CreatedAtAction(nameof(GetItemById), new { id = itemId }, new { id = itemId });
    }
}