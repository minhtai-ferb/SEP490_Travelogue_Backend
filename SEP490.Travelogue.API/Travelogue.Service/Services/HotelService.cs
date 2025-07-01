using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.HotelModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IHotelService
{
    Task<HotelDetailDataModel?> GetHotelByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<HotelDetailDataModel>> GetAllHotelsAsync(CancellationToken cancellationToken);
    Task AddHotelAsync(HotelCreateModel hotelCreateModel, CancellationToken cancellationToken);
    Task UpdateHotelAsync(Guid id, HotelUpdateModel hotelUpdateModel, CancellationToken cancellationToken);
    Task DeleteHotelAsync(Guid id, CancellationToken cancellationToken);
    // Task<HotelMediaResponse> AddHotelWithMediaAsync(HotelCreateWithMediaFileModel hotelCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    // Task UpdateHotelAsync(Guid id, HotelUpdateWithMediaFileModel hotelUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task<PagedResult<HotelDetailDataModel>> GetPagedHotelsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<HotelDetailDataModel>> GetPagedHotelsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    // Task<HotelMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
    // Task<HotelMediaResponse> UploadMediaAsync(
    //     Guid id,
    //     List<IFormFile>? imageUploads,
    //     string? thumbnailSelected,
    //     CancellationToken cancellationToken);
}

