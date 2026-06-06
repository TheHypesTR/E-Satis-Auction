using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Attributes;
using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.Campaign.ActivateCampaign;
using E_Satis_Auction.Features.Campaign.CreateCampaign;
using E_Satis_Auction.Features.Campaign.DeactivateCampaign;
using E_Satis_Auction.Features.Campaign.GetCampaignById;
using E_Satis_Auction.Features.Campaign.GetCampaigns;
using E_Satis_Auction.Features.Campaign.UpdateCampaign;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

[RequireRoles(AppRoles.GeneralAdmin)]
public sealed class AdminCampaignController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(PaginatedList<CampaignDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetCampaigns([FromQuery] GetCampaignsQuery query)
    {
        PaginatedList<CampaignDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCampaignById(Guid id)
    {
        CampaignDto result = await Mediator.Send(new GetCampaignByIdQuery(id));
        return Ok(result);
    }

    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequest request)
    {
        CampaignDto result = await Mediator.Send(new CreateCampaignCommand(request));
        return CreatedAtAction(nameof(GetCampaignById), new { id = result.Id }, result);
    }

    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCampaign(Guid id, [FromBody] UpdateCampaignRequest request)
    {
        CampaignDto result = await Mediator.Send(new UpdateCampaignCommand(id, request));
        return Ok(result);
    }

    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> ActivateCampaign(Guid id)
    {
        CampaignDto result = await Mediator.Send(new ActivateCampaignCommand(id));
        return Ok(result);
    }

    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateCampaign(Guid id)
    {
        CampaignDto result = await Mediator.Send(new DeactivateCampaignCommand(id));
        return Ok(result);
    }
}
