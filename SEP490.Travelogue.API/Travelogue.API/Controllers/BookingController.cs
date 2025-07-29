using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        [HttpPost("create-booking-tour")]
        public async Task<IActionResult> AddTourBookingAsync([FromBody] CreateBookingTourDto model, CancellationToken cancellationToken)
        {
            var result = await _bookingService.CreateTourBookingAsync(model, cancellationToken);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "booking tour")
            ));
        }

        [HttpPost("create-booking-tour-guide")]
        public async Task<IActionResult> AddTourGuideBookingAsync([FromBody] CreateBookingTourGuideDto model, CancellationToken cancellationToken)
        {
            var result = await _bookingService.CreateTourGuideBookingAsync(model, cancellationToken);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "booking tour guide")
            ));
        }

        [HttpPost("create-booking-workshop")]
        public async Task<IActionResult> AddWorkshopBookingAsync([FromBody] CreateBookingWorkshopDto model, CancellationToken cancellationToken)
        {
            var result = await _bookingService.CreateWorkshopBookingAsync(model, cancellationToken);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "booking workshop")
            ));
        }

        [HttpPost("create-payment-link")]
        public async Task<IActionResult> CreatePaymentLinkAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            var paymentResult = await _bookingService.CreatePaymentLink(bookingId, cancellationToken);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: paymentResult,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "booking")
            ));
        }

        /// <summary>
        /// Nhận kết quả sau khi giao dịch thành công
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost("receive-hook")]
        public async Task<IActionResult> ReceiveHook([FromBody] dynamic payload)
        {
            // Xử lý payload tại đây
            Console.WriteLine(payload);
            string jsonResponse = JsonConvert.SerializeObject(payload);

            try
            {
                // Chuyển payload thành chuỗi JSON
                string jsonString = Convert.ToString(payload);
                Console.WriteLine("jsonString");
                Console.WriteLine(jsonString);

                var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(jsonString);

                var result = await _bookingService.ProcessPaymentResponseAsync(paymentResponse);
                return Ok(ResponseModel<object>.OkResponseModel(
                 data: result,
                 message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "booking")
             ));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi chuyển đổi payload: " + ex.Message);
                return BadRequest("Dữ liệu không hợp lệ.");
            }
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings([FromQuery] BookingFilterDto filter)
        {
            var result = await _bookingService.GetUserBookingsAsync(filter);
            return Ok(ResponseModel<object>.OkResponseModel(
                 data: result,
                 message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "booking")
             ));
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingById(Guid bookingId)
        {
            var result = await _bookingService.GetBookingByIdAsync(bookingId);
            return Ok(ResponseModel<object>.OkResponseModel(
                 data: result,
                 message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "booking")
             ));
        }
    }
}
