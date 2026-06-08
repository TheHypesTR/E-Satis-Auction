using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Attributes;
using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.UserSaleRequest.ApproveUserSaleRequest;
using E_Satis_Auction.Features.UserSaleRequest.GetAdminUserSaleRequestById;
using E_Satis_Auction.Features.UserSaleRequest.GetAdminUserSaleRequests;
using E_Satis_Auction.Features.UserSaleRequest.IntakeUserSaleRequest;
using E_Satis_Auction.Features.UserSaleRequest.RejectUserSaleRequest;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

[RequireRoles(AppRoles.GeneralAdmin)]
public sealed class AdminUserSaleRequestController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(PaginatedList<UserSaleRequestDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAdminUserSaleRequestsQuery query)
    {
        PaginatedList<UserSaleRequestDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [ProducesResponseType(typeof(UserSaleRequestDto), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        UserSaleRequestDto result = await Mediator.Send(new GetAdminUserSaleRequestByIdQuery(id));
        return Ok(result);
    }

    [ProducesResponseType(typeof(UserSaleRequestDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveUserSaleRequestRequest request)
    {
        UserSaleRequestDto result = await Mediator.Send(new ApproveUserSaleRequestCommand(id, request));
        return Ok(result);
    }

    [ProducesResponseType(typeof(UserSaleRequestDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectUserSaleRequestRequest request)
    {
        UserSaleRequestDto result = await Mediator.Send(new RejectUserSaleRequestCommand(id, request));
        return Ok(result);
    }

    [ProducesResponseType(typeof(UserSaleRequestDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}/intake")]
    public async Task<IActionResult> Intake(Guid id, [FromBody] IntakeUserSaleRequestRequest request)
    {
        UserSaleRequestDto result = await Mediator.Send(new IntakeUserSaleRequestCommand(id, request));
        return Ok(result);
    }
}
