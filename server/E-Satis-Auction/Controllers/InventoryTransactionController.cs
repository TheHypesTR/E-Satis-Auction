using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Attributes;
using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.InventoryTransaction;
using E_Satis_Auction.Features.InventoryTransaction.GetAllInventoryTransactions;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

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