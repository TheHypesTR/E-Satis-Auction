using e_Sat_Auction.Common;
using e_Sat_Auction.Common.Attributes;
using e_Sat_Auction.Common.Controllers;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.InventoryTransaction;
using e_Sat_Auction.Features.InventoryTransaction.GetAllInventoryTransactions;
using Microsoft.AspNetCore.Mvc;

namespace e_Sat_Auction.Controllers;

public class InventoryTransactionController : AuthorizedBaseController
{
    /// <summary>
    /// Gets a paginated list of inventory transactions (stock logs) visible to the current user.
    /// </summary>
    /// <param name="query">Filtering and pagination parameters.</param>
    /// <returns>A paginated list of inventory transactions.</returns>
    /// <response code="200">Returns the paginated and authorized list of transactions.</response>
    /// <response code="403">User is not authorized to access the specified facility.</response>
    [ProducesResponseType(typeof(PaginatedList<InventoryTransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.WarehouseManager)]
    [HttpGet]
    public async Task<IActionResult> GetAllInventoryTransactions([FromQuery] GetAllInventoryTransactionsQuery query)
    {
        PaginatedList<InventoryTransactionDto> result = await Mediator.Send(query);

        return Ok(result);
    }
}