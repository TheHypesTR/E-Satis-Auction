using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.UserSaleRequest.CreateUserSaleRequest;
using E_Satis_Auction.Features.UserSaleRequest.GetMyUserSaleRequestById;
using E_Satis_Auction.Features.UserSaleRequest.GetMyUserSaleRequests;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

public sealed class UserSaleRequestController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(UserSaleRequestDto), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserSaleRequestRequest request)
    {
        UserSaleRequestDto result = await Mediator.Send(new CreateUserSaleRequestCommand(request));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [ProducesResponseType(typeof(PaginatedList<UserSaleRequestDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetMine([FromQuery] GetMyUserSaleRequestsQuery query)
    {
        PaginatedList<UserSaleRequestDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [ProducesResponseType(typeof(UserSaleRequestDto), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        UserSaleRequestDto result = await Mediator.Send(new GetMyUserSaleRequestByIdQuery(id));
        return Ok(result);
    }
}
