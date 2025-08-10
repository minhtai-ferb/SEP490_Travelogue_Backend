using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.BankAccountModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<List<BankAccountDto>>))]
        public async Task<IActionResult> GetUserBankAccounts(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _bankAccountService.GetUserBankAccountsAsync(userId);
            return Ok(ResponseModel<List<BankAccountDto>>.OkResponseModel(
                data: result,
                message: ResponseMessages.SUCCESS
            ));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<BankAccountDto>))]
        public async Task<IActionResult> AddBankAccount([FromBody] BankAccountCreateDto dto, CancellationToken cancellationToken)
        {
            var result = await _bankAccountService.AddBankAccountAsync(dto);
            return Ok(ResponseModel<BankAccountDto>.OkResponseModel(
                data: result,
                message: ResponseMessages.SUCCESS
            ));
        }

        [HttpPut("{bankAccountId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<BankAccountDto>))]
        public async Task<IActionResult> UpdateBankAccount(Guid bankAccountId, [FromBody] BankAccountUpdateDto dto, CancellationToken cancellationToken)
        {
            var result = await _bankAccountService.UpdateBankAccountAsync(bankAccountId, dto);
            return Ok(ResponseModel<BankAccountDto>.OkResponseModel(
                data: result,
                message: ResponseMessages.SUCCESS
            ));
        }

        [HttpDelete("{bankAccountId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<string>))]
        public async Task<IActionResult> DeleteBankAccount(Guid bankAccountId, CancellationToken cancellationToken)
        {
            await _bankAccountService.DeleteBankAccountAsync(bankAccountId);
            return Ok(ResponseModel<string>.OkResponseModel(
                data: null,
                message: ResponseMessages.SUCCESS
            ));
        }
    }
}