public class HotelService : IHotelService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;

    public HotelService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task AddHotelAsync(HotelCreateModel hotelCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var locationHotel = _mapper.Map<Location>(hotelCreateModel);
            locationHotel.CreatedBy = currentUserId;
            locationHotel.LastUpdatedBy = currentUserId;
            locationHotel.CreatedTime = currentTime;
            locationHotel.LastUpdatedTime = currentTime;
            await _unitOfWork.LocationRepository.AddAsync(locationHotel);

            var newHotel = _mapper.Map<Hotel>(hotelCreateModel);
            newHotel.CreatedBy = currentUserId;
            newHotel.LastUpdatedBy = currentUserId;
            newHotel.CreatedTime = currentTime;
            newHotel.LastUpdatedTime = currentTime;
            newHotel.LocationId = locationHotel.Id;
            await _unitOfWork.HotelRepository.AddAsync(newHotel);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
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

    public async Task DeleteHotelAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingHotel = await _unitOfWork.HotelRepository.GetByIdAsync(id, cancellationToken);

            if (existingHotel == null || existingHotel.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("hotel");
            }

            //var isInUsing = await _unitOfWork.LocationHotelSuggestionRepository.ActiveEntities.FirstOrDefaultAsync(e => e.LocationId == id, cancellationToken) != null;

            // nếu đang được suggess thì xóa đi
            await _unitOfWork.LocationHotelSuggestionRepository.ActiveEntities
                .Where(s => s.HotelId == id)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            await _unitOfWork.LocationMediaRepository.ActiveEntities
                .Where(s => s.LocationId == existingHotel.LocationId)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            //if (isInUsing)
            //{
            //    throw CustomExceptionFactory.CreateBadRequest(ResponseMessages.BE_USED);
            //}

            existingHotel.LastUpdatedBy = currentUserId;
            existingHotel.DeletedBy = currentUserId;
            existingHotel.DeletedTime = currentTime;
            existingHotel.LastUpdatedTime = currentTime;
            existingHotel.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.HotelRepository.Update(existingHotel);
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

    public async Task<List<HotelDetailDataModel>> GetAllHotelsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingHotel = await _unitOfWork.HotelRepository.GetAllAsync(cancellationToken);
            if (existingHotel == null || !existingHotel.Any())
            {
                return new List<HotelDetailDataModel>();
            }

            var hotelDataModels = _mapper.Map<List<HotelDetailDataModel>>(existingHotel);

            foreach (var hotelData in hotelDataModels)
            {
                hotelData.Medias = await GetMediaByIdAsync(hotelData.Id, cancellationToken);
                var locationHotel = _unitOfWork.LocationRepository
                    .ActiveEntities
                    .FirstOrDefault(l => l.Id == hotelData.LocationId);
                if (locationHotel != null)
                {
                    hotelData.Address = locationHotel.Address;
                    hotelData.Latitude = locationHotel.Latitude;
                    hotelData.Longitude = locationHotel.Longitude;
                    hotelData.LocationId = locationHotel.Id;
                    hotelData.Name = locationHotel.Name;
                    hotelData.Description = locationHotel.Description;
                    hotelData.Content = locationHotel.Content;
                    hotelData.OpenTime = locationHotel.OpenTime;
                    hotelData.CloseTime = locationHotel.CloseTime;
                }
            }

            return hotelDataModels;
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

    public async Task<HotelDetailDataModel?> GetHotelByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingHotel = await _unitOfWork.HotelRepository.GetByIdAsync(id, cancellationToken);
            if (existingHotel == null || existingHotel.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("hotel");
            }

            var hotelDataModel = _mapper.Map<HotelDetailDataModel>(existingHotel);
            hotelDataModel.Medias = await GetMediaByIdAsync(id, cancellationToken);

            return hotelDataModel;
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

    public async Task<PagedResult<HotelDetailDataModel>> GetPagedHotelsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.HotelRepository.GetPageAsync(pageNumber, pageSize);

            var hotelDataModels = _mapper.Map<List<HotelDetailDataModel>>(pagedResult.Items);

            foreach (var hotel in hotelDataModels)
            {
                hotel.Medias = await GetMediaByIdAsync(hotel.Id, cancellationToken);
            }

            return new PagedResult<HotelDetailDataModel>
            {
                Items = hotelDataModels,
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

    public async Task<PagedResult<HotelDetailDataModel>> GetPagedHotelsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.HotelRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

            var hotelDataModels = _mapper.Map<List<HotelDetailDataModel>>(pagedResult.Items);

            foreach (var hotel in hotelDataModels)
            {
                hotel.Medias = await GetMediaByIdAsync(hotel.Id, cancellationToken);
            }

            return new PagedResult<HotelDetailDataModel>
            {
                Items = hotelDataModels,
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

    public async Task UpdateHotelAsync(Guid id, HotelUpdateModel hotelUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingHotel = await _unitOfWork.HotelRepository.GetByIdAsync(id, cancellationToken);
            if (existingHotel == null || existingHotel.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("hotel");
            }

            _mapper.Map(hotelUpdateModel, existingHotel);

            existingHotel.LastUpdatedBy = currentUserId;
            existingHotel.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.HotelRepository.Update(existingHotel);
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

    // public async Task<HotelMediaResponse> UploadMediaAsync(
    //     Guid id,
    //     List<IFormFile>? imageUploads,
    //     string? thumbnailSelected,
    //     CancellationToken cancellationToken)
    // {
    //     using var transaction = await _unitOfWork.BeginTransactionAsync();
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();
    //         var existingHotel = await _unitOfWork.HotelRepository.GetByIdAsync(id, cancellationToken);
    //         if (existingHotel == null || existingHotel.IsDeleted)
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("hotel");
    //         }

    //         if (imageUploads == null || imageUploads.Count == 0)
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("images");
    //         }

    //         var allMedia = _unitOfWork.HotelMediaRepository.Entities
    //             .Where(dm => dm.HotelId == existingHotel.Id).ToList();

    //         // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin hotel
    //         if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
    //         {
    //             await _unitOfWork.SaveAsync();
    //             await transaction.CommitAsync(cancellationToken);
    //             return new HotelMediaResponse
    //             {
    //                 HotelId = existingHotel.Id,
    //                 // HotelName = existingHotel.Name,
    //                 Media = new List<MediaResponse>()
    //             };
    //         }

    //         bool isThumbnailUpdated = false;

    //         // Nếu có thumbnailSelected và nó là link (ảnh cũ) -> Cập nhật ảnh cũ làm thumbnail
    //         if (!string.IsNullOrEmpty(thumbnailSelected) && Helper.IsValidUrl(thumbnailSelected))
    //         {
    //             foreach (var media in allMedia)
    //             {
    //                 media.IsThumbnail = media.MediaUrl == thumbnailSelected;
    //                 _unitOfWork.HotelMediaRepository.Update(media);
    //             }
    //             isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
    //         }

    //         // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
    //         if (imageUploads == null || imageUploads.Count == 0)
    //         {
    //             await _unitOfWork.SaveAsync();
    //             await transaction.CommitAsync(cancellationToken);
    //             return new HotelMediaResponse
    //             {
    //                 HotelId = existingHotel.Id,
    //                 // HotelName = existingHotel.Name,
    //                 Media = new List<MediaResponse>()
    //             };
    //         }

    //         // Có ảnh mới -> Upload lên Cloudinary
    //         var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
    //         var mediaResponses = new List<MediaResponse>();

    //         for (int i = 0; i < imageUploads.Count; i++)
    //         {
    //             var imageUpload = imageUploads[i];
    //             bool isThumbnail = false;

    //             // Nếu thumbnailSelected là tên file -> Đặt ảnh mới làm thumbnail
    //             if (!string.IsNullOrEmpty(thumbnailSelected) && !Helper.IsValidUrl(thumbnailSelected))
    //             {
    //                 isThumbnail = imageUpload.FileName == thumbnailSelected;
    //             }

    //             var newHotelMedia = new HotelMedia
    //             {
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 HotelId = existingHotel.Id,
    //                 MediaUrl = imageUrls[i],
    //                 SizeInBytes = imageUpload.Length,
    //                 IsThumbnail = isThumbnail,
    //                 CreatedBy = currentUserId,
    //                 CreatedTime = _timeService.SystemTimeNow,
    //                 LastUpdatedBy = currentUserId,
    //             };

    //             await _unitOfWork.HotelMediaRepository.AddAsync(newHotelMedia);
    //             mediaResponses.Add(new MediaResponse
    //             {
    //                 MediaUrl = imageUrls[i],
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 IsThumbnail = isThumbnail,
    //                 SizeInBytes = imageUpload.Length
    //             });

    //             // Nếu ảnh mới được chọn làm thumbnail -> Cập nhật tất cả ảnh cũ về IsThumbnail = false
    //             if (isThumbnail)
    //             {
    //                 foreach (var media in allMedia)
    //                 {
    //                     media.IsThumbnail = false;
    //                     _unitOfWork.HotelMediaRepository.Update(media);
    //                 }
    //                 isThumbnailUpdated = true;
    //             }
    //         }

    //         // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
    //         if (!isThumbnailUpdated && mediaResponses.Count > 0)
    //         {
    //             var firstMedia = mediaResponses.First();
    //             var firstMediaEntity = await _unitOfWork.HotelMediaRepository.ActiveEntities
    //                 .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

    //             if (firstMediaEntity != null)
    //             {
    //                 firstMediaEntity.IsThumbnail = true;
    //                 _unitOfWork.HotelMediaRepository.Update(firstMediaEntity);
    //             }
    //         }

    //         await _unitOfWork.SaveAsync();
    //         await transaction.CommitAsync(cancellationToken);

    //         return new HotelMediaResponse
    //         {
    //             HotelId = existingHotel.Id,
    //             // HotelName = existingHotel.Name,
    //             Media = mediaResponses
    //         };
    //     }
    //     catch (CustomException)
    //     {
    //         await _unitOfWork.RollBackAsync();
    //         throw;
    //     }
    //     catch (Exception)
    //     {
    //         await _unitOfWork.RollBackAsync();
    //         throw CustomExceptionFactory.CreateInternalServerError();
    //     }
    // }

    // có add ảnh kèm theo
    // public async Task<HotelMediaResponse> AddHotelWithMediaAsync(HotelCreateWithMediaFileModel hotelCreateModel, string? thumbnailSelected, CancellationToken cancellationToken)
    // {
    //     using var transaction = await _unitOfWork.BeginTransactionAsync();
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();
    //         var currentTime = _timeService.SystemTimeNow;

    //         //var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), hotelCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
    //         //if (!checkRole)
    //         //{
    //         //    throw CustomExceptionFactory.CreateForbiddenError();
    //         //}

    //         var newHotel = _mapper.Map<Hotel>(hotelCreateModel);
    //         newHotel.CreatedBy = currentUserId;
    //         newHotel.LastUpdatedBy = currentUserId;
    //         newHotel.CreatedTime = currentTime;
    //         newHotel.LastUpdatedTime = currentTime;

    //         await _unitOfWork.HotelRepository.AddAsync(newHotel);
    //         await _unitOfWork.SaveAsync();

    //         var mediaResponses = new List<MediaResponse>();

    //         if (hotelCreateModel.ImageUploads != null && hotelCreateModel.ImageUploads.Count > 0)
    //         {
    //             var imageUrls = await _cloudinaryService.UploadImagesAsync(hotelCreateModel.ImageUploads);

    //             bool isAutoSelectThumbnail = string.IsNullOrEmpty(thumbnailSelected);
    //             bool thumbnailSet = false;

    //             for (int i = 0; i < hotelCreateModel.ImageUploads.Count; i++)
    //             {
    //                 var imageFile = hotelCreateModel.ImageUploads[i];
    //                 var imageUrl = imageUrls[i];

    //                 var newHotelMedia = new HotelMedia
    //                 {
    //                     FileName = imageFile.FileName,
    //                     FileType = imageFile.ContentType,
    //                     HotelId = newHotel.Id,
    //                     MediaUrl = imageUrl,
    //                     SizeInBytes = imageFile.Length,
    //                     CreatedBy = currentUserId,
    //                     CreatedTime = _timeService.SystemTimeNow,
    //                     LastUpdatedBy = currentUserId,
    //                 };

    //                 // Chọn ảnh làm thumbnail
    //                 if ((isAutoSelectThumbnail && i == 0) || (!isAutoSelectThumbnail && imageFile.FileName == thumbnailSelected))
    //                 {
    //                     newHotelMedia.IsThumbnail = true;
    //                     thumbnailSet = true;
    //                 }

    //                 await _unitOfWork.HotelMediaRepository.AddAsync(newHotelMedia);

    //                 mediaResponses.Add(new MediaResponse
    //                 {
    //                     MediaUrl = imageUrl,
    //                     FileName = imageFile.FileName,
    //                     FileType = imageFile.ContentType,
    //                     SizeInBytes = imageFile.Length
    //                 });
    //             }

    //             // Trường hợp người dùng chọn ảnh thumbnail nhưng không tìm thấy ảnh khớp
    //             if (!thumbnailSet && hotelCreateModel.ImageUploads.Count > 0)
    //             {
    //                 var firstMedia = mediaResponses.First();
    //                 var firstHotelMedia = await _unitOfWork.HotelMediaRepository
    //                     .GetFirstByHotelIdAsync(newHotel.Id);
    //                 if (firstHotelMedia != null)
    //                 {
    //                     firstHotelMedia.IsThumbnail = true;
    //                     _unitOfWork.HotelMediaRepository.Update(firstHotelMedia);
    //                 }
    //             }
    //         }

    //         await _unitOfWork.SaveAsync();
    //         await transaction.CommitAsync(cancellationToken);

    //         return new HotelMediaResponse
    //         {
    //             HotelId = newHotel.Id,
    //             // HotelName = newHotel.Name,
    //             Media = mediaResponses
    //         };
    //     }
    //     catch (CustomException)
    //     {
    //         await transaction.RollbackAsync(cancellationToken);
    //         throw;
    //     }
    //     catch (Exception)
    //     {
    //         await transaction.RollbackAsync(cancellationToken);
    //         throw CustomExceptionFactory.CreateInternalServerError();
    //     }
    // }

    // có update ảnh kèm theo
    // public async Task UpdateHotelAsync(
    //     Guid id,
    //     HotelUpdateWithMediaFileModel hotelUpdateModel,
    //     string? thumbnailSelected,
    //     CancellationToken cancellationToken)
    // {
    //     using var transaction = await _unitOfWork.BeginTransactionAsync();
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();

    //         //var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), hotelUpdateModel.DistrictId ?? Guid.Empty, cancellationToken);
    //         //if (!checkRole)
    //         //{
    //         //    throw CustomExceptionFactory.CreateForbiddenError();
    //         //}

    //         var existingHotel = await _unitOfWork.HotelRepository.GetByIdAsync(id, cancellationToken);
    //         if (existingHotel == null || existingHotel.IsDeleted)
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("hotel");
    //         }

    //         _mapper.Map(hotelUpdateModel, existingHotel);

    //         existingHotel.LastUpdatedBy = currentUserId;
    //         existingHotel.LastUpdatedTime = _timeService.SystemTimeNow;

    //         _unitOfWork.HotelRepository.Update(existingHotel);

    //         // xu ly anh
    //         var imageUploads = hotelUpdateModel.ImageUploads;
    //         var allMedia = _unitOfWork.HotelMediaRepository.ActiveEntities
    //             .Where(dm => dm.HotelId == existingHotel.Id).ToList();

    //         // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin hotel
    //         if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
    //         {
    //             await _unitOfWork.SaveAsync();
    //             await transaction.CommitAsync(cancellationToken);
    //             return;
    //         }

    //         bool isThumbnailUpdated = false;

    //         // Nếu có thumbnailSelected và nó là link (ảnh cũ) -> Cập nhật ảnh cũ làm thumbnail
    //         if (!string.IsNullOrEmpty(thumbnailSelected) && Helper.IsValidUrl(thumbnailSelected))
    //         {
    //             foreach (var media in allMedia)
    //             {
    //                 media.IsThumbnail = media.MediaUrl == thumbnailSelected;
    //                 _unitOfWork.HotelMediaRepository.Update(media);
    //             }
    //             isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
    //         }

    //         // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
    //         if (imageUploads == null || imageUploads.Count == 0)
    //         {
    //             await _unitOfWork.SaveAsync();
    //             await transaction.CommitAsync(cancellationToken);
    //             return;
    //         }

    //         // Có ảnh mới -> Upload lên Cloudinary
    //         var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
    //         var mediaResponses = new List<MediaResponse>();

    //         for (int i = 0; i < imageUploads.Count; i++)
    //         {
    //             var imageUpload = imageUploads[i];
    //             bool isThumbnail = false;

    //             // Nếu thumbnailSelected là tên file -> Đặt ảnh mới làm thumbnail
    //             if (!string.IsNullOrEmpty(thumbnailSelected) && !Helper.IsValidUrl(thumbnailSelected))
    //             {
    //                 isThumbnail = imageUpload.FileName == thumbnailSelected;
    //             }

    //             var newHotelMedia = new HotelMedia
    //             {
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 HotelId = existingHotel.Id,
    //                 MediaUrl = imageUrls[i],
    //                 SizeInBytes = imageUpload.Length,
    //                 IsThumbnail = isThumbnail,
    //                 CreatedBy = currentUserId,
    //                 CreatedTime = _timeService.SystemTimeNow,
    //                 LastUpdatedBy = currentUserId,
    //             };

    //             await _unitOfWork.HotelMediaRepository.AddAsync(newHotelMedia);
    //             mediaResponses.Add(new MediaResponse
    //             {
    //                 MediaUrl = imageUrls[i],
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 IsThumbnail = isThumbnail,
    //                 SizeInBytes = imageUpload.Length
    //             });

    //             // Nếu ảnh mới được chọn làm thumbnail -> Cập nhật tất cả ảnh cũ về IsThumbnail = false
    //             if (isThumbnail)
    //             {
    //                 foreach (var media in allMedia)
    //                 {
    //                     media.IsThumbnail = false;
    //                     _unitOfWork.HotelMediaRepository.Update(media);
    //                 }
    //                 isThumbnailUpdated = true;
    //             }
    //         }

    //         // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
    //         if (!isThumbnailUpdated && mediaResponses.Count > 0)
    //         {
    //             var firstMedia = mediaResponses.First();
    //             var firstMediaEntity = await _unitOfWork.HotelMediaRepository.ActiveEntities
    //                 .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

    //             if (firstMediaEntity != null)
    //             {
    //                 firstMediaEntity.IsThumbnail = true;
    //                 _unitOfWork.HotelMediaRepository.Update(firstMediaEntity);
    //             }
    //         }

    //         await _unitOfWork.SaveAsync();
    //         await transaction.CommitAsync(cancellationToken);
    //     }
    //     catch (CustomException)
    //     {
    //         await transaction.RollbackAsync(cancellationToken);
    //         throw;
    //     }
    //     catch (Exception)
    //     {
    //         await transaction.RollbackAsync(cancellationToken);
    //         throw CustomExceptionFactory.CreateInternalServerError();
    //     }
    // }

    private async Task<List<MediaResponse>> GetMediaByIdAsync(Guid hotelId, CancellationToken cancellationToken)
    {
        try
        {
            var existingHotel = await _unitOfWork.HotelRepository.GetByIdAsync(hotelId, cancellationToken);

            var hotelMedias = await _unitOfWork.LocationMediaRepository
                .ActiveEntities
                .Where(em => em.LocationId == existingHotel!.LocationId)
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
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    //public async Task<HotelDataModel?> GetHotelByNameAsync(string name, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var existingHotel = await _unitOfWork.HotelRepository.GetByNameAsync(name, cancellationToken);
    //        if (existingHotel == null || existingHotel.IsDeleted)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("hotel");
    //        }

    //        return _mapper.Map<HotelDataModel>(existingHotel);
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