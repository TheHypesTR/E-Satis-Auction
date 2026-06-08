using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Auction.Requests;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Features.Auction.GetAuctionBids;
using E_Satis_Auction.Features.Auction.GetAuctionById;
using E_Satis_Auction.Features.Auction.GetAuctionWinner;
using E_Satis_Auction.Features.Auction.GetAuctions;
using E_Satis_Auction.Features.Auction.InitiateAuctionPayment;
using E_Satis_Auction.Features.Auction.PlaceBid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

public sealed class AuctionController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(PaginatedList<AuctionSummaryDto>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAuctionsQuery query)
    {
        PaginatedList<AuctionSummaryDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [ProducesResponseType(typeof(AuctionDetailDto), StatusCodes.Status200OK)]
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        AuctionDetailDto result = await Mediator.Send(new GetAuctionByIdQuery(id));
        return Ok(result);
    }

    [ProducesResponseType(typeof(PaginatedList<AuctionBidDto>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    [HttpGet("{id:guid}/bids")]
    public async Task<IActionResult> GetBids(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        PaginatedList<AuctionBidDto> result = await Mediator.Send(new GetAuctionBidsQuery(id, pageNumber, pageSize));
        return Ok(result);
    }

    [ProducesResponseType(typeof(AuctionBidDto), StatusCodes.Status201Created)]
    [HttpPost("{id:guid}/bids")]
    public async Task<IActionResult> PlaceBid(Guid id, [FromBody] PlaceBidRequest request)
    {
        AuctionBidDto result = await Mediator.Send(new PlaceBidCommand(id, request));
        return CreatedAtAction(nameof(GetBids), new { id }, result);
    }

    [ProducesResponseType(typeof(AuctionWinnerDto), StatusCodes.Status200OK)]
    [AllowAnonymous]
    [HttpGet("{id:guid}/winner")]
    public async Task<IActionResult> GetWinner(Guid id)
    {
        AuctionWinnerDto result = await Mediator.Send(new GetAuctionWinnerQuery(id));
        return Ok(result);
    }

    [ProducesResponseType(typeof(PaymentInitiationDto), StatusCodes.Status201Created)]
    [HttpPost("{id:guid}/payment/initiate")]
    public async Task<IActionResult> InitiatePayment(Guid id, [FromBody] InitiateAuctionPaymentRequest request)
    {
        PaymentInitiationDto result = await Mediator.Send(new InitiateAuctionPaymentCommand(id, request));
        return CreatedAtAction(nameof(GetById), new { id }, result);
    }
}
