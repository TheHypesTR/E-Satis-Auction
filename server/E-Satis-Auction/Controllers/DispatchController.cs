using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Attributes;
using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Dispatch;
using E_Satis_Auction.Dtos.Dispatch.Requests;
using E_Satis_Auction.Features.Dispatch.CancelDispatch;
using E_Satis_Auction.Features.Dispatch.CompleteAddressDispatch;
using E_Satis_Auction.Features.Dispatch.CreateDispatch;
using E_Satis_Auction.Features.Dispatch.GetAllDispatches;
using E_Satis_Auction.Features.Dispatch.GetDispatchById;
using E_Satis_Auction.Features.Dispatch.ReceiveDispatch;
using E_Satis_Auction.Features.Dispatch.ShipDispatch;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

public class DispatchController : AuthorizedBaseController
{
    /// <summary>
    /// Gets a paginated list of dispatches visible to the current user.
    /// </summary>
    /// <param name="query">Pagination and filtering options.</param>
    /// <returns>A paginated list of dispatches.</returns>
    /// <response code="200">Returns the paginated and authorized list of dispatches.</response>
    [ProducesResponseType(typeof(PaginatedList<DispatchSummaryDto>), StatusCodes.Status200OK)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.WarehouseManager)]
    [HttpGet]
    public async Task<IActionResult> GetAllDispatches([FromQuery] GetAllDispatchesQuery query)
    {
        PaginatedList<DispatchSummaryDto> result = await Mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves details of a specific dispatch including line items and target details.
    /// </summary>
    /// <param name="id">The unique identifier of the dispatch.</param>
    /// <returns>The dispatch details.</returns>
    /// <response code="200">Returns the dispatch details.</response>
    /// <response code="403">User is not authorized to view the dispatch.</response>
    /// <response code="404">Dispatch not found.</response>
    [ProducesResponseType(typeof(DispatchDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.WarehouseManager)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDispatchById(Guid id)
    {
        DispatchDetailDto result = await Mediator.Send(new GetDispatchByIdQuery(id));

        return Ok(result);
    }

    /// <summary>
    /// Creates a new dispatch (shipment) from a specific source facility to a target facility or address.
    /// Deducts inventory from the source facility and creates a reserved dispatch order.
    /// Requires authorization over the source facility.
    /// </summary>
    /// <param name="sourceFacilityId">The unique identifier of the facility where the items are dispatched from.</param>
    /// <param name="request">Dispatch details including target destinations, receiver info, and item quantities.</param>
    /// <returns>The unique identifier (Guid) of the newly created dispatch.</returns>
    /// <response code="201">Dispatch successfully created.</response>
    /// <response code="400">Validation error (e.g., target destination mismatch, insufficient stock, duplicate items).</response>
    /// <response code="403">User is not authorized to dispatch items from the specified source facility.</response>
    /// <response code="404">An item specified in the payload was not found in the source facility.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.WarehouseManager)]
    [HttpPost("facility/{sourceFacilityId:guid}")]
    public async Task<IActionResult> CreateDispatch(Guid sourceFacilityId, [FromBody] CreateDispatchRequest request)
    {
        Guid dispatchId = await Mediator.Send(new CreateDispatchCommand(sourceFacilityId, request));

        return CreatedAtAction(nameof(CreateDispatch), new { id = dispatchId }, new { id = dispatchId });
    }
    
    /// <summary>
    /// Ships a pending dispatch. Changes the status of the dispatch and its associated items to InTransit.
    /// Requires authorization over the source facility.
    /// </summary>
    /// <param name="id">The unique identifier of the dispatch to ship.</param>
    /// <response code="204">Dispatch successfully shipped.</response>
    /// <response code="400">Validation error or dispatch is not in Pending status.</response>
    /// <response code="403">User is not authorized to ship from the source facility.</response>
    /// <response code="404">Dispatch or associated items not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.WarehouseManager)]
    [HttpPut("{id:guid}/ship")]
    public async Task<IActionResult> ShipDispatch(Guid id)
    {
        await Mediator.Send(new ShipDispatchCommand(id));

        return NoContent();
    }

    /// <summary>
    /// Completes an in-transit dispatch at the target facility, updating its status Completed.
    /// Requires authorization over the target facility.
    /// </summary>
    /// <param name="id">The unique identifier of the dispatch to complete.</param>
    /// <param name="request">Details of the received dispatch including item conditions and quantities.</param>
    /// <response code="204">Dispatch successfully received and completed.</response>
    /// <response code="400">Validation error (e.g., item conditions not met, quantity mismatch).</response>
    /// <response code="403">User is not authorized to receive dispatches at the target facility.</response>
    /// <response code="404">Dispatch not found or already completed.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.WarehouseManager)]
    [HttpPut("{id:guid}/receive")]
    public async Task<IActionResult> ReceiveDispatch(Guid id, [FromBody] ReceiveDispatchRequest request)
    {
        await Mediator.Send(new ReceiveDispatchCommand(id, request));

        return NoContent();
    }

    /// <summary>
    /// Completes a dispatch delivery to a specific address, updating its status and recording delivery details.
    /// Requires authorization over the source facility.
    /// </summary>
    /// <param name="id">The unique identifier of the dispatch to complete delivery for.</param>
    /// <param name="request">Delivery confirmation details for the address.</param>
    /// <response code="204">Delivery successfully completed and dispatch status updated.</response>
    /// <response code="400">Validation error (e.g., invalid status, missing address).</response>
    /// <response code="403">User is not authorized to complete delivery for this dispatch.</response>
    /// <response code="404">Dispatch not found or already completed.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.WarehouseManager)]
    [HttpPut("{id:guid}/complete-address-delivery")]
    public async Task<IActionResult> CompleteAddressDelivery(Guid id, [FromBody] CompleteAddressDispatchRequest request)
    {
        await Mediator.Send(new CompleteAddressDispatchCommand(id, request));

        return NoContent();
    }
    
    /// <summary>
    /// Cancels a pending dispatch and restores reserved items to available stock.
    /// Requires authorization over the source facility.
    /// </summary>
    /// <param name="id">The unique identifier of the dispatch to cancel.</param>
    /// <param name="request">Optional cancellation note.</param>
    /// <response code="204">Dispatch successfully canceled.</response>
    /// <response code="400">Validation error or dispatch is not in Pending status.</response>
    /// <response code="403">User is not authorized to cancel dispatches from the source facility.</response>
    /// <response code="404">Dispatch or associated items not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.WarehouseManager)]
    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> CancelDispatch(Guid id, [FromBody] CancelDispatchRequest request)
    {
        await Mediator.Send(new CancelDispatchCommand(id, request));

        return NoContent();
    }
}