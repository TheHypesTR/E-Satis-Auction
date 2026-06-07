using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.ReturnRequest.CreateReturnRequest;
using E_Satis_Auction.Features.ReturnRequest.GetMyReturnRequestById;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

public sealed class ReturnRequestController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(ReturnRequestDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMyReturnRequestById(Guid id)
    {
        ReturnRequestDetailDto result = await Mediator.Send(new GetMyReturnRequestByIdQuery(id));

        return Ok(result);
    }

    [ProducesResponseType(typeof(ReturnRequestDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("purchase-orders/{purchaseOrderId:guid}")]
    public async Task<IActionResult> CreateReturnRequest(Guid purchaseOrderId, [FromBody] CreateReturnRequestRequest request)
    {
        ReturnRequestDetailDto result = await Mediator.Send(new CreateReturnRequestCommand(purchaseOrderId, request));

        return CreatedAtAction(nameof(GetMyReturnRequestById), new { id = result.Id }, result);
    }
}
