using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("create-booking")]
        public async Task<IActionResult> AddBookingAsync([FromBody] BookingCreateModel model, CancellationToken cancellationToken)
        {
            var result = await _bookingService.AddBookingAsync(model, cancellationToken);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "booking")
            ));
        }
    }
}
