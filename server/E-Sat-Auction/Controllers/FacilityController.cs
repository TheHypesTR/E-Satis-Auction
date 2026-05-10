using e_Sat_Auction.Common;
using e_Sat_Auction.Common.Attributes;
using e_Sat_Auction.Common.Controllers;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.Facility;
using e_Sat_Auction.Dtos.Manager.Requests;
using e_Sat_Auction.Features.Facility.AddFacility;
using e_Sat_Auction.Features.Facility.AssignFacilityManager;
using e_Sat_Auction.Features.Facility.DeleteFacility;
using e_Sat_Auction.Features.Facility.GetAllFacilities;
using e_Sat_Auction.Features.Facility.GetFacilityById;
using e_Sat_Auction.Features.Facility.GetMyFacilities;
using e_Sat_Auction.Features.Facility.SetPrimaryFacilityManager;
using e_Sat_Auction.Features.Facility.UnassignFacilityManager;
using Microsoft.AspNetCore.Mvc;

namespace e_Sat_Auction.Controllers;

public class FacilityController : AuthorizedBaseController
{
    
    // TODO: Doluluk miktar ve oranlarını da Dto'ya eklemeliyiz.
    /// <summary>
    /// Retrieves a paginated list of facilities (Logistics Depots, Gathering Areas, etc.) with optional filtering.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters</param>
    /// <returns>A paginated list of facilities</returns>
    /// <response code="200">Returns the requested page of facilities</response>
    [ProducesResponseType(typeof(PaginatedList<FacilityDto>), StatusCodes.Status200OK)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpGet]
    public async Task<IActionResult> GetAllFacilities([FromQuery] GetAllFacilitiesQuery query)
    {
        PaginatedList<FacilityDto> result = await Mediator.Send(query);

        return Ok(result);
    }

    // TODO: Doluluk miktar ve oranlarını da Dto'ya eklemeliyiz.
    // TODO: Reports (Ihbarlar) Dto'ya eklenmeli.
    /// <summary>
    /// Retrieves the details of a specific facility by its unique ID.
    /// </summary>
    /// <param name="id">The unique identifier of the facility</param>
    /// <returns>Facility details including specific infrastructure data</returns>
    /// <response code="200">Facility found and returned</response>
    /// <response code="404">Facility with the specified ID does not exist</response>
    [ProducesResponseType(typeof(FacilityDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.WarehouseManager)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFacilityById(Guid id)
    {
        FacilityDetailDto result = await Mediator.Send(new GetFacilityByIdQuery(id));

        return Ok(result);
    }
    
    // TODO: Doluluk miktar ve oranlarını da Dto'ya eklemeliyiz.
    /// <summary>
    /// Retrieves a paginated list of facilities managed by the currently authenticated user.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A paginated list of user's facilities</returns>
    /// <response code="200">Returns the requested page of user's facilities</response>
    [ProducesResponseType(typeof(PaginatedList<FacilityDto>), StatusCodes.Status200OK)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.WarehouseManager)]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyFacilities([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        GetMyFacilitiesQuery query = new(pageNumber, pageSize);
        PaginatedList<FacilityDto> result = await Mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Add a new Facility.
    /// Restricted to users with the GeneralAdmin role.
    /// </summary>
    /// <param name="command">Details of the logistics depot, including capacity and address information</param>
    /// <returns>The unique identifier (Guid) of the newly added logistics depot</returns>
    /// <response code="201">Logistics depot successfully added.</response>
    /// <response code="400">Validation error.</response>
    /// <response code="404">Associated organization not found.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPost()]
    public async Task<IActionResult> AddFacility([FromBody] AddFacilityCommand command)
    {
        Guid facilityId = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetFacilityById), new { id = facilityId }, new { id = facilityId });
    }

    /// <summary>
    /// Soft deletes a specific facility by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the facility to delete</param>
    /// <response code="204">Facility successfully deleted.</response>
    /// <response code="404">Facility not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFacility(Guid id)
    {
        await Mediator.Send(new DeleteFacilityCommand(id));
        
        return NoContent();
    }
    
    /// <summary>
    /// Assigns a new manager to an existing facility. Sends an invitation if the user does not exist.
    /// </summary>
    /// <param name="id">The unique identifier of the facility</param>
    /// <param name="request">Manager details (Email, First Name, Last Name, IsPrimary)</param>
    /// <response code="204">Manager successfully assigned to the facility.</response>
    /// <response code="400">Validation error or manager already exists in this facility.</response>
    /// <response code="404">Facility not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPost("{id:guid}/managers")]
    public async Task<IActionResult> AssignManager(Guid id, [FromBody] AssignManagerRequest request)
    {
        await Mediator.Send(new AssignFacilityManagerCommand(id, request));

        return NoContent();
    }
    
    /// <summary>
    /// Sets an existing manager as the primary manager of the facility. 
    /// Demotes the previous primary manager if one exists.
    /// </summary>
    /// <param name="id">The unique identifier of the facility</param>
    /// <param name="userId">The unique identifier of the user to be promoted</param>
    /// <response code="204">Manager successfully set as primary.</response>
    /// <response code="404">Facility or manager association not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpPut("{id:guid}/managers/{userId}/primary")]
    public async Task<IActionResult> SetPrimaryManager(Guid id, string userId)
    {
        await Mediator.Send(new SetPrimaryFacilityManagerCommand(id, userId));

        return NoContent();
    }
    
    /// <summary>
    /// Removes a manager from a facility (Soft delete the association).
    /// </summary>
    /// <param name="id">The unique identifier of the facility</param>
    /// <param name="userId">The unique identifier of the user to be removed</param>
    /// <response code="204">Manager successfully removed.</response>
    /// <response code="404">Facility or manager association not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin)]
    [HttpDelete("{id:guid}/managers/{userId}")]
    public async Task<IActionResult> UnassignManager(Guid id, string userId)
    {
        await Mediator.Send(new UnassignFacilityManagerCommand(id, userId));

        return NoContent();
    }
}