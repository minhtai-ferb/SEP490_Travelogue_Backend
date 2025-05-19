using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.RestaurantModels;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IRestaurantService
{
    Task<RestaurantDataModel?> GetRestaurantByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<RestaurantDataModel>> GetAllRestaurantsAsync(CancellationToken cancellationToken);
    Task AddRestaurantAsync(RestaurantCreateModel restaurantCreateModel, CancellationToken cancellationToken);
    Task UpdateRestaurantAsync(Guid id, RestaurantUpdateModel restaurantUpdateModel, CancellationToken cancellationToken);
    Task DeleteRestaurantAsync(Guid id, CancellationToken cancellationToken);
    Task<RestaurantMediaResponse> AddRestaurantWithMediaAsync(RestaurantCreateWithMediaFileModel restaurantCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task UpdateRestaurantAsync(Guid id, RestaurantUpdateWithMediaFileModel restaurantUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task<PagedResult<RestaurantDataModel>> GetPagedRestaurantsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<RestaurantDataModel>> GetPagedRestaurantsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<RestaurantMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
    Task<bool> DeleteMediaAsync(Guid id, List<string> deletedImages, CancellationToken cancellationToken);
}

public class RestaurantService : IRestaurantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;

    public RestaurantService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        this._cloudinaryService = cloudinaryService;
    }

    public async Task AddRestaurantAsync(RestaurantCreateModel restaurantCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newRestaurant = _mapper.Map<Restaurant>(restaurantCreateModel);
            newRestaurant.CreatedBy = currentUserId;
            newRestaurant.LastUpdatedBy = currentUserId;
            newRestaurant.CreatedTime = currentTime;
            newRestaurant.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.RestaurantRepository.AddAsync(newRestaurant);
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

    public async Task DeleteRestaurantAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingRestaurant = await _unitOfWork.RestaurantRepository.GetByIdAsync(id, cancellationToken);

            if (existingRestaurant == null || existingRestaurant.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("restaurant");
            }

            //var isInUsing = await _unitOfWork.LocationHotelSuggestionRepository.ActiveEntities.FirstOrDefaultAsync(e => e.LocationId == id, cancellationToken) != null;

            // nếu đang được suggess thì xóa đi
            await _unitOfWork.LocationRestaurantSuggestionRepository.ActiveEntities
                .Where(s => s.RestaurantId == id)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            await _unitOfWork.RestaurantMediaRepository.ActiveEntities
               .Where(s => s.RestaurantId == id)
               .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            //if (isInUsing)
            //{
            //    throw CustomExceptionFactory.CreateBadRequest(ResponseMessages.BE_USED);
            //}

            existingRestaurant.LastUpdatedBy = currentUserId;
            existingRestaurant.DeletedBy = currentUserId;
            existingRestaurant.DeletedTime = currentTime;
            existingRestaurant.LastUpdatedTime = currentTime;
            existingRestaurant.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.RestaurantRepository.Update(existingRestaurant);
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

    public async Task<List<RestaurantDataModel>> GetAllRestaurantsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingRestaurant = await _unitOfWork.RestaurantRepository.GetAllAsync(cancellationToken);
            if (existingRestaurant == null || existingRestaurant.Count() == 0)
            {
                return new List<RestaurantDataModel>();
            }

            var restaurantDataModels = _mapper.Map<List<RestaurantDataModel>>(existingRestaurant);

            foreach (var restaurantData in restaurantDataModels)
            {
                restaurantData.Medias = await GetMediaByIdAsync(restaurantData.Id, cancellationToken);
            }

            return restaurantDataModels;
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

    public async Task<RestaurantDataModel?> GetRestaurantByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingRestaurant = await _unitOfWork.RestaurantRepository.GetByIdAsync(id, cancellationToken);
            if (existingRestaurant == null || existingRestaurant.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("restaurant");
            }

            var restaurantDataModel = _mapper.Map<RestaurantDataModel>(existingRestaurant);
            restaurantDataModel.Medias = await GetMediaByIdAsync(id, cancellationToken);

            return restaurantDataModel;
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

    public async Task<PagedResult<RestaurantDataModel>> GetPagedRestaurantsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.RestaurantRepository.GetPageAsync(pageNumber, pageSize);

            var restaurantDataModels = _mapper.Map<List<RestaurantDataModel>>(pagedResult.Items);

            foreach (var restaurant in restaurantDataModels)
            {
                restaurant.Medias = await GetMediaByIdAsync(restaurant.Id, cancellationToken);
            }

            return new PagedResult<RestaurantDataModel>
            {
                Items = restaurantDataModels,
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

    public async Task<PagedResult<RestaurantDataModel>> GetPagedRestaurantsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.RestaurantRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

            var restaurantDataModels = _mapper.Map<List<RestaurantDataModel>>(pagedResult.Items);

            foreach (var restaurant in restaurantDataModels)
            {
                restaurant.Medias = await GetMediaByIdAsync(restaurant.Id, cancellationToken);
                restaurant.LocationName = await _unitOfWork.LocationRepository
                    .ActiveEntities
                    .Where(l => l.Id == restaurant.LocationId && !l.IsDeleted)
                    .Select(l => l.Name)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return new PagedResult<RestaurantDataModel>
            {
                Items = restaurantDataModels,
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

    public async Task UpdateRestaurantAsync(Guid id, RestaurantUpdateModel restaurantUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingRestaurant = await _unitOfWork.RestaurantRepository.GetByIdAsync(id, cancellationToken);
            if (existingRestaurant == null || existingRestaurant.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("restaurant");
            }

            _mapper.Map(restaurantUpdateModel, existingRestaurant);

            existingRestaurant.LastUpdatedBy = currentUserId;
            existingRestaurant.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.RestaurantRepository.Update(existingRestaurant);
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

    public async Task<RestaurantMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingRestaurant = await _unitOfWork.RestaurantRepository.GetByIdAsync(id, cancellationToken);
            if (existingRestaurant == null || existingRestaurant.IsDeleted)
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

                var newRestaurantMedia = new RestaurantMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    RestaurantId = existingRestaurant.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.RestaurantMediaRepository.AddAsync(newRestaurantMedia);

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

            return new RestaurantMediaResponse
            {
                RestaurantId = existingRestaurant.Id,
                RestaurantName = existingRestaurant.Name,
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
    public async Task<RestaurantMediaResponse> AddRestaurantWithMediaAsync(RestaurantCreateWithMediaFileModel restaurantCreateModel, string? thumbnailSelected, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            //var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), restaurantCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
            //if (!checkRole)
            //{
            //    throw CustomExceptionFactory.CreateForbiddenError();
            //}

            var newRestaurant = _mapper.Map<Restaurant>(restaurantCreateModel);
            newRestaurant.CreatedBy = currentUserId;
            newRestaurant.LastUpdatedBy = currentUserId;
            newRestaurant.CreatedTime = currentTime;
            newRestaurant.LastUpdatedTime = currentTime;

            await _unitOfWork.RestaurantRepository.AddAsync(newRestaurant);
            await _unitOfWork.SaveAsync();

            var mediaResponses = new List<MediaResponse>();

            if (restaurantCreateModel.ImageUploads != null && restaurantCreateModel.ImageUploads.Count > 0)
            {
                var imageUrls = await _cloudinaryService.UploadImagesAsync(restaurantCreateModel.ImageUploads);

                bool isAutoSelectThumbnail = string.IsNullOrEmpty(thumbnailSelected);
                bool thumbnailSet = false;

                for (int i = 0; i < restaurantCreateModel.ImageUploads.Count; i++)
                {
                    var imageFile = restaurantCreateModel.ImageUploads[i];
                    var imageUrl = imageUrls[i];

                    var newRestaurantMedia = new RestaurantMedia
                    {
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        RestaurantId = newRestaurant.Id,
                        MediaUrl = imageUrl,
                        SizeInBytes = imageFile.Length,
                        CreatedBy = currentUserId,
                        CreatedTime = _timeService.SystemTimeNow,
                        LastUpdatedBy = currentUserId,
                    };

                    // Chọn ảnh làm thumbnail
                    if ((isAutoSelectThumbnail && i == 0) || (!isAutoSelectThumbnail && imageFile.FileName == thumbnailSelected))
                    {
                        newRestaurantMedia.IsThumbnail = true;
                        thumbnailSet = true;
                    }

                    await _unitOfWork.RestaurantMediaRepository.AddAsync(newRestaurantMedia);

                    mediaResponses.Add(new MediaResponse
                    {
                        MediaUrl = imageUrl,
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        SizeInBytes = imageFile.Length
                    });
                }

                // Trường hợp người dùng chọn ảnh thumbnail nhưng không tìm thấy ảnh khớp
                if (!thumbnailSet && restaurantCreateModel.ImageUploads.Count > 0)
                {
                    var firstMedia = mediaResponses.First();
                    var firstRestaurantMedia = await _unitOfWork.RestaurantMediaRepository
                        .GetFirstByRestaurantIdAsync(newRestaurant.Id);
                    if (firstRestaurantMedia != null)
                    {
                        firstRestaurantMedia.IsThumbnail = true;
                        _unitOfWork.RestaurantMediaRepository.Update(firstRestaurantMedia);
                    }
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new RestaurantMediaResponse
            {
                RestaurantId = newRestaurant.Id,
                RestaurantName = newRestaurant.Name,
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
    public async Task UpdateRestaurantAsync(
        Guid id,
        RestaurantUpdateWithMediaFileModel restaurantUpdateModel,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            //var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), restaurantUpdateModel.DistrictId ?? Guid.Empty, cancellationToken);
            //if (!checkRole)
            //{
            //    throw CustomExceptionFactory.CreateForbiddenError();
            //}

            var existingRestaurant = await _unitOfWork.RestaurantRepository.GetByIdAsync(id, cancellationToken);
            if (existingRestaurant == null || existingRestaurant.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("restaurant");
            }

            _mapper.Map(restaurantUpdateModel, existingRestaurant);

            existingRestaurant.LastUpdatedBy = currentUserId;
            existingRestaurant.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.RestaurantRepository.Update(existingRestaurant);

            // xu ly anh
            var imageUploads = restaurantUpdateModel.ImageUploads;
            var allMedia = _unitOfWork.RestaurantMediaRepository.Entities
                .Where(dm => dm.RestaurantId == existingRestaurant.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin restaurant
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
                    _unitOfWork.RestaurantMediaRepository.Update(media);
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

                var newRestaurantMedia = new RestaurantMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    RestaurantId = existingRestaurant.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.RestaurantMediaRepository.AddAsync(newRestaurantMedia);
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
                        _unitOfWork.RestaurantMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.RestaurantMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.RestaurantMediaRepository.Update(firstMediaEntity);
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

    private async Task<List<MediaResponse>> GetMediaByIdAsync(Guid restaurantId, CancellationToken cancellationToken)
    {
        try
        {
            var hotelMedias = await _unitOfWork.RestaurantMediaRepository
                .ActiveEntities
                .Where(em => em.RestaurantId == restaurantId)
                .ToListAsync(cancellationToken);

            return hotelMedias.Select(x => new MediaResponse
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
            var existingRestaurant = await _unitOfWork.RestaurantRepository.GetByIdAsync(id, cancellationToken);
            if (existingRestaurant == null || existingRestaurant.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("restaurant");
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
                var restaurantMedia = await _unitOfWork.RestaurantMediaRepository
                    .ActiveEntities
                    .FirstOrDefaultAsync(m => m.RestaurantId == id && m.MediaUrl == imageUpload && !m.IsDeleted, cancellationToken);

                if (restaurantMedia != null)
                {
                    restaurantMedia.IsDeleted = true;
                    restaurantMedia.DeletedTime = DateTime.UtcNow;
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

    //public async Task<RestaurantDataModel?> GetRestaurantByNameAsync(string name, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var existingRestaurant = await _unitOfWork.RestaurantRepository.GetByNameAsync(name, cancellationToken);
    //        if (existingRestaurant == null || existingRestaurant.IsDeleted)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("restaurant");
    //        }

    //        return _mapper.Map<RestaurantDataModel>(existingRestaurant);
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