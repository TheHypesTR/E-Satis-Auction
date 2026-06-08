using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Attributes;
using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Auction.Requests;
using E_Satis_Auction.Features.AdminAuction.ActivateAuction;
using E_Satis_Auction.Features.AdminAuction.CancelAuction;
using E_Satis_Auction.Features.AdminAuction.CreateAuction;
using E_Satis_Auction.Features.AdminAuction.FinalizeAuction;
using E_Satis_Auction.Features.AdminAuction.GetAdminAuctionById;
using E_Satis_Auction.Features.AdminAuction.GetAdminAuctions;
using E_Satis_Auction.Features.AdminAuction.RelistAuction;
using E_Satis_Auction.Features.AdminAuction.ScheduleAuction;
using E_Satis_Auction.Features.AdminAuction.UpdateAuction;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

[RequireRoles(AppRoles.GeneralAdmin)]
public sealed class AdminAuctionController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(AuctionDetailDto), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuctionRequest request)
    {
        AuctionDetailDto result = await Mediator.Send(new CreateAuctionCommand(request));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [ProducesResponseType(typeof(PaginatedList<AuctionSummaryDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAdminAuctionsQuery query)
    {
        PaginatedList<AuctionSummaryDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [ProducesResponseType(typeof(AuctionDetailDto), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        AuctionDetailDto result = await Mediator.Send(new GetAdminAuctionByIdQuery(id));
        return Ok(result);
    }

    [ProducesResponseType(typeof(AuctionDetailDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuctionRequest request)
    {
        AuctionDetailDto result = await Mediator.Send(new UpdateAuctionCommand(id, request));
        return Ok(result);
    }

    [ProducesResponseType(typeof(AuctionDetailDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}/schedule")]
    public async Task<IActionResult> Schedule(Guid id, [FromBody] ScheduleAuctionRequest request)
    {
        AuctionDetailDto result = await Mediator.Send(new ScheduleAuctionCommand(id, request));
        return Ok(result);
    }

    [ProducesResponseType(typeof(AuctionDetailDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        AuctionDetailDto result = await Mediator.Send(new ActivateAuctionCommand(id));
        return Ok(result);
    }

    [ProducesResponseType(typeof(AuctionDetailDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        AuctionDetailDto result = await Mediator.Send(new CancelAuctionCommand(id));
        return Ok(result);
    }

    [ProducesResponseType(typeof(AuctionDetailDto), StatusCodes.Status200OK)]
    [HttpPost("{id:guid}/finalize")]
    public async Task<IActionResult> Finalize(Guid id)
    {
        AuctionDetailDto result = await Mediator.Send(new FinalizeAuctionCommand(id));
        return Ok(result);
    }

    [ProducesResponseType(typeof(AuctionDetailDto), StatusCodes.Status200OK)]
    [HttpPost("{id:guid}/relist")]
    public async Task<IActionResult> Relist(Guid id, [FromBody] RelistAuctionRequest request)
    {
        AuctionDetailDto result = await Mediator.Send(new RelistAuctionCommand(id, request));
        return Ok(result);
    }
}
