using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.CuisineModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ICuisineService
{
    Task<CuisineDataModel?> GetCuisineByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<CuisineDataModel>> GetAllCuisinesAsync(CancellationToken cancellationToken);
    Task AddCuisineAsync(CuisineCreateModel cuisineCreateModel, CancellationToken cancellationToken);
    Task UpdateCuisineAsync(Guid id, CuisineUpdateModel cuisineUpdateModel, CancellationToken cancellationToken);
    Task DeleteCuisineAsync(Guid id, CancellationToken cancellationToken);
    Task<CuisineMediaResponse> AddCuisineWithMediaAsync(CuisineCreateWithMediaFileModel cuisineCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task UpdateCuisineAsync(Guid id, CuisineUpdateWithMediaFileModel cuisineUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task<PagedResult<CuisineDataModel>> GetPagedCuisinesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<CuisineDataModel>> GetPagedCuisinesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<CuisineMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
    Task<bool> DeleteMediaAsync(Guid id, List<string> deletedImages, CancellationToken cancellationToken);
}

public class CuisineService : ICuisineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;

    public CuisineService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        this._cloudinaryService = cloudinaryService;
    }

    public async Task AddCuisineAsync(CuisineCreateModel cuisineCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newCuisine = _mapper.Map<Cuisine>(cuisineCreateModel);
            newCuisine.CreatedBy = currentUserId;
            newCuisine.LastUpdatedBy = currentUserId;
            newCuisine.CreatedTime = currentTime;
            newCuisine.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.CuisineRepository.AddAsync(newCuisine);
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
           //  _unitOfWork.Dispose();
        }
    }

    public async Task DeleteCuisineAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id, cancellationToken);

            if (existingCuisine == null || existingCuisine.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("cuisine");
            }

            //var isInUsing = await _unitOfWork.LocationCraftVillageSuggestionRepository.ActiveEntities.FirstOrDefaultAsync(e => e.LocationId == id, cancellationToken) != null;

            // nếu đang được suggess thì xóa đi
            await _unitOfWork.LocationCuisineSuggestionRepository.ActiveEntities
                .Where(s => s.CuisineId == id)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            await _unitOfWork.CuisineMediaRepository.ActiveEntities
               .Where(s => s.CuisineId == id)
               .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            //if (isInUsing)
            //{
            //    throw CustomExceptionFactory.CreateBadRequest(ResponseMessages.BE_USED);
            //}

            existingCuisine.LastUpdatedBy = currentUserId;
            existingCuisine.DeletedBy = currentUserId;
            existingCuisine.DeletedTime = currentTime;
            existingCuisine.LastUpdatedTime = currentTime;
            existingCuisine.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.CuisineRepository.Update(existingCuisine);
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
           //  _unitOfWork.Dispose();
        }
    }

    public async Task<List<CuisineDataModel>> GetAllCuisinesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingCuisine = await _unitOfWork.CuisineRepository.GetAllAsync(cancellationToken);
            if (existingCuisine == null || existingCuisine.Count() == 0)
            {
                return new List<CuisineDataModel>();
            }

            var cuisineDataModels = _mapper.Map<List<CuisineDataModel>>(existingCuisine);

            foreach (var cuisineData in cuisineDataModels)
            {
                cuisineData.Medias = await GetMediaByIdAsync(cuisineData.Id, cancellationToken);
            }

            return cuisineDataModels;
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
           //  _unitOfWork.Dispose();
        }
    }

    public async Task<CuisineDataModel?> GetCuisineByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id, cancellationToken);
            if (existingCuisine == null || existingCuisine.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("cuisine");
            }

            var cuisineDataModel = _mapper.Map<CuisineDataModel>(existingCuisine);
            cuisineDataModel.Medias = await GetMediaByIdAsync(id, cancellationToken);

            return cuisineDataModel;
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
           //  _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<CuisineDataModel>> GetPagedCuisinesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.CuisineRepository.GetPageAsync(pageNumber, pageSize);

            var cuisineDataModels = _mapper.Map<List<CuisineDataModel>>(pagedResult.Items);

            foreach (var cuisine in cuisineDataModels)
            {
                cuisine.Medias = await GetMediaByIdAsync(cuisine.Id, cancellationToken);
            }

            return new PagedResult<CuisineDataModel>
            {
                Items = cuisineDataModels,
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
           //  _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<CuisineDataModel>> GetPagedCuisinesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.CuisineRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

            var cuisineDataModels = _mapper.Map<List<CuisineDataModel>>(pagedResult.Items);

            foreach (var cuisine in cuisineDataModels)
            {
                cuisine.Medias = await GetMediaByIdAsync(cuisine.Id, cancellationToken);
                cuisine.LocationName = await _unitOfWork.LocationRepository
                    .ActiveEntities
                    .Where(l => l.Id == cuisine.LocationId && !l.IsDeleted)
                    .Select(l => l.Name)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return new PagedResult<CuisineDataModel>
            {
                Items = cuisineDataModels,
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
           //  _unitOfWork.Dispose();
        }
    }

    public async Task UpdateCuisineAsync(Guid id, CuisineUpdateModel cuisineUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id, cancellationToken);
            if (existingCuisine == null || existingCuisine.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("cuisine");
            }

            _mapper.Map(cuisineUpdateModel, existingCuisine);

            existingCuisine.LastUpdatedBy = currentUserId;
            existingCuisine.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.CuisineRepository.Update(existingCuisine);
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
           //  _unitOfWork.Dispose();
        }
    }

    public async Task<CuisineMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id, cancellationToken);
            if (existingCuisine == null || existingCuisine.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("nhà hàng");
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

                var newCuisineMedia = new CuisineMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    CuisineId = existingCuisine.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.CuisineMediaRepository.AddAsync(newCuisineMedia);

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

            return new CuisineMediaResponse
            {
                CuisineId = existingCuisine.Id,
                CuisineName = existingCuisine.Name,
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

    // có add kèm theo ảnh
    public async Task<CuisineMediaResponse> AddCuisineWithMediaAsync(CuisineCreateWithMediaFileModel cuisineCreateModel, string? thumbnailSelected, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            //var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), cuisineCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
            //if (!checkRole)
            //{
            //    throw CustomExceptionFactory.CreateForbiddenError();
            //}

            var newCuisine = _mapper.Map<Cuisine>(cuisineCreateModel);
            newCuisine.CreatedBy = currentUserId;
            newCuisine.LastUpdatedBy = currentUserId;
            newCuisine.CreatedTime = currentTime;
            newCuisine.LastUpdatedTime = currentTime;

            await _unitOfWork.CuisineRepository.AddAsync(newCuisine);
            await _unitOfWork.SaveAsync();

            var mediaResponses = new List<MediaResponse>();

            if (cuisineCreateModel.ImageUploads != null && cuisineCreateModel.ImageUploads.Count > 0)
            {
                var imageUrls = await _cloudinaryService.UploadImagesAsync(cuisineCreateModel.ImageUploads);

                bool isAutoSelectThumbnail = string.IsNullOrEmpty(thumbnailSelected);
                bool thumbnailSet = false;

                for (int i = 0; i < cuisineCreateModel.ImageUploads.Count; i++)
                {
                    var imageFile = cuisineCreateModel.ImageUploads[i];
                    var imageUrl = imageUrls[i];

                    var newCuisineMedia = new CuisineMedia
                    {
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        CuisineId = newCuisine.Id,
                        MediaUrl = imageUrl,
                        SizeInBytes = imageFile.Length,
                        CreatedBy = currentUserId,
                        CreatedTime = _timeService.SystemTimeNow,
                        LastUpdatedBy = currentUserId,
                    };

                    // Chọn ảnh làm thumbnail
                    if ((isAutoSelectThumbnail && i == 0) || (!isAutoSelectThumbnail && imageFile.FileName == thumbnailSelected))
                    {
                        newCuisineMedia.IsThumbnail = true;
                        thumbnailSet = true;
                    }

                    await _unitOfWork.CuisineMediaRepository.AddAsync(newCuisineMedia);

                    mediaResponses.Add(new MediaResponse
                    {
                        MediaUrl = imageUrl,
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        SizeInBytes = imageFile.Length
                    });
                }

                // Trường hợp người dùng chọn ảnh thumbnail nhưng không tìm thấy ảnh khớp
                if (!thumbnailSet && cuisineCreateModel.ImageUploads.Count > 0)
                {
                    var firstMedia = mediaResponses.First();
                    var firstCuisineMedia = await _unitOfWork.CuisineMediaRepository
                        .GetFirstByCuisineIdAsync(newCuisine.Id);
                    if (firstCuisineMedia != null)
                    {
                        firstCuisineMedia.IsThumbnail = true;
                        _unitOfWork.CuisineMediaRepository.Update(firstCuisineMedia);
                    }
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new CuisineMediaResponse
            {
                CuisineId = newCuisine.Id,
                CuisineName = newCuisine.Name,
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
    public async Task UpdateCuisineAsync(
        Guid id,
        CuisineUpdateWithMediaFileModel cuisineUpdateModel,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            //var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), cuisineUpdateModel.DistrictId ?? Guid.Empty, cancellationToken);
            //if (!checkRole)
            //{
            //    throw CustomExceptionFactory.CreateForbiddenError();
            //}

            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id, cancellationToken);
            if (existingCuisine == null || existingCuisine.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("cuisine");
            }

            _mapper.Map(cuisineUpdateModel, existingCuisine);

            existingCuisine.LastUpdatedBy = currentUserId;
            existingCuisine.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.CuisineRepository.Update(existingCuisine);

            // xu ly anh
            var imageUploads = cuisineUpdateModel.ImageUploads;
            var allMedia = _unitOfWork.CuisineMediaRepository.Entities
                .Where(dm => dm.CuisineId == existingCuisine.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin cuisine
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
                    _unitOfWork.CuisineMediaRepository.Update(media);
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

                var newCuisineMedia = new CuisineMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    CuisineId = existingCuisine.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.CuisineMediaRepository.AddAsync(newCuisineMedia);
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
                        _unitOfWork.CuisineMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.CuisineMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.CuisineMediaRepository.Update(firstMediaEntity);
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

    private async Task<List<MediaResponse>> GetMediaByIdAsync(Guid cuisineId, CancellationToken cancellationToken)
    {
        try
        {
            var craftVillageMedias = await _unitOfWork.CuisineMediaRepository
                .ActiveEntities
                .Where(em => em.CuisineId == cuisineId)
                .ToListAsync(cancellationToken);

            return craftVillageMedias.Select(x => new MediaResponse
            {
                MediaUrl = x.MediaUrl,
                FileName = x.FileName ?? string.Empty,
                IsThumbnail = x.IsThumbnail,
                FileType = x.FileType,
                SizeInBytes = x.SizeInBytes,
                CreatedTime = x.CreatedTime
            }).ToList();
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

    public async Task<bool> DeleteMediaAsync(Guid id, List<string> deletedImages, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id, cancellationToken);
            if (existingCuisine == null || existingCuisine.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("cuisine");
            }

            if (deletedImages == null || deletedImages.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var imageUrlsDeleted = await _cloudinaryService.DeleteImagesAsync(deletedImages);

            if (!imageUrlsDeleted)
            {
                return false;
            }

            foreach (var imageUpload in deletedImages)
            {
                var cuisineMedia = await _unitOfWork.CuisineMediaRepository
                    .ActiveEntities
                    .FirstOrDefaultAsync(m => m.CuisineId == id && m.MediaUrl == imageUpload && !m.IsDeleted, cancellationToken);

                if (cuisineMedia != null)
                {
                    cuisineMedia.IsDeleted = true;
                    cuisineMedia.DeletedTime = DateTime.UtcNow;
                }
            }

            await _unitOfWork.SaveAsync();
            _unitOfWork.CommitTransaction();

            return true;
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

    //public async Task<CuisineDataModel?> GetCuisineByNameAsync(string name, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var existingCuisine = await _unitOfWork.CuisineRepository.GetByNameAsync(name, cancellationToken);
    //        if (existingCuisine == null || existingCuisine.IsDeleted)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("cuisine");
    //        }

    //        return _mapper.Map<CuisineDataModel>(existingCuisine);
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
    //       //  _unitOfWork.Dispose();
    //    }
    //}
}