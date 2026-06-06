using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Attributes;
using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.PartSaleOperation.CreatePartSaleOperation;
using E_Satis_Auction.Features.PartSaleOperation.GetPartSaleOperationById;
using E_Satis_Auction.Features.PartSaleOperation.GetPartSaleOperations;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

[RequireRoles(AppRoles.GeneralAdmin)]
public sealed class AdminItemPartSaleOperationsController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(PartSaleOperationDto), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartSaleOperationRequest request)
    {
        PartSaleOperationDto result = await Mediator.Send(new CreatePartSaleOperationCommand(request));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [ProducesResponseType(typeof(PaginatedList<PartSaleOperationDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetPartSaleOperationsQuery query)
    {
        PaginatedList<PartSaleOperationDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [ProducesResponseType(typeof(PartSaleOperationDto), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        PartSaleOperationDto result = await Mediator.Send(new GetPartSaleOperationByIdQuery(id));
        return Ok(result);
    }
}
