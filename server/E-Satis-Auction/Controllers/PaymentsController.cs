using E_Satis_Auction.Common.Controllers;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Features.Payment.ConfirmPayment;
using E_Satis_Auction.Features.Payment.FailPayment;
using E_Satis_Auction.Features.Payment.GetPaymentAttempt;
using E_Satis_Auction.Features.Payment.InitiatePayment;
using Microsoft.AspNetCore.Mvc;

namespace E_Satis_Auction.Controllers;

public sealed class PaymentsController : AuthorizedBaseController
{
    [ProducesResponseType(typeof(PaymentInitiationDto), StatusCodes.Status201Created)]
    [HttpPost("initiate")]
    public async Task<IActionResult> Initiate([FromBody] InitiatePaymentRequest request)
    {
        PaymentInitiationDto result = await Mediator.Send(new InitiatePaymentCommand(request));
        return CreatedAtAction(nameof(GetPayment), new { id = result.Payment.Id }, result);
    }

    [ProducesResponseType(typeof(PaymentAttemptDto), StatusCodes.Status200OK)]
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmPaymentRequest request)
    {
        PaymentAttemptDto result = await Mediator.Send(new ConfirmPaymentCommand(id, request));
        return Ok(result);
    }

    [ProducesResponseType(typeof(PaymentAttemptDto), StatusCodes.Status200OK)]
    [HttpPost("{id:guid}/fail")]
    public async Task<IActionResult> Fail(Guid id, [FromBody] FailPaymentRequest request)
    {
        PaymentAttemptDto result = await Mediator.Send(new FailPaymentCommand(id, request));
        return Ok(result);
    }

    [ProducesResponseType(typeof(PaymentAttemptDto), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        PaymentAttemptDto result = await Mediator.Send(new GetPaymentAttemptQuery(id));
        return Ok(result);
    }
}
