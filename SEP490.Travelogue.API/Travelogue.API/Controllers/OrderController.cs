using Microsoft.AspNetCore.Mvc;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{

    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("create-payment-link")]
    public async Task<IActionResult> CreatePaymentLinkAsync([FromBody] CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return BadRequest("Invalid request data.");
        }

        var paymentResult = await _orderService.CreatePaymentLink(request, cancellationToken);
        if (paymentResult == null)
        {
            return StatusCode(500, "Failed to create payment link.");
        }

        return Ok(new
        {
            PaymentLink = paymentResult,
            Message = "Payment link created successfully."
        });
    }
}