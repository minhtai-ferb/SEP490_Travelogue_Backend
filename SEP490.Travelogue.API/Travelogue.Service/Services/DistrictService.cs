using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.DistrictModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IDistrictService
{
    Task<DistrictDataModel?> GetDistrictByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<DistrictDataModel>> GetAllDistrictsAsync(CancellationToken cancellationToken);
    Task AddDistrictAsync(DistrictCreateModel districtCreateModel, CancellationToken cancellationToken);
    Task UpdateDistrictAsync(Guid id, DistrictUpdateModel districtUpdateModel, CancellationToken cancellationToken);
    Task DeleteDistrictAsync(Guid id, CancellationToken cancellationToken);
    //Task<PagedResult<DistrictDataModel>> GetPagedDistrictsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<DistrictDataModel>> GetPagedDistrictsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    //Task<string> GetDistrictRoleNameAsync(DistrictCreateModel districtCreateModel, CancellationToken cancellationToken);
    Task<bool> AddDistrictWithRoleAsync(DistrictCreateModel districtCreateModel, CancellationToken cancellationToken);
    Task<DistrictMediaResponse> AddDistrictWithMediaAsync(DistrictCreateWithMediaFileModel districtCreateModel, CancellationToken cancellationToken);
    //Task<DistrictMediaResponse> UploadMediaAsync(Guid id, IFormFile imageUpload, CancellationToken cancellationToken);
    Task<DistrictMediaResponse> UpdateDistrictAsync(Guid id, DistrictUpdateWithMediaFileModel districtUpdateModel, CancellationToken cancellationToken);
    Task<DistrictMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
}

