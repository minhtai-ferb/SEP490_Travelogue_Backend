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

        [HttpPost("create-request")]
        public async Task<IActionResult> CreateBookingFromRequestAsync([FromBody] TourGuideBookingRequestCreateModel model, CancellationToken cancellationToken)
        {
            var result = await _bookingService.CreateRequestAsync(model, cancellationToken);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "request")
            ));
        }

        [HttpPut("reject-request/{id:guid}")]
        public async Task<IActionResult> RejectByGuideAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _bookingService.RejectByGuideAsync(id, cancellationToken);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.PROCESS_SUCCESS, "reject request")
            ));
        }

        [HttpPost("suggest-new-version/{bookingRequestId:guid}")]
        public async Task<IActionResult> SuggestNewVersionAsync(
            Guid bookingRequestId,
            Guid versionId,
            string? guideNote)
        {
            var result = await _bookingService.SuggestNewVersionAsync(bookingRequestId, versionId, guideNote);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.PROCESS_SUCCESS, "suggest new version")
            ));
        }

        [HttpPost("confirm-request/{bookingRequestId:guid}")]
        public async Task<IActionResult> ConfirmRequestToBookingAsync(Guid bookingRequestId, CancellationToken cancellationToken)
        {
            var result = await _bookingService.CreateBookingFromRequestAsync(bookingRequestId, cancellationToken);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "booking from request")
            ));
        }
    }
}
