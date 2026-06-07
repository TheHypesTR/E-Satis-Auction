using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Attributes;
using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.PurchaseOrder.ApprovePurchaseOrder;
using E_Satis_Auction.Features.PurchaseOrder.GetAdminPurchaseOrderById;
using E_Satis_Auction.Features.PurchaseOrder.GetAdminPurchaseOrders;
using E_Satis_Auction.Features.PurchaseOrder.RejectPurchaseOrder;
using E_Satis_Auction.Features.PurchaseOrder.ShipPurchaseOrder;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

[RequireRoles(AppRoles.GeneralAdmin)]
public sealed class AdminPurchaseOrderController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(PaginatedList<AdminOrderSummaryDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetPurchaseOrders([FromQuery] GetAdminPurchaseOrdersQuery query)
    {
        PaginatedList<AdminOrderSummaryDto> result = await Mediator.Send(query);

        return Ok(result);
    }

    [ProducesResponseType(typeof(AdminOrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPurchaseOrderById(Guid id)
    {
        AdminOrderDetailDto result = await Mediator.Send(new GetAdminPurchaseOrderByIdQuery(id));

        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> ApprovePurchaseOrder(Guid id, [FromBody] ApprovePurchaseOrderRequest request)
    {
        await Mediator.Send(new ApprovePurchaseOrderCommand(id, request));

        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> RejectPurchaseOrder(Guid id, [FromBody] RejectOrderRequest request)
    {
        await Mediator.Send(new RejectPurchaseOrderCommand(id, request));

        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}/ship")]
    public async Task<IActionResult> ShipPurchaseOrder(Guid id, [FromBody] ShipOrderRequest request)
    {
        await Mediator.Send(new ShipPurchaseOrderCommand(id, request));

        return NoContent();
    }
}
