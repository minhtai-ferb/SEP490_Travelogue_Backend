using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TypeExperienceModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TypeExperienceController : ControllerBase
{
    private readonly ITypeExperienceService _typeExperienceService;

    public TypeExperienceController(ITypeExperienceService typeExperienceService)
    {
        _typeExperienceService = typeExperienceService;
    }

    /// <summary>
    /// Tạo mới typeExperience
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateTypeExperience([FromBody] TypeExperienceCreateModel model)
    {
        await _typeExperienceService.AddTypeExperienceAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "type experience")
        ));
    }

    /// <summary>
    /// Xóa typeExperience theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTypeExperience(Guid id)
    {
        await _typeExperienceService.DeleteTypeExperienceAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "type experience")
        ));
    }

    /// <summary>
    /// Lấy tất cả typeExperience
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllTypeExperiences()
    {
        var typeExperiences = await _typeExperienceService.GetAllTypeExperiencesAsync(new CancellationToken());
        return Ok(ResponseModel<List<TypeExperienceDataModel>>.OkResponseModel(
            data: typeExperiences,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type experience")
        ));
    }

    /// <summary>
    /// Lấy typeExperience theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeExperienceById(Guid id)
    {
        var typeExperience = await _typeExperienceService.GetTypeExperienceByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<TypeExperienceDataModel>.OkResponseModel(
            data: typeExperience,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type experience")
        ));
    }
    /// <summary>
    /// Cập nhật typeExperience
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTypeExperience(Guid id, [FromBody] TypeExperienceUpdateModel model)
    {
        await _typeExperienceService.UpdateTypeExperienceAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "type experience")
        ));
    }

    /// <summary>
    /// Lấy danh sách typeExperience phân trang theo tiêu đề
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <param name="name">Tiêu đề cần tìm kiếm</param>
    /// <returns>Trả về danh sách các typeExperience</returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedTypeExperienceWithSearch(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var typeExperiences = await _typeExperienceService.GetPagedTypeExperiencesWithSearchAsync(name, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: typeExperiences.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type experience"),
            totalCount: typeExperiences.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    //[HttpGet("get-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedTypeExperience(int pageNumber = 1, int pageSize = 10)
    //{
    //    var typeExperiences = await _typeExperienceService.GetPagedTypeExperiencesAsync(pageNumber, pageSize, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: typeExperiences.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type experience"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}
}
