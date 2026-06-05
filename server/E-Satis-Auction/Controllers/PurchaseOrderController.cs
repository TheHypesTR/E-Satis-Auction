using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.PurchaseOrder.BuyNow;
using E_Satis_Auction.Features.PurchaseOrder.GetMyOrderById;
using E_Satis_Auction.Features.PurchaseOrder.GetMyOrders;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

public sealed class PurchaseOrderController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("buy-now")]
    public async Task<IActionResult> BuyNow([FromBody] BuyNowRequest request)
    {
        OrderDetailDto result = await Mediator.Send(new BuyNowCommand(request.ProductListingId, request.Quantity, request.CampaignId));

        return CreatedAtAction(nameof(GetMyOrderById), new { id = result.Id }, result);
    }

    [ProducesResponseType(typeof(PaginatedList<OrderSummaryDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetMyOrders([FromQuery] GetMyOrdersQuery query)
    {
        PaginatedList<OrderSummaryDto> result = await Mediator.Send(query);

        return Ok(result);
    }

    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMyOrderById(Guid id)
    {
        OrderDetailDto result = await Mediator.Send(new GetMyOrderByIdQuery(id));

        return Ok(result);
    }
}
