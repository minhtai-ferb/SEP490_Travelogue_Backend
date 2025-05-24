using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.ExperienceModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IExperienceService
{
    Task<ExperienceDataModel?> GetExperienceByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<ExperienceDataModel>> GetAllExperiencesAsync(CancellationToken cancellationToken);
    Task<ExperienceDataModel> AddExperienceAsync(ExperienceCreateModel experienceCreateModel, CancellationToken cancellationToken);
    Task UpdateExperienceAsync(Guid id, ExperienceUpdateModel experienceUpdateModel, CancellationToken cancellationToken);
    Task DeleteExperienceAsync(Guid id, CancellationToken cancellationToken);
    Task<ExperienceMediaResponse> AddExperienceWithMediaAsync(ExperienceCreateWithMediaFileModel experienceCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task UpdateExperienceAsync(Guid id, ExperienceUpdateWithMediaFileModel experienceUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);

    //Task<PagedResult<ExperienceDataModel>> GetPagedExperiencesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    //Task<PagedResult<ExperienceDataModel>> GetPagedExperiencesWithSearchAsync(int pageNumber, int pageSize, string title, CancellationToken cancellationToken);
    Task<PagedResult<ExperienceDataModel>> GetPagedExperiencesWithSearchAsync(string? title, Guid? typeExperienceId, Guid? locationId, Guid? experienceId, Guid? districtId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<ExperienceMediaResponse> UploadMediaAsync(
        Guid id,
        List<IFormFile>? imageUploads,
        string? thumbnailSelected,
        CancellationToken cancellationToken);
    Task<List<ExperienceDataModel>> GetAllExperienceAdminAsync();
}

public class ExperienceService : IExperienceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;

    public ExperienceService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ExperienceDataModel> AddExperienceAsync(ExperienceCreateModel experienceCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), experienceCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var newExperience = _mapper.Map<Experience>(experienceCreateModel);
            newExperience.CreatedBy = currentUserId;
            newExperience.LastUpdatedBy = currentUserId;
            newExperience.CreatedTime = currentTime;
            newExperience.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.ExperienceRepository.AddAsync(newExperience);
            _unitOfWork.CommitTransaction();
            var result = _unitOfWork.ExperienceRepository.ActiveEntities
    .FirstOrDefault(l => l.Id == newExperience.Id);

            return _mapper.Map<ExperienceDataModel>(result);
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
            _unitOfWork.Dispose();
        }
    }

    public async Task DeleteExperienceAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingExperience = await _unitOfWork.ExperienceRepository.GetByIdAsync(id, cancellationToken);

            if (existingExperience == null || existingExperience.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("experience");
            }

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), existingExperience.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            await _unitOfWork.ExperienceMediaRepository.ActiveEntities
                .Where(s => s.ExperienceId == id)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            existingExperience.LastUpdatedBy = currentUserId;
            existingExperience.DeletedBy = currentUserId;
            existingExperience.DeletedTime = currentTime;
            existingExperience.LastUpdatedTime = currentTime;
            existingExperience.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.ExperienceRepository.Update(existingExperience);
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
            _unitOfWork.Dispose();
        }
    }

    public async Task<List<ExperienceDataModel>> GetAllExperiencesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingExperience = await _unitOfWork.ExperienceRepository.GetAllAsync(cancellationToken);
            if (existingExperience == null || !existingExperience.Any())
            {
                return new List<ExperienceDataModel>();
            }

            var experienceDataModels = _mapper.Map<List<ExperienceDataModel>>(existingExperience);

            foreach (var experienceData in experienceDataModels)
            {
                experienceData.Medias = await GetMediaByIdAsync(experienceData.Id, cancellationToken);
                experienceData.LocationName = await _unitOfWork.LocationRepository.ActiveEntities
                    .Where(l => l.Id == experienceData.LocationId)
                    .Select(l => l.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                experienceData.EventName = await _unitOfWork.EventRepository.ActiveEntities
                    .Where(l => l.Id == experienceData.EventId)
                    .Select(l => l.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                experienceData.DistrictName = await _unitOfWork.DistrictRepository.ActiveEntities
                    .Where(l => l.Id == experienceData.DistrictId)
                    .Select(l => l.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                experienceData.TypeExperienceName = await _unitOfWork.TypeEventRepository.ActiveEntities
                    .Where(l => l.Id == experienceData.TypeExperienceId)
                    .Select(l => l.TypeName)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return experienceDataModels;
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
            _unitOfWork.Dispose();
        }
    }

    public async Task<ExperienceDataModel?> GetExperienceByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            //var existingExperience = await _unitOfWork.ExperienceRepository.GetByIdAsync(id, cancellationToken);
            var existingExperience = await _unitOfWork.ExperienceRepository
                .ActiveEntities
                .Include(e => e.Location)
                .Include(e => e.Event)
                .Include(e => e.District)
                .Include(e => e.TypeExperience)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (existingExperience == null || existingExperience.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("experience");
            }

            var experienceDataModel = _mapper.Map<ExperienceDataModel>(existingExperience);
            experienceDataModel.Medias = await GetMediaByIdAsync(id, cancellationToken);
            experienceDataModel.LocationName = existingExperience.Location?.Name;
            experienceDataModel.LocationName = existingExperience.Location?.Name;
            experienceDataModel.LocationName = existingExperience.Location?.Name;

            return experienceDataModel;
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
            _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<ExperienceDataModel>> GetPagedExperiencesWithSearchAsync(string? title, Guid? typeExperienceId, Guid? locationId, Guid? experienceId, Guid? districtId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.ExperienceRepository.GetPageWithSearchAsync(title, typeExperienceId, locationId, experienceId, districtId, pageNumber, pageSize, cancellationToken);

            var experienceDataModels = _mapper.Map<List<ExperienceDataModel>>(pagedResult.Items);

            foreach (var experience in experienceDataModels)
            {
                experience.Medias = await GetMediaByIdAsync(experience.Id, cancellationToken);
                experience.EventName = await _unitOfWork.EventRepository.ActiveEntities
                    .Where(l => l.Id == experience.EventId)
                    .Select(l => l.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                experience.DistrictName = await _unitOfWork.DistrictRepository.ActiveEntities
                    .Where(l => l.Id == experience.DistrictId)
                    .Select(l => l.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                experience.TypeExperienceName = await _unitOfWork.TypeExperienceRepository.ActiveEntities
                    .Where(l => l.Id == experience.TypeExperienceId)
                    .Select(l => l.TypeName)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return new PagedResult<ExperienceDataModel>
            {
                Items = experienceDataModels,
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
            _unitOfWork.Dispose();
        }
    }

    public async Task UpdateExperienceAsync(Guid id, ExperienceUpdateModel experienceUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingExperience = await _unitOfWork.ExperienceRepository.GetByIdAsync(id, cancellationToken);
            if (existingExperience == null || existingExperience.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("experience");
            }

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), existingExperience.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            _mapper.Map(experienceUpdateModel, existingExperience);

            existingExperience.LastUpdatedBy = currentUserId;
            existingExperience.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.ExperienceRepository.Update(existingExperience);
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
            _unitOfWork.Dispose();
        }
    }

    public async Task<ExperienceMediaResponse> UploadMediaAsync(
        Guid id,
        List<IFormFile>? imageUploads,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingExperience = await _unitOfWork.ExperienceRepository.GetByIdAsync(id, cancellationToken);
            if (existingExperience == null || existingExperience.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("experience");
            }

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), id, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            if (imageUploads == null || imageUploads.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var allMedia = _unitOfWork.ExperienceMediaRepository.Entities
                .Where(dm => dm.ExperienceId == existingExperience.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin experience
            if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return new ExperienceMediaResponse
                {
                    ExperienceId = existingExperience.Id,
                    Title = existingExperience.Title,
                    Media = new List<MediaResponse>()
                };
            }

            bool isThumbnailUpdated = false;

            // Nếu có thumbnailSelected và nó là link (ảnh cũ) -> Cập nhật ảnh cũ làm thumbnail
            if (!string.IsNullOrEmpty(thumbnailSelected) && Helper.IsValidUrl(thumbnailSelected))
            {
                foreach (var media in allMedia)
                {
                    media.IsThumbnail = media.MediaUrl == thumbnailSelected;
                    _unitOfWork.ExperienceMediaRepository.Update(media);
                }
                isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
            }

            // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
            if (imageUploads == null || imageUploads.Count == 0)
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return new ExperienceMediaResponse
                {
                    ExperienceId = existingExperience.Id,
                    Title = existingExperience.Title,
                    Media = new List<MediaResponse>()
                };
            }

            // Có ảnh mới -> Upload lên Cloudinary
            var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
            var mediaResponses = new List<MediaResponse>();

            for (int i = 0; i < imageUploads.Count; i++)
            {
                var imageUpload = imageUploads[i];
                bool isThumbnail = false;

                // Nếu thumbnailSelected là tên file -> Đặt ảnh mới làm thumbnail
                if (!string.IsNullOrEmpty(thumbnailSelected) && !Helper.IsValidUrl(thumbnailSelected))
                {
                    isThumbnail = imageUpload.FileName == thumbnailSelected;
                }

                var newExperienceMedia = new ExperienceMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    ExperienceId = existingExperience.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.ExperienceMediaRepository.AddAsync(newExperienceMedia);
                mediaResponses.Add(new MediaResponse
                {
                    MediaUrl = imageUrls[i],
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    IsThumbnail = isThumbnail,
                    SizeInBytes = imageUpload.Length
                });

                // Nếu ảnh mới được chọn làm thumbnail -> Cập nhật tất cả ảnh cũ về IsThumbnail = false
                if (isThumbnail)
                {
                    foreach (var media in allMedia)
                    {
                        media.IsThumbnail = false;
                        _unitOfWork.ExperienceMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.ExperienceMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.ExperienceMediaRepository.Update(firstMediaEntity);
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new ExperienceMediaResponse
            {
                ExperienceId = existingExperience.Id,
                Title = existingExperience.Title,
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

    // có add ảnh kèm theo
    public async Task<ExperienceMediaResponse> AddExperienceWithMediaAsync(ExperienceCreateWithMediaFileModel experienceCreateModel, string? thumbnailSelected, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), experienceCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var newExperience = _mapper.Map<Experience>(experienceCreateModel);
            newExperience.CreatedBy = currentUserId;
            newExperience.LastUpdatedBy = currentUserId;
            newExperience.CreatedTime = currentTime;
            newExperience.LastUpdatedTime = currentTime;

            await _unitOfWork.ExperienceRepository.AddAsync(newExperience);
            await _unitOfWork.SaveAsync();

            var mediaResponses = new List<MediaResponse>();

            if (experienceCreateModel.ImageUploads != null && experienceCreateModel.ImageUploads.Count > 0)
            {
                var imageUrls = await _cloudinaryService.UploadImagesAsync(experienceCreateModel.ImageUploads);

                bool isAutoSelectThumbnail = string.IsNullOrEmpty(thumbnailSelected);
                bool thumbnailSet = false;

                for (int i = 0; i < experienceCreateModel.ImageUploads.Count; i++)
                {
                    var imageFile = experienceCreateModel.ImageUploads[i];
                    var imageUrl = imageUrls[i];

                    var newExperienceMedia = new ExperienceMedia
                    {
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        ExperienceId = newExperience.Id,
                        MediaUrl = imageUrl,
                        SizeInBytes = imageFile.Length,
                        CreatedBy = currentUserId,
                        CreatedTime = _timeService.SystemTimeNow,
                        LastUpdatedBy = currentUserId,
                    };

                    // Chọn ảnh làm thumbnail
                    if ((isAutoSelectThumbnail && i == 0) || (!isAutoSelectThumbnail && imageFile.FileName == thumbnailSelected))
                    {
                        newExperienceMedia.IsThumbnail = true;
                        thumbnailSet = true;
                    }

                    await _unitOfWork.ExperienceMediaRepository.AddAsync(newExperienceMedia);

                    mediaResponses.Add(new MediaResponse
                    {
                        MediaUrl = imageUrl,
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        SizeInBytes = imageFile.Length
                    });
                }

                // Trường hợp người dùng chọn ảnh thumbnail nhưng không tìm thấy ảnh khớp
                if (!thumbnailSet && experienceCreateModel.ImageUploads.Count > 0)
                {
                    var firstMedia = mediaResponses.First();
                    var firstExperienceMedia = await _unitOfWork.ExperienceMediaRepository
                        .GetFirstByExperienceIdAsync(newExperience.Id);
                    if (firstExperienceMedia != null)
                    {
                        firstExperienceMedia.IsThumbnail = true;
                        _unitOfWork.ExperienceMediaRepository.Update(firstExperienceMedia);
                    }
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new ExperienceMediaResponse
            {
                ExperienceId = newExperience.Id,
                Title = newExperience.Title,
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

    // có update ảnh kèm theo
    public async Task UpdateExperienceAsync(
        Guid id,
        ExperienceUpdateWithMediaFileModel experienceUpdateModel,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), experienceUpdateModel.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var existingExperience = await _unitOfWork.ExperienceRepository.GetByIdAsync(id, cancellationToken);
            if (existingExperience == null || existingExperience.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("experience");
            }

            _mapper.Map(experienceUpdateModel, existingExperience);

            existingExperience.LastUpdatedBy = currentUserId;
            existingExperience.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.ExperienceRepository.Update(existingExperience);

            // xu ly anh
            var imageUploads = experienceUpdateModel.ImageUploads;
            var allMedia = _unitOfWork.ExperienceMediaRepository.ActiveEntities
                .Where(dm => dm.ExperienceId == existingExperience.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin experience
            if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            bool isThumbnailUpdated = false;

            // Nếu có thumbnailSelected và nó là link (ảnh cũ) -> Cập nhật ảnh cũ làm thumbnail
            if (!string.IsNullOrEmpty(thumbnailSelected) && Helper.IsValidUrl(thumbnailSelected))
            {
                foreach (var media in allMedia)
                {
                    media.IsThumbnail = media.MediaUrl == thumbnailSelected;
                    _unitOfWork.ExperienceMediaRepository.Update(media);
                }
                isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
            }

            // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
            if (imageUploads == null || imageUploads.Count == 0)
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            // Có ảnh mới -> Upload lên Cloudinary
            var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
            var mediaResponses = new List<MediaResponse>();

            for (int i = 0; i < imageUploads.Count; i++)
            {
                var imageUpload = imageUploads[i];
                bool isThumbnail = false;

                // Nếu thumbnailSelected là tên file -> Đặt ảnh mới làm thumbnail
                if (!string.IsNullOrEmpty(thumbnailSelected) && !Helper.IsValidUrl(thumbnailSelected))
                {
                    isThumbnail = imageUpload.FileName == thumbnailSelected;
                }

                var newExperienceMedia = new ExperienceMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    ExperienceId = existingExperience.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.ExperienceMediaRepository.AddAsync(newExperienceMedia);
                mediaResponses.Add(new MediaResponse
                {
                    MediaUrl = imageUrls[i],
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    IsThumbnail = isThumbnail,
                    SizeInBytes = imageUpload.Length
                });

                // Nếu ảnh mới được chọn làm thumbnail -> Cập nhật tất cả ảnh cũ về IsThumbnail = false
                if (isThumbnail)
                {
                    foreach (var media in allMedia)
                    {
                        media.IsThumbnail = false;
                        _unitOfWork.ExperienceMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.ExperienceMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.ExperienceMediaRepository.Update(firstMediaEntity);
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

    public async Task<List<ExperienceDataModel>> GetAllExperienceAdminAsync()
    {
        try
        {
            // 1. Lấy userId nếu có
            var currentUserId = _userContextService.TryGetCurrentUserId();

            // 2. Nếu không có userId → là khách → trả toàn bộ
            if (string.IsNullOrEmpty(currentUserId))
            {
                var allExperiences = await _unitOfWork.ExperienceRepository.GetAllAsync();
                var allExperienceDataModels = _mapper.Map<List<ExperienceDataModel>>(allExperiences);
                await EnrichExperienceDataModelsAsync(allExperienceDataModels, new CancellationToken());
                return allExperienceDataModels;
            }

            // 3. Nếu có userId → tìm user
            var userId = Guid.Parse(currentUserId);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, CancellationToken.None);

            // 4. Nếu không tìm được user (trường hợp hiếm) → fallback: trả toàn bộ
            if (user == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("user");
            }

            // 5. Lấy role
            var roles = await _unitOfWork.UserRepository.GetRolesByUserIdAsync(userId);
            var roleNames = roles.Select(r => r.Name).ToList();
            var roleIds = roles.Select(r => r.Id).ToList();

            List<Experience> experiences;

            // 6. Admin toàn quyền
            if (roleNames.Equals(AppRole.ADMIN))
            {
                experiences = (await _unitOfWork.ExperienceRepository.GetAllAsync()).ToList();
            }
            // 7. Admin huyện (dựa vào RoleDistrict)
            else
            {
                // Lấy các DistrictId mà user được phân quyền quản lý
                var allowedDistrictIds = await _unitOfWork.RoleDistrictRepository.ActiveEntities
                    .Where(rd => roleIds.Contains(rd.RoleId))
                    .Select(rd => rd.DistrictId)
                    .Distinct()
                    .ToListAsync();

                if (allowedDistrictIds.Any())
                {
                    // Là admin huyện → chỉ được lấy các Experience trong danh sách huyện đó
                    experiences = await _unitOfWork.ExperienceRepository.ActiveEntities
                        .Where(l => l.DistrictId.HasValue && allowedDistrictIds.Contains(l.DistrictId.Value))
                        .ToListAsync();
                }
                else
                {
                    // Không có quyền theo huyện nào → xem là người dùng thường
                    experiences = (await _unitOfWork.ExperienceRepository.GetAllAsync()).ToList();
                }
            }

            var experienceDataModels = _mapper.Map<List<ExperienceDataModel>>(experiences);
            await EnrichExperienceDataModelsAsync(experienceDataModels, new CancellationToken());
            return experienceDataModels;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    private async Task EnrichExperienceDataModelsAsync(List<ExperienceDataModel> experienceDataModels, CancellationToken cancellationToken)
    {
        var experienceIds = experienceDataModels.Select(x => x.Id).ToList();
        var districtIds = experienceDataModels.Where(x => x.DistrictId.HasValue).Select(x => x.DistrictId.Value).Distinct().ToList();
        var typeExperienceIds = experienceDataModels.Where(x => x.TypeExperienceId.HasValue).Select(x => x.TypeExperienceId.Value).Distinct().ToList();

        var allMedias = await _unitOfWork.ExperienceMediaRepository
            .ActiveEntities
            .Where(m => experienceIds.Contains(m.ExperienceId) && !m.FileType.Contains("video"))
            .ToListAsync(cancellationToken);

        var districtNames = await _unitOfWork.DistrictRepository
            .ActiveEntities
            .Where(d => districtIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        var typeExperienceNames = await _unitOfWork.TypeExperienceRepository
            .ActiveEntities
            .Where(t => typeExperienceIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.TypeName, cancellationToken);

        foreach (var experienceModel in experienceDataModels)
        {
            experienceModel.Medias = _mapper.Map<List<MediaResponse>>(allMedias
                .Where(m => m.ExperienceId == experienceModel.Id)
                .ToList());

            if (experienceModel.DistrictId.HasValue && districtNames.TryGetValue(experienceModel.DistrictId.Value, out var districtName))
            {
                experienceModel.DistrictName = districtName;
            }

            if (experienceModel.TypeExperienceId.HasValue && typeExperienceNames.TryGetValue(experienceModel.TypeExperienceId.Value, out var typeExperienceName))
            {
                experienceModel.TypeExperienceName = typeExperienceName;
            }
        }
    }

    private async Task<List<MediaResponse>> GetMediaByIdAsync(Guid experienceId, CancellationToken cancellationToken)
    {
        var experienceMedias = await _unitOfWork.ExperienceMediaRepository
            .ActiveEntities
            .Where(em => em.ExperienceId == experienceId)
            .ToListAsync(cancellationToken);

        return experienceMedias.Select(x => new MediaResponse
        {
            MediaUrl = x.MediaUrl,
            FileName = x.FileName ?? string.Empty,
            IsThumbnail = x.IsThumbnail,
            FileType = x.FileType,
            SizeInBytes = x.SizeInBytes,
            CreatedTime = x.CreatedTime
        }).ToList();
    }

    public async Task<ExperienceMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingExperience = await _unitOfWork.ExperienceRepository.GetByIdAsync(id, cancellationToken);
            if (existingExperience == null || existingExperience.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("khách sạn");
            }

            if (imageUploads == null || !imageUploads.Any())
            {
                throw CustomExceptionFactory.CreateNotFoundError("medias");
            }

            var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
            var mediaResponses = new List<MediaResponse>();

            for (int i = 0; i < imageUploads.Count; i++)
            {
                var imageUpload = imageUploads[i];

                var newExperienceMedia = new ExperienceMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    ExperienceId = existingExperience.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.ExperienceMediaRepository.AddAsync(newExperienceMedia);

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

            return new ExperienceMediaResponse
            {
                ExperienceId = existingExperience.Id,
                Title = existingExperience.Title,
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

    //public async Task<ExperienceDataModel?> GetExperienceByTitleAsync(string title, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var existingExperience = await _unitOfWork.ExperienceRepository.GetByTitleAsync(title, cancellationToken);
    //        if (existingExperience == null || existingExperience.IsDeleted)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("experience");
    //        }

    //        return _mapper.Map<ExperienceDataModel>(existingExperience);
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
    //        _unitOfWork.Dispose();
    //    }
    //}

    //public async Task<PagedResult<ExperienceDataModel>> GetPagedExperiencesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var pagedResult = await _unitOfWork.ExperienceRepository.GetPageAsync(pageNumber, pageSize);

    //        var experienceDataModels = _mapper.Map<List<ExperienceDataModel>>(pagedResult.Items);

    //        return new PagedResult<ExperienceDataModel>
    //        {
    //            Items = experienceDataModels,
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
    //        _unitOfWork.Dispose();
    //    }
    //}

    //public async Task<PagedResult<ExperienceDataModel>> GetPagedExperiencesWithSearchAsync(int pageNumber, int pageSize, string title, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var pagedResult = await _unitOfWork.ExperienceRepository.GetPageWithSearchAsync(pageNumber, pageSize, title, cancellationToken);

    //        var experienceDataModels = _mapper.Map<List<ExperienceDataModel>>(pagedResult.Items);

    //        return new PagedResult<ExperienceDataModel>
    //        {
    //            Items = experienceDataModels,
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
    //        _unitOfWork.Dispose();
    //    }
    //}
}