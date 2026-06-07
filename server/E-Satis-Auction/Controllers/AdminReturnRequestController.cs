using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Attributes;
using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.ReturnRequest.ApproveReturnRequest;
using E_Satis_Auction.Features.ReturnRequest.GetAdminReturnRequestById;
using E_Satis_Auction.Features.ReturnRequest.GetAdminReturnRequests;
using E_Satis_Auction.Features.ReturnRequest.ReceiveReturnRequest;
using E_Satis_Auction.Features.ReturnRequest.RejectReturnRequest;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

[RequireRoles(AppRoles.GeneralAdmin)]
public sealed class AdminReturnRequestController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(PaginatedList<AdminReturnRequestSummaryDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetReturnRequests([FromQuery] GetAdminReturnRequestsQuery query)
    {
        PaginatedList<AdminReturnRequestSummaryDto> result = await Mediator.Send(query);

        return Ok(result);
    }

    [ProducesResponseType(typeof(ReturnRequestDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReturnRequestById(Guid id)
    {
        ReturnRequestDetailDto result = await Mediator.Send(new GetAdminReturnRequestByIdQuery(id));

        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> ApproveReturnRequest(Guid id, [FromBody] ApproveReturnRequestRequest request)
    {
        await Mediator.Send(new ApproveReturnRequestCommand(id, request));

        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> RejectReturnRequest(Guid id, [FromBody] RejectReturnRequestRequest request)
    {
        await Mediator.Send(new RejectReturnRequestCommand(id, request));

        return NoContent();
    }

    [ProducesResponseType(typeof(ReturnRequestDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}/receive")]
    public async Task<IActionResult> ReceiveReturnRequest(Guid id, [FromBody] ReceiveReturnRequestRequest request)
    {
        ReturnRequestDetailDto result = await Mediator.Send(new ReceiveReturnRequestCommand(id, request));

        return Ok(result);
    }
}
