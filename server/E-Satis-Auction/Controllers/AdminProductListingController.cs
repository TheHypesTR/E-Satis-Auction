using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Attributes;
using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.ProductListing.ActivateProductListing;
using E_Satis_Auction.Features.ProductListing.CreateProductListing;
using E_Satis_Auction.Features.ProductListing.DeactivateProductListing;
using E_Satis_Auction.Features.ProductListing.DeleteProductListing;
using E_Satis_Auction.Features.ProductListing.GetAdminProductListingById;
using E_Satis_Auction.Features.ProductListing.GetAdminProductListings;
using E_Satis_Auction.Features.ProductListing.UpdateProductListing;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

[RequireRoles(AppRoles.GeneralAdmin)]
public sealed class AdminProductListingController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(PaginatedList<AdminProductListingSummaryDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetProductListings([FromQuery] GetAdminProductListingsQuery query)
    {
        PaginatedList<AdminProductListingSummaryDto> result = await Mediator.Send(query);

        return Ok(result);
    }

    [ProducesResponseType(typeof(AdminProductListingDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductListingById(Guid id)
    {
        AdminProductListingDetailDto result = await Mediator.Send(new GetAdminProductListingByIdQuery(id));

        return Ok(result);
    }

    [ProducesResponseType(typeof(AdminProductListingDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<IActionResult> CreateProductListing([FromBody] CreateProductListingRequest request)
    {
        AdminProductListingDetailDto result = await Mediator.Send(new CreateProductListingCommand(request));

        return CreatedAtAction(nameof(GetProductListingById), new { id = result.Id }, result);
    }

    [ProducesResponseType(typeof(AdminProductListingDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProductListing(Guid id, [FromBody] UpdateProductListingRequest request)
    {
        AdminProductListingDetailDto result = await Mediator.Send(new UpdateProductListingCommand(id, request));

        return Ok(result);
    }

    [ProducesResponseType(typeof(AdminProductListingDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> ActivateProductListing(Guid id)
    {
        AdminProductListingDetailDto result = await Mediator.Send(new ActivateProductListingCommand(id));

        return Ok(result);
    }

    [ProducesResponseType(typeof(AdminProductListingDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateProductListing(Guid id)
    {
        AdminProductListingDetailDto result = await Mediator.Send(new DeactivateProductListingCommand(id));

        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProductListing(Guid id)
    {
        await Mediator.Send(new DeleteProductListingCommand(id));

        return NoContent();
    }
}
