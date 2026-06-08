using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.Cart.ApplyCartCoupon;
using E_Satis_Auction.Features.Cart.ClearCart;
using E_Satis_Auction.Features.Cart.GetCart;
using E_Satis_Auction.Features.Cart.GetCartPricePreview;
using E_Satis_Auction.Features.Cart.RemoveCartCoupon;
using E_Satis_Auction.Features.Cart.UpdateCartListing;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

public sealed class CartController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        CartDto? result = await Mediator.Send(new GetCartQuery());
        return Ok(result);
    }

    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [HttpPut("listing")]
    [HttpPut("items")]
    public async Task<IActionResult> UpdateListing([FromBody] UpdateCartListingRequest request)
    {
        CartDto result = await Mediator.Send(new UpdateCartListingCommand(request));
        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        await Mediator.Send(new ClearCartCommand());
        return NoContent();
    }

    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [HttpPost("apply-coupon")]
    public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
    {
        CartDto result = await Mediator.Send(new ApplyCartCouponCommand(request));
        return Ok(result);
    }

    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [HttpDelete("coupon")]
    public async Task<IActionResult> RemoveCoupon()
    {
        CartDto result = await Mediator.Send(new RemoveCartCouponCommand());
        return Ok(result);
    }

    [ProducesResponseType(typeof(CartPricePreviewDto), StatusCodes.Status200OK)]
    [HttpGet("price-preview")]
    public async Task<IActionResult> GetPricePreview()
    {
        CartPricePreviewDto result = await Mediator.Send(new GetCartPricePreviewQuery());
        return Ok(result);
    }

    [ProducesResponseType(typeof(CartPricePreviewDto), StatusCodes.Status200OK)]
    [HttpPost("checkout-intent")]
    public async Task<IActionResult> CheckoutIntent()
    {
        CartPricePreviewDto result = await Mediator.Send(new GetCartPricePreviewQuery());
        return Ok(result);
    }
}
