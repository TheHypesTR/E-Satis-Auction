using e_Sat_Auction.Common;
using e_Sat_Auction.Common.Attributes;
using e_Sat_Auction.Common.Controllers;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.Product;
using e_Sat_Auction.Features.Product.ActivateProduct;
using e_Sat_Auction.Features.Product.AddProduct;
using e_Sat_Auction.Features.Product.DeactivateProduct;
using e_Sat_Auction.Features.Product.GetAllProducts;
using e_Sat_Auction.Features.Product.GetProductById;
using Microsoft.AspNetCore.Mvc;

namespace e_Sat_Auction.Controllers;

public class ProductController : AuthorizedBaseController
{
    /// <summary>
    /// Gets a paginated list of the Master Product Catalog.
    /// Returns global product definitions, independent of physical facility stock.
    /// </summary>
    /// <param name="query">Pagination, search term, and status filters.</param>
    /// <returns>A paginated list of products.</returns>
    /// <response code="200">Returns the requested page of products.</response>
    [ProducesResponseType(typeof(PaginatedList<ProductSummaryDto>), StatusCodes.Status200OK)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.NGOAdmin, AppRoles.WarehouseManager)]
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductsQuery query)
    {
        PaginatedList<ProductSummaryDto> result = await Mediator.Send(query);

        return Ok(result);
    }
    
    /// <summary>
    /// Retrieves details of a specific Master Product.
    /// Includes an aggregated real-time stock summary across all active facilities.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>Product details and facility-based stock summary.</returns>
    /// <response code="200">Product found and returned.</response>
    /// <response code="404">Product with the specified ID does not exist.</response>
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.NGOAdmin, AppRoles.WarehouseManager)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        ProductDetailDto result = await Mediator.Send(new GetProductByIdQuery(id));

        return Ok(result);
    }
    
    /// <summary>
    /// Add a new Product (Master Catalog Item) with standardized base attributes.
    /// Restricted to users with GeneralAdmin roles.
    /// </summary>
    /// <param name="command">Product details including SKU, Category, Unit of Measure, and required Base Attributes</param>
    /// <returns>The unique identifier (Guid) of the newly created product</returns>
    /// <response code="201">Product successfully created.</response>
    /// <response code="400">Validation error, missing required attributes, invalid attribute keys, or duplicate SKU.</response>
    /// <response code="404">The specified Category was not found.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] AddProductCommand command)
    {
        Guid productId = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetProductById), new { id = productId }, new { id = productId });
    }
    
    /// <summary>
    /// Activates an existing product in the catalog.
    /// </summary>
    /// <param name="id">The unique identifier of the product</param>
    /// <response code="204">Product successfully activated.</response>
    /// <response code="404">Product not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> ActivateProduct(Guid id)
    {
        await Mediator.Send(new ActivateProductCommand(id));

        return NoContent();
    }

    /// <summary>
    /// Deactivates an existing product in the catalog.
    /// </summary>
    /// <param name="id">The unique identifier of the product</param>
    /// <response code="204">Product successfully deactivated.</response>
    /// <response code="404">Product not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateProduct(Guid id)
    {
        await Mediator.Send(new DeactivateProductCommand(id));

        return NoContent();
    }
}