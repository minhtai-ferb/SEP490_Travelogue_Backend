using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TourModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly IWalletService _walletService;

    public TransactionController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet("users/{userId:guid}/list")]
    public async Task<IActionResult> GetUserTransactions([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _walletService.GetTransactionsByUserIdAsync(userId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTransactions(CancellationToken cancellationToken)
    {
        var result = await _walletService.GetAllTransactionsAsync(cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpGet("top-system-transactions")]
    public async Task<IActionResult> GetTopSystemTransactions(
    [FromQuery] int top = 5,
    CancellationToken cancellationToken = default)
    {
        var result = await _walletService.GetTopSystemTransactionsAsync(top, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }
}