public class DistrictService : IDistrictService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly IMediaCloudService _mediaCloudService;
    private readonly ICloudinaryService _cloudinaryService;

    public DistrictService(
        IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService,
        IMediaCloudService mediaCloudService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _mediaCloudService = mediaCloudService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task AddDistrictAsync(DistrictCreateModel districtCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newDistrict = _mapper.Map<District>(districtCreateModel);
            newDistrict.CreatedBy = currentUserId;
            newDistrict.LastUpdatedBy = currentUserId;
            newDistrict.CreatedTime = currentTime;
            newDistrict.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.DistrictRepository.AddAsync(newDistrict);
            _unitOfWork.CommitTransaction();
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    public async Task<bool> AddDistrictWithRoleAsync(DistrictCreateModel districtCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            districtCreateModel.Name = GenerateFormattedDistrictName(districtCreateModel.Name);

            // chỗ này dùng .Entities vì cần lấy kể cả những district đã bị xóa
            var districtExist = _unitOfWork.DistrictRepository.Entities
                .FirstOrDefault(d => d.Name == districtCreateModel.Name);

            // khôi phục district đã bị xóa 
            if (districtExist != null)
            {
                if (districtExist.IsDeleted)
                {
                    districtExist.IsDeleted = false;
                    districtExist.IsActive = true;
                    districtExist.LastUpdatedBy = currentUserId;
                    districtExist.LastUpdatedTime = currentTime;

                    _unitOfWork.DistrictRepository.Update(districtExist);

                    // khôi phục role district
                    var roleDistrictExist = _unitOfWork.RoleDistrictRepository.Entities
                        .FirstOrDefault(rd => rd.DistrictId == districtExist.Id);
                    if (roleDistrictExist != null)
                    {
                        roleDistrictExist.IsActive = true;
                        roleDistrictExist.LastUpdatedBy = currentUserId;
                        roleDistrictExist.LastUpdatedTime = currentTime;
                        _unitOfWork.RoleDistrictRepository.Update(roleDistrictExist);
                    }

                    if (roleDistrictExist != null)
                    {
                        // khôi phục role tương ứng với district
                        var roleExist = _unitOfWork.RoleRepository.Entities
                            .FirstOrDefault(r => r.Id == roleDistrictExist.RoleId);
                        if (roleExist != null)
                        {
                            roleExist.IsActive = true;
                            roleExist.LastUpdatedBy = currentUserId;
                            roleExist.LastUpdatedTime = currentTime;
                            _unitOfWork.RoleRepository.Update(roleExist);
                        }
                    }

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync(cancellationToken);
                    return true;
                }
                else
                {
                    throw CustomExceptionFactory.CreateBadRequestError($"{ResponseMessages.EXISTED.Replace("{0}", "đơn vị hành chính")}");
                }
            }

            var newDistrict = _mapper.Map<District>(districtCreateModel);
            newDistrict.CreatedBy = currentUserId;
            newDistrict.LastUpdatedBy = currentUserId;
            newDistrict.CreatedTime = currentTime;
            newDistrict.LastUpdatedTime = currentTime;

            await _unitOfWork.DistrictRepository.AddAsync(newDistrict);

            // tạo role cho district
            //var roleName = GenerateRoleName(newDistrict.Name);

            var result = await _unitOfWork.RoleRepository
                .AddAsync(new Role(newDistrict.Name)
                {
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                });

            // gán district cho role
            var addRoleDistrict = await _unitOfWork.RoleDistrictRepository
                .AddAsync(new RoleDistrict
                {
                    RoleId = result.Id,
                    DistrictId = newDistrict.Id,
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                });

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task DeleteDistrictAsync(Guid id, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingDistrict = await _unitOfWork.DistrictRepository.GetByIdAsync(id, cancellationToken);

            if (existingDistrict == null || existingDistrict.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("district");
            }

            // xóa mềm district
            existingDistrict.IsDeleted = true;
            existingDistrict.DeletedBy = currentUserId;
            existingDistrict.DeletedTime = currentTime;
            existingDistrict.LastUpdatedBy = currentUserId;
            existingDistrict.LastUpdatedTime = currentTime;
            _unitOfWork.DistrictRepository.Update(existingDistrict);

            // xóa mềm role districts liên quan
            var roleDistricts = _unitOfWork.RoleDistrictRepository.Entities
                .Where(rd => rd.DistrictId == id)
                .ToList();

            foreach (var roleDistrict in roleDistricts)
            {
                roleDistrict.IsActive = false;
                roleDistrict.IsDeleted = true;
                roleDistrict.DeletedBy = currentUserId;
                roleDistrict.DeletedTime = currentTime;
                roleDistrict.LastUpdatedBy = currentUserId;
                roleDistrict.LastUpdatedTime = currentTime;
                _unitOfWork.RoleDistrictRepository.Update(roleDistrict);

                // xóa mềm role 
                var hasOtherLinks = _unitOfWork.RoleDistrictRepository.Entities
                    .Any(rd => rd.RoleId == roleDistrict.RoleId && !rd.IsDeleted);

                if (!hasOtherLinks)
                {
                    var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleDistrict.RoleId, cancellationToken);
                    if (role != null && !role.IsDeleted)
                    {
                        role.IsActive = false;
                        role.IsDeleted = true;
                        role.DeletedBy = currentUserId;
                        role.DeletedTime = currentTime;
                        role.LastUpdatedBy = currentUserId;
                        role.LastUpdatedTime = currentTime;
                        _unitOfWork.RoleRepository.Update(role);
                    }
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<List<DistrictDataModel>> GetAllDistrictsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingDistrict = await _unitOfWork.DistrictRepository.GetAllAsync(cancellationToken);
            if (existingDistrict == null || existingDistrict.Count() == 0)
            {
                return new List<DistrictDataModel>();
            }

            var dataModel = _mapper.Map<List<DistrictDataModel>>(existingDistrict);

            foreach (var item in dataModel)
            {
                var districtMedia = _unitOfWork.DistrictMediaRepository.ActiveEntities
                    .Where(dm => dm.DistrictId == item.Id)
                    .OrderByDescending(dm => dm.CreatedTime)
                    .ToList();

                foreach (var media in districtMedia)
                {
                    item.Medias.Add(new MediaResponse
                    {
                        MediaUrl = media.MediaUrl,
                        FileName = media.FileName ?? string.Empty,
                        FileType = media.FileType,
                        SizeInBytes = media.SizeInBytes,
                        CreatedTime = media.CreatedTime,
                    });
                }
            }

            return dataModel;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    public async Task<DistrictDataModel?> GetDistrictByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingDistrict = await _unitOfWork.DistrictRepository.GetByIdAsync(id, cancellationToken);
            if (existingDistrict == null || existingDistrict.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("district");
            }

            var dataModel = _mapper.Map<DistrictDataModel>(existingDistrict);

            var districtMedia = _unitOfWork.DistrictMediaRepository.ActiveEntities
                .Where(dm => dm.DistrictId == dataModel.Id)
                .OrderByDescending(dm => dm.CreatedTime)
                .ToList();

            foreach (var media in districtMedia)
            {
                dataModel.Medias.Add(new MediaResponse
                {
                    MediaUrl = media.MediaUrl,
                    FileName = media.FileName ?? string.Empty,
                    FileType = media.FileType,
                    SizeInBytes = media.SizeInBytes,
                    CreatedTime = media.CreatedTime,
                });
            }

            return dataModel;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    public async Task<DistrictDataModel?> GetDistrictByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var existingDistrict = await _unitOfWork.DistrictRepository.GetByNameAsync(name, cancellationToken);
            if (existingDistrict == null || existingDistrict.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("district");
            }

            return _mapper.Map<DistrictDataModel>(existingDistrict);
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<DistrictDataModel>> GetPagedDistrictsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.DistrictRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

            var districtDataModels = _mapper.Map<List<DistrictDataModel>>(pagedResult.Items);

            foreach (var item in districtDataModels)
            {
                var districtMedia = _unitOfWork.DistrictMediaRepository.ActiveEntities
                    .Where(dm => dm.DistrictId == item.Id)
                    .OrderByDescending(dm => dm.CreatedTime)
                    .ToList();

                foreach (var media in districtMedia)
                {
                    item.Medias.Add(new MediaResponse
                    {
                        MediaUrl = media.MediaUrl,
                        FileName = media.FileName ?? string.Empty,
                        FileType = media.FileType,
                        SizeInBytes = media.SizeInBytes,
                        CreatedTime = media.CreatedTime,
                    });
                }
            }

            return new PagedResult<DistrictDataModel>
            {
                Items = districtDataModels,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    public async Task UpdateDistrictAsync(Guid id, DistrictUpdateModel districtUpdateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingDistrict = await _unitOfWork.DistrictRepository.GetByIdAsync(id, cancellationToken);
            if (existingDistrict == null || existingDistrict.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("district");
            }

            _mapper.Map(districtUpdateModel, existingDistrict);
            existingDistrict.LastUpdatedBy = currentUserId;
            existingDistrict.LastUpdatedTime = currentTime;

            _unitOfWork.DistrictRepository.Update(existingDistrict);

            // cập nhật lại role
            //var roleName = GenerateRoleName(existingDistrict.Name);
            var roleDistrict = _unitOfWork.RoleDistrictRepository.Entities
                .FirstOrDefault(rd => rd.DistrictId == existingDistrict.Id);

            if (roleDistrict != null)
            {
                // update
                var existingRole = await _unitOfWork.RoleRepository.GetByIdAsync(roleDistrict.RoleId, cancellationToken);
                if (existingRole != null)
                {
                    // xử dụng hàm này vì đây là RULE CỦA CHƯƠNG TRÌNH
                    existingRole.SetName(existingDistrict.Name);
                    existingRole.LastUpdatedBy = currentUserId;
                    existingRole.LastUpdatedTime = currentTime;

                    _unitOfWork.RoleRepository.Update(existingRole);
                }
            }
            else
            {
                // create new
                var newRole = new Role(existingDistrict.Name)
                {
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                };

                await _unitOfWork.RoleRepository.AddAsync(newRole);
                await _unitOfWork.SaveAsync();

                var newRoleDistrict = new RoleDistrict
                {
                    RoleId = newRole.Id,
                    DistrictId = existingDistrict.Id,
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                };
                await _unitOfWork.RoleDistrictRepository.AddAsync(newRoleDistrict);
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<DistrictMediaResponse> UpdateDistrictAsync(Guid id, DistrictUpdateWithMediaFileModel districtUpdateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingDistrict = await _unitOfWork.DistrictRepository.GetByIdAsync(id, cancellationToken);
            if (existingDistrict == null || existingDistrict.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("district");
            }

            // Cập nhật thông tin District
            _mapper.Map(districtUpdateModel, existingDistrict);
            existingDistrict.LastUpdatedBy = currentUserId;
            existingDistrict.LastUpdatedTime = currentTime;
            _unitOfWork.DistrictRepository.Update(existingDistrict);

            // Cập nhật hoặc tạo mới Role
            var roleDistrict = _unitOfWork.RoleDistrictRepository.Entities.FirstOrDefault(rd => rd.DistrictId == existingDistrict.Id);
            if (roleDistrict != null)
            {
                var existingRole = await _unitOfWork.RoleRepository.GetByIdAsync(roleDistrict.RoleId, cancellationToken);
                if (existingRole != null)
                {
                    existingRole.SetName(existingDistrict.Name);
                    existingRole.LastUpdatedBy = currentUserId;
                    existingRole.LastUpdatedTime = currentTime;
                    _unitOfWork.RoleRepository.Update(existingRole);
                }
            }
            else
            {
                // khó xảy ra vlon nhưng mà chat gpt kêu làm z mới tốt nên để
                // cái này chỉ xảy ra khi update một district mà không có trong bảng role - district 
                var newRole = new Role(existingDistrict.Name.Trim())
                {
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                };

                await _unitOfWork.RoleRepository.AddAsync(newRole);
                await _unitOfWork.SaveAsync();

                var newRoleDistrict = new RoleDistrict
                {
                    RoleId = newRole.Id,
                    DistrictId = existingDistrict.Id,
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                };
                await _unitOfWork.RoleDistrictRepository.AddAsync(newRoleDistrict);
            }

            // upload ảnh
            var mediaResponses = new List<MediaResponse>();
            var imageUpload = districtUpdateModel.ImageUpload;
            string? newMediaUrl = null;
            if (imageUpload != null)
            {
                // Xóa ảnh cũ nếu tồn tại
                var oldMedia = _unitOfWork.DistrictMediaRepository.ActiveEntities
                    .FirstOrDefault(dm => dm.DistrictId == existingDistrict.Id);
                if (oldMedia != null)
                {
                    await _cloudinaryService.DeleteImageAsync(oldMedia.MediaUrl);
                    _unitOfWork.DistrictMediaRepository.Remove(oldMedia);
                }

                // Upload ảnh mới
                newMediaUrl = await _cloudinaryService.UploadImageAsync(imageUpload);

                var newDistrictMedia = new DistrictMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    DistrictId = existingDistrict.Id,
                    MediaUrl = newMediaUrl,
                    SizeInBytes = imageUpload.Length,
                    CreatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedBy = currentUserId
                };

                await _unitOfWork.DistrictMediaRepository.AddAsync(newDistrictMedia);
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new DistrictMediaResponse
            {
                DistrictId = existingDistrict.Id,
                DistrictName = existingDistrict.Name,
                Media = mediaResponses
            };
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<DistrictMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingDistrict = await _unitOfWork.DistrictRepository.GetByIdAsync(id, cancellationToken);
            if (existingDistrict == null || existingDistrict.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("district");
            }

            if (imageUploads == null || !imageUploads.Any())
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
            var mediaResponses = new List<MediaResponse>();

            for (int i = 0; i < imageUploads.Count; i++)
            {
                var imageUpload = imageUploads[i];

                var newDistrictMedia = new DistrictMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    DistrictId = existingDistrict.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.DistrictMediaRepository.AddAsync(newDistrictMedia);

                mediaResponses.Add(new MediaResponse
                {
                    MediaUrl = imageUrls[i],
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    SizeInBytes = imageUpload.Length
                });
            }

            await _unitOfWork.SaveAsync();
            _unitOfWork.CommitTransaction();

            return new DistrictMediaResponse
            {
                DistrictId = existingDistrict.Id,
                DistrictName = existingDistrict.Name,
                Media = mediaResponses
            };
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<DistrictMediaResponse> AddDistrictWithMediaAsync(DistrictCreateWithMediaFileModel districtCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            districtCreateModel.Name = GenerateFormattedDistrictName(districtCreateModel.Name);

            // Lấy District kể cả khi đã bị xóa
            var districtExist = _unitOfWork.DistrictRepository.Entities
                .FirstOrDefault(d => d.Name == districtCreateModel.Name);

            District district;
            if (districtExist != null)
            {
                // Khôi phục district nếu bị xóa
                if (districtExist.IsDeleted)
                {
                    districtExist.IsDeleted = false;
                    districtExist.IsActive = true;
                    districtExist.LastUpdatedBy = currentUserId;
                    districtExist.LastUpdatedTime = currentTime;
                    _unitOfWork.DistrictRepository.Update(districtExist);
                }

                district = districtExist;
            }
            else
            {
                // Tạo mới district
                district = _mapper.Map<District>(districtCreateModel);
                district.CreatedBy = currentUserId;
                district.LastUpdatedBy = currentUserId;
                district.CreatedTime = currentTime;
                district.LastUpdatedTime = currentTime;
                await _unitOfWork.DistrictRepository.AddAsync(district);
            }

            // Tạo hoặc khôi phục Role & RoleDistrict
            var roleDistrictExist = _unitOfWork.RoleDistrictRepository.Entities
                .FirstOrDefault(rd => rd.DistrictId == district.Id);

            if (roleDistrictExist != null)
            {
                roleDistrictExist.IsActive = true;
                roleDistrictExist.LastUpdatedBy = currentUserId;
                roleDistrictExist.LastUpdatedTime = currentTime;
                _unitOfWork.RoleDistrictRepository.Update(roleDistrictExist);
            }
            else
            {
                var newRole = await _unitOfWork.RoleRepository.AddAsync(new Role(district.Name.Trim())
                {
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                });

                await _unitOfWork.RoleDistrictRepository.AddAsync(new RoleDistrict
                {
                    RoleId = newRole.Id,
                    DistrictId = district.Id,
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                });
            }

            // upload anh
            var mediaResponses = new List<MediaResponse>();
            var imageUpload = districtCreateModel.ImageUpload;
            if (imageUpload != null)
            {
                var imageUrls = await _cloudinaryService.UploadImageAsync(imageUpload);

                var newDistrictMedia = new DistrictMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    DistrictId = district.Id,
                    MediaUrl = imageUrls,
                    SizeInBytes = imageUpload.Length,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.DistrictMediaRepository.AddAsync(newDistrictMedia);

                mediaResponses.Add(new MediaResponse
                {
                    MediaUrl = imageUrls,
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    SizeInBytes = imageUpload.Length
                });
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new DistrictMediaResponse
            {
                DistrictId = district.Id,
                DistrictName = district.Name,
                Media = mediaResponses
            };
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    // ----------------- Helper -----------------
    public static string GenerateNormalizedRoleName(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "admin";

        string normalized = input.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        string noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);
        string formatted = Regex.Replace(noDiacritics, @"\s+", "_").ToLower();

        return $"admin_{formatted}";
    }

    public static string GenerateRoleName(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "Admin";

        input = input.ToLower();

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        string titleCase = textInfo.ToTitleCase(input);

        return $"Admin {titleCase}";
    }

    public static string GenerateFormattedDistrictName(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "Admin";

        input = input.ToLower();

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        string titleCase = textInfo.ToTitleCase(input);

        return titleCase;
    }

    //public async Task<DistrictMediaResponse> UploadMediaAsync(Guid id, IFormFile imageUpload, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var currentUserId = _userContextService.GetCurrentUserId();
    //        var existingDistrict = await _unitOfWork.DistrictRepository.GetByIdAsync(id, cancellationToken);
    //        if (existingDistrict == null || existingDistrict.IsDeleted)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("district");
    //        }

    //        string imageUrl = string.Empty;

    //        if (imageUpload != null && imageUpload.Length > 0)
    //        {
    //            imageUrl = await _cloudinaryService.UploadImageAsync(imageUpload);
    //        }
    //        else
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("image");
    //        }

    //        var newDistrictMedia = new DistrictMedia
    //        {
    //            FileName = imageUpload.FileName,
    //            FileType = imageUpload.ContentType,
    //            DistrictId = existingDistrict.Id,
    //            MediaUrl = imageUrl,
    //            SizeInBytes = imageUpload.Length,
    //            CreatedBy = currentUserId,
    //            CreatedTime = _timeService.SystemTimeNow,
    //            LastUpdatedBy = currentUserId,
    //        };

    //        await _unitOfWork.DistrictMediaRepository.AddAsync(newDistrictMedia);
    //        await _unitOfWork.SaveAsync();

    //        return new DistrictMediaResponse
    //        {
    //            DistrictId = existingDistrict.Id,
    //            DistrictName = existingDistrict.Name,
    //            MediaUrl = imageUrl,
    //            FileName = imageUpload.FileName,
    //            FileType = imageUpload.ContentType,
    //            SizeInBytes = imageUpload.Length,
    //            CreatedTime = newDistrictMedia.CreatedTime,
    //            CreatedBy = newDistrictMedia.CreatedBy,
    //            LastUpdatedBy = newDistrictMedia.LastUpdatedBy
    //        };
    //    }
    //    catch (CustomException)
    //    {
    //        throw;
    //    }
    //    catch (Exception)
    //    {
    //        throw CustomExceptionFactory.CreateInternalServerError();
    //    }
    //    finally
    //    {
    //        ////  _unitOfWork.Dispose();
    //    }
    //}

    //public async Task<PagedResult<DistrictDataModel>> GetPagedDistrictsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var pagedResult = await _unitOfWork.DistrictRepository.GetPageAsync(pageNumber, pageSize);

    //        var districtDataModels = _mapper.Map<List<DistrictDataModel>>(pagedResult.Items);

    //        return new PagedResult<DistrictDataModel>
    //        {
    //            Items = districtDataModels,
    //            TotalCount = pagedResult.TotalCount,
    //            PageNumber = pageNumber,
    //            PageSize = pageSize
    //        };
    //    }
    //    catch (CustomException)
    //    {
    //        throw;
    //    }
    //    catch (Exception)
    //    {
    //        throw CustomExceptionFactory.CreateInternalServerError();
    //    }
    //    finally
    //    {
    //        ////  _unitOfWork.Dispose();
    //    }
    //}
}