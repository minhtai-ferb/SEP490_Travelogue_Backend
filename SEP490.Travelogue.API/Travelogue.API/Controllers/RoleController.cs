using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.RoleModels.Responses;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // [HttpPost]
    // public async Task<IActionResult> CreateRole([FromBody] RoleRequestModel model)
    // {
    //     var result = await _roleService.CreateRole(model, new CancellationToken());
    //     return Ok(new ResponseModel<bool>
    //             (statusCode: StatusCodes.Status200OK,
    //             message: $"{ResponseMessages.CREATE_SUCCESS.Replace("{0}", "role")}",
    //             data: result));
    // }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(Guid id)
    {
        var result = await _roleService.GetRoleById(id, new CancellationToken());
        return Ok(new ResponseModel<RoleResponseModel>
                (statusCode: StatusCodes.Status200OK,
                message: $"{ResponseMessages.GET_SUCCESS.Replace("{0}", "role")}",
                data: result));
    }

    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedCraftVillageWithSearch(int pageNumber = 1, int pageSize = 10, string name = "")
    {
        var craftVillages = await _roleService.GetPagedRolesWithSearchAsync(pageNumber, pageSize, name, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: craftVillages.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "role"),
            totalCount: craftVillages.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }
}
