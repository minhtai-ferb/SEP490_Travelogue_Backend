using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TransactionModels;
using Travelogue.Service.BusinessModels.WithdrawalRequestModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet("balance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<decimal>))]
    public async Task<IActionResult> GetBalance(CancellationToken cancellationToken)
    {
        var result = await _walletService.GetBalanceAsync();
        return Ok(ResponseModel<decimal>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpGet("transactions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<List<TransactionDto>>))]
    public async Task<IActionResult> GetTransactions(CancellationToken cancellationToken)
    {
        var result = await _walletService.GetTransactionsAsync();
        return Ok(ResponseModel<List<TransactionDto>>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpPost("withdrawal-request")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<string>))]
    public async Task<IActionResult> RequestWithdrawal([FromBody] WithdrawalRequestCreateDto request, CancellationToken cancellationToken)
    {
        await _walletService.RequestWithdrawalAsync(request);
        return Ok(ResponseModel<string>.OkResponseModel(
            data: null,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpGet("withdrawal-requests/filter")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<List<WithdrawalRequestDto>>))]
    public async Task<IActionResult> GetWithdrawalRequests([FromQuery] WithdrawalRequestFilterDto filterDto, CancellationToken cancellationToken)
    {
        var result = await _walletService.GetWithdrawalRequestAsync(filterDto);
        return Ok(ResponseModel<List<WithdrawalRequestDto>>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpGet("my-withdrawal-requests/filter")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<List<WithdrawalRequestDto>>))]
    public async Task<IActionResult> GetMyWithdrawalRequests([FromQuery] MyWithdrawalRequestFilterDto filterDto, CancellationToken cancellationToken)
    {
        var result = await _walletService.GetMyWithdrawalRequestAsync(filterDto);
        return Ok(ResponseModel<List<WithdrawalRequestDto>>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpPatch("withdrawal-requests/{requestId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<string>))]
    public async Task<IActionResult> Approve(Guid requestId, [FromQuery] string proofImageUrl, [FromQuery] string? adminNote, CancellationToken cancellationToken)
    {
        await _walletService.ApproveAsync(requestId, proofImageUrl, adminNote);
        return Ok(ResponseModel<string>.OkResponseModel(
            data: null,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpPatch("withdrawal-requests/{requestId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<string>))]
    public async Task<IActionResult> Reject(Guid requestId, [FromQuery] string reason, CancellationToken cancellationToken)
    {
        await _walletService.RejectAsync(requestId, reason);
        return Ok(ResponseModel<string>.OkResponseModel(
            data: null,
            message: ResponseMessages.SUCCESS
        ));
    }
}
