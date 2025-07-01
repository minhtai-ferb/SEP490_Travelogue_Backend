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
    // Task<CuisineMediaResponse> AddCuisineWithMediaAsync(CuisineCreateWithMediaFileModel cuisineCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    // Task UpdateCuisineAsync(Guid id, CuisineUpdateWithMediaFileModel cuisineUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task<PagedResult<CuisineDataModel>> GetPagedCuisinesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<CuisineDataModel>> GetPagedCuisinesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    // Task<CuisineMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
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
        _cloudinaryService = cloudinaryService;
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

            await _unitOfWork.LocationMediaRepository.ActiveEntities
               .Where(s => s.LocationId == existingCuisine.LocationId)
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

    // public async Task<CuisineMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken)
    // {
    //     _unitOfWork.BeginTransaction();
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();
    //         var existingCuisine = await _unitOfWork.CuisineRepository.GetWithIncludeAsync(id, c => c.Include(c => c.Location));
    //         if (existingCuisine == null || existingCuisine.IsDeleted)
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("nhà hàng");
    //         }

    //         if (imageUploads == null || !imageUploads.Any())
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("medias");
    //         }

    //         var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
    //         var mediaResponses = new List<MediaResponse>();

    //         for (int i = 0; i < imageUploads.Count; i++)
    //         {
    //             var imageUpload = imageUploads[i];

    //             var newCuisineMedia = new CuisineMedia
    //             {
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 CuisineId = existingCuisine.Id,
    //                 MediaUrl = imageUrls[i],
    //                 SizeInBytes = imageUpload.Length,
    //                 CreatedBy = currentUserId,
    //                 CreatedTime = _timeService.SystemTimeNow,
    //                 LastUpdatedBy = currentUserId,
    //             };

    //             await _unitOfWork.CuisineMediaRepository.AddAsync(newCuisineMedia);

    //             mediaResponses.Add(new MediaResponse
    //             {
    //                 MediaUrl = imageUrls[i],
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 SizeInBytes = imageUpload.Length
    //             });
    //         }

    //         await _unitOfWork.SaveAsync();
    //         _unitOfWork.CommitTransaction();

    //         return new CuisineMediaResponse
    //         {
    //             CuisineId = existingCuisine.Id,
    //             CuisineName = existingCuisine.Location.Name,
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

    // có add kèm theo ảnh

    private async Task<List<MediaResponse>> GetMediaByIdAsync(Guid cuisineId, CancellationToken cancellationToken)
    {
        try
        {
            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(cuisineId, cancellationToken);

            var cuisineMedias = await _unitOfWork.LocationMediaRepository
                .ActiveEntities
                .Where(em => em.LocationId == existingCuisine!.LocationId)
                .ToListAsync(cancellationToken);

            return cuisineMedias.Select(x => new MediaResponse
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
                var cuisineMedia = await _unitOfWork.LocationMediaRepository
                    .ActiveEntities
                    .FirstOrDefaultAsync(m => m.LocationId == existingCuisine.LocationId && m.MediaUrl == imageUpload && !m.IsDeleted, cancellationToken);

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
}