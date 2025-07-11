using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.ExchangeModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        private readonly IExchangeSessionService _exchangeSessionService;
        private readonly IExchangeService _exchangeService;

        public ExchangeController(IExchangeSessionService exchangeSessionService, IExchangeService exchangeService)
        {
            _exchangeSessionService = exchangeSessionService;
            _exchangeService = exchangeService;
        }

        // tour guide gửi lại cho người dùng
        [HttpPost]
        public async Task<IActionResult> SuggestedTripPlan(UpdateUserResponseModel updateUserResponseModel, CancellationToken cancellationToken)
        {
            await _exchangeService.UpdateUserResponseAsync(updateUserResponseModel, cancellationToken);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: true,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "exchange")
            ));
        }

        [HttpGet("sessions/{sessionId}")]
        public async Task<IActionResult> GetExchangeSessionDataDetail(Guid sessionId, CancellationToken cancellationToken)
        {
            var result = await _exchangeSessionService.GetSessionByIdAsync(sessionId, cancellationToken);
            return Ok(ResponseModel<object>.OkResponseModel(
                 data: result,
                 message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "exchange")
            ));
        }
    }
}
