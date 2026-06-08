using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Features.ProductListing.GetProductListingById;
using E_Satis_Auction.Features.ProductListing.GetProductListings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

public sealed class ProductListingController : BaseController
{
    [ProducesResponseType(typeof(PaginatedList<ProductListingSummaryDto>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetProductListings([FromQuery] GetProductListingsQuery query)
    {
        PaginatedList<ProductListingSummaryDto> result = await Mediator.Send(query);

        return Ok(result);
    }

    [ProducesResponseType(typeof(ProductListingDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductListingById(Guid id)
    {
        ProductListingDetailDto result = await Mediator.Send(new GetProductListingByIdQuery(id));

        return Ok(result);
    }
}
