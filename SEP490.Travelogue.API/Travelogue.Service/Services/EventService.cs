using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.EventModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IEventService
{
    Task<EventDataModel?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<EventDataModel>> GetAllEventsAsync(CancellationToken cancellationToken);
    Task<EventDataModel> AddEventAsync(EventCreateModel eventCreateModel, CancellationToken cancellationToken);
    Task UpdateEventAsync(Guid id, EventUpdateModel eventUpdateModel, CancellationToken cancellationToken);
    Task DeleteEventAsync(Guid id, CancellationToken cancellationToken);

    Task<EventMediaResponse> AddEventWithMediaAsync(EventCreateWithMediaFileModel eventCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task UpdateEventAsync(Guid id, EventUpdateWithMediaFileModel eventUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);

    Task<PagedResult<EventDataModel>> GetPagedEventsWithSearchAsync(string? name, Guid? typeId, Guid? locationId, Guid? districtId, int? month, int? year, int pageNumber, int pageSize, CancellationToken cancellationToken);
    //Task<PagedResult<EventDataModel>> GetPagedEventsWithSearchAsync(int pageNumber, int pageSize, string name, Guid typeId, Guid locationId, int? month, int? year, CancellationToken cancellationToken);
    //Task<EventDataModel?> GetActivitiesWithFilterAsync(Guid typeId, Guid locationId, CancellationToken cancellationToken);
    Task<Dictionary<int, Dictionary<int, List<EventDataModel>>>> GetHighlightedEventsAsync(CancellationToken cancellationToken);
    Task<List<object>> GetHighlightedEventsObjectAsync(CancellationToken cancellationToken);
    Task<EventMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, string? thumbnailFileName, CancellationToken cancellationToken);
    Task<bool> DeleteMediaAsync(Guid id, List<string> deletedUrlImages, CancellationToken cancellationToken);
    Task<List<EventDataModel>> GetAllEventAdminAsync();
}

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;

    public EventService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<EventDataModel> AddEventAsync(EventCreateModel eventCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), eventCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var newEvent = _mapper.Map<Event>(eventCreateModel);

            if (newEvent.IsHighlighted)
            {
                var existingHighlightedEvent = await _unitOfWork.EventRepository.GetHighlightedEvent(cancellationToken);
                if (existingHighlightedEvent.Count >= LimitNumber.HIGHLIGHTED_EVENT_LIMIT)
                {
                    throw CustomExceptionFactory.CreateBadRequestError(ResponseMessages.HIGHLIGHTED_EVENT_LIMIT);
                }
            }

            newEvent.CreatedBy = currentUserId;
            newEvent.LastUpdatedBy = currentUserId;
            newEvent.CreatedTime = currentTime;
            newEvent.LastUpdatedTime = currentTime;

            await _unitOfWork.SaveAsync();
            await _unitOfWork.EventRepository.AddAsync(newEvent);

            await transaction.CommitAsync(cancellationToken);
            var result = _unitOfWork.EventRepository.ActiveEntities
                .FirstOrDefault(l => l.Id == newEvent.Id);

            return _mapper.Map<EventDataModel>(result);
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

    public async Task DeleteEventAsync(Guid id, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingEvent = await _unitOfWork.EventRepository.GetByIdAsync(id, cancellationToken);

            if (existingEvent == null || existingEvent.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("event");
            }

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), existingEvent.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var isInUsing =
                await _unitOfWork.NewsRepository.ActiveEntities.FirstOrDefaultAsync(e => e.EventId == id, cancellationToken) != null;
            if (isInUsing)
            {
                throw CustomExceptionFactory.CreateBadRequestError(ResponseMessages.BE_USED);
            }

            // Xóa tất cả media liên quan đến sự kiện
            await _unitOfWork.EventMediaRepository.ActiveEntities
                .Where(s => s.EventId == id)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            existingEvent.LastUpdatedBy = currentUserId;
            existingEvent.DeletedBy = currentUserId;
            existingEvent.DeletedTime = currentTime;
            existingEvent.LastUpdatedTime = currentTime;
            existingEvent.IsDeleted = true;

            _unitOfWork.EventRepository.Update(existingEvent);

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

    public async Task<List<EventDataModel>> GetAllEventsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var events = await _unitOfWork.EventRepository.GetAllAsync(cancellationToken);
            if (!events.Any())
            {
                return new List<EventDataModel>();
            }

            var eventMedias = await _unitOfWork.EventMediaRepository
                .ActiveEntities
                .Where(em => events.Select(e => e.Id).Contains(em.EventId))
                .ToListAsync(cancellationToken);

            // Ánh xạ sự kiện sang EventDataModel
            var eventDataModels = _mapper.Map<List<EventDataModel>>(events);

            // Gán danh sách media va ten cua cac truong thonng tin can vào từng sự kiện
            foreach (var eventData in eventDataModels)
            {
                //eventData.Medias = eventMedias
                //    .Where(em => em.EventId == eventData.Id)
                //    .Select(em => new MediaResponse
                //    {
                //        MediaUrl = em.MediaUrl,
                //        FileName = em.FileName ?? string.Empty,
                //        FileType = em.FileType,
                //        IsThumbnail = em.IsThumbnail,
                //        SizeInBytes = em.SizeInBytes,
                //        CreatedTime = em.CreatedTime
                //    })
                //    .ToList();
                eventData.Medias = await GetMediaWithoutVideoByIdAsync(eventData.Id, cancellationToken);

                eventData.DistrictName = eventData.DistrictId != null
                    ? await _unitOfWork.DistrictRepository.ActiveEntities.Where(x => x.Id == eventData.DistrictId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;

                eventData.LocationName = eventData.LocationId != null
                    ? await _unitOfWork.LocationRepository.ActiveEntities.Where(x => x.Id == eventData.LocationId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;

                eventData.TypeEventName = eventData.TypeEventId != null
                    ? await _unitOfWork.TypeEventRepository.ActiveEntities.Where(x => x.Id == eventData.TypeEventId)
                        .Select(x => x.TypeName)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;
            }

            return eventDataModels;
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

    public async Task<EventDataModel?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingEvent = await _unitOfWork.EventRepository.GetByIdAsync(id, cancellationToken);
            if (existingEvent == null || existingEvent.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("event");
            }

            var eventMedias = await _unitOfWork.EventMediaRepository
                .ActiveEntities
                .Where(em => em.EventId == id)
                .ToListAsync(cancellationToken);

            var eventDataModel = _mapper.Map<EventDataModel>(existingEvent);

            eventDataModel.Medias = eventMedias
                .Select(em => new MediaResponse
                {
                    MediaUrl = em.MediaUrl,
                    FileName = em.FileName ?? string.Empty,
                    IsThumbnail = em.IsThumbnail,
                    FileType = em.FileType,
                    SizeInBytes = em.SizeInBytes,
                    CreatedTime = em.CreatedTime
                })
                .ToList();

            return eventDataModel;
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

    public async Task UpdateEventAsync(Guid id, EventUpdateModel eventUpdateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingEvent = await _unitOfWork.EventRepository.GetByIdAsync(id, cancellationToken);
            if (existingEvent == null || existingEvent.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("event");
            }

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), existingEvent.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            _mapper.Map(eventUpdateModel, existingEvent);

            existingEvent.LastUpdatedBy = currentUserId;
            existingEvent.LastUpdatedTime = _timeService.SystemTimeNow;

            if (existingEvent.IsHighlighted)
            {
                var existingHighlightedEvent = await _unitOfWork.EventRepository.GetHighlightedEvent(cancellationToken);
                if (existingHighlightedEvent.Count >= LimitNumber.HIGHLIGHTED_EVENT_LIMIT)
                {
                    throw CustomExceptionFactory.CreateBadRequestError(ResponseMessages.HIGHLIGHTED_EVENT_LIMIT);
                }
            }

            _unitOfWork.EventRepository.Update(existingEvent);

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

    public async Task<PagedResult<EventDataModel>> GetPagedEventsWithSearchAsync(int pageNumber, int pageSize, string name, Guid typeId, Guid locationId, int? month, int? year, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.EventRepository.GetPageWithFilterAsync(pageNumber, pageSize, name, typeId, locationId, month, year, cancellationToken);

            var eventIds = pagedResult.Items.Select(e => e.Id).ToList();

            var eventMedias = await _unitOfWork.EventMediaRepository
                .ActiveEntities
                .Where(em => eventIds.Contains(em.EventId))
                .Select(em => new
                {
                    em.EventId,
                    Media = new MediaResponse
                    {
                        MediaUrl = em.MediaUrl,
                        FileName = em.FileName ?? string.Empty,
                        FileType = em.FileType,
                        IsThumbnail = em.IsThumbnail,
                        SizeInBytes = em.SizeInBytes,
                        CreatedTime = em.CreatedTime
                    }
                })
                .ToListAsync(cancellationToken);

            var eventDataModels = _mapper.Map<List<EventDataModel>>(pagedResult.Items);

            foreach (var eventData in eventDataModels)
            {
                eventData.Medias = eventMedias
                    .Where(em => em.EventId == eventData.Id)
                    .Select(em => em.Media)
                    .ToList();

                eventData.DistrictName = eventData.DistrictId != null
                    ? await _unitOfWork.DistrictRepository.ActiveEntities.Where(x => x.Id == eventData.DistrictId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;

                eventData.LocationName = eventData.LocationId != null
                    ? await _unitOfWork.LocationRepository.ActiveEntities.Where(x => x.Id == eventData.LocationId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;

                eventData.TypeEventName = eventData.TypeEventId != null
                    ? await _unitOfWork.TypeEventRepository.ActiveEntities.Where(x => x.Id == eventData.TypeEventId)
                        .Select(x => x.TypeName)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;
            }

            return new PagedResult<EventDataModel>
            {
                Items = eventDataModels,
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

    public async Task<Dictionary<int, Dictionary<int, List<EventDataModel>>>> GetHighlightedEventsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var events = await _unitOfWork.EventRepository.GetAllAsync();

            var eventIds = events.Select(e => e.Id).ToList();

            var eventMedias = await _unitOfWork.EventMediaRepository
                .ActiveEntities
                .Where(em => eventIds.Contains(em.EventId))
                .Select(em => new
                {
                    em.EventId,
                    Media = new MediaResponse
                    {
                        MediaUrl = em.MediaUrl,
                        FileName = em.FileName ?? string.Empty,
                        FileType = em.FileType,
                        IsThumbnail = em.IsThumbnail,
                        SizeInBytes = em.SizeInBytes,
                        CreatedTime = em.CreatedTime
                    }
                })
                .ToListAsync(cancellationToken);

            var eventDataModels = _mapper.Map<List<EventDataModel>>(events);

            foreach (var eventData in eventDataModels)
            {
                eventData.Medias = eventMedias
                    .Where(em => em.EventId == eventData.Id)
                    .Select(em => em.Media)
                    .ToList();

                eventData.DistrictName = eventData.DistrictId != null
                   ? await _unitOfWork.DistrictRepository.ActiveEntities.Where(x => x.Id == eventData.DistrictId)
                       .Select(x => x.Name)
                       .FirstOrDefaultAsync(cancellationToken)
                   : null;

                eventData.LocationName = eventData.LocationId != null
                    ? await _unitOfWork.LocationRepository.ActiveEntities.Where(x => x.Id == eventData.LocationId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;

                eventData.TypeEventName = eventData.TypeEventId != null
                    ? await _unitOfWork.TypeEventRepository.ActiveEntities.Where(x => x.Id == eventData.TypeEventId)
                        .Select(x => x.TypeName)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;
            }

            var result = eventDataModels
                .Where(e => e.StartDate.HasValue)
                .GroupBy(e => e.StartDate.GetValueOrDefault().Year)
                .ToDictionary(
                    yearGroup => yearGroup.Key,
                    yearGroup => yearGroup
                        .GroupBy(e => e.StartDate.GetValueOrDefault().Month)
                        .ToDictionary(
                            monthGroup => monthGroup.Key,
                            monthGroup => monthGroup.ToList()
                        )
                );

            return result;
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

    public async Task<List<object>> GetHighlightedEventsObjectAsync(CancellationToken cancellationToken)
    {
        try
        {
            var query = _unitOfWork.EventRepository.ActiveEntities;
            var events = await query.Where(e => e.IsHighlighted == true).ToListAsync();

            var eventIds = events.Select(e => e.Id).ToList();

            var eventMedias = await _unitOfWork.EventMediaRepository
                .ActiveEntities
                .Where(em => eventIds.Contains(em.EventId))
                .Select(em => new
                {
                    em.EventId,
                    Media = new MediaResponse
                    {
                        MediaUrl = em.MediaUrl,
                        FileName = em.FileName ?? string.Empty,
                        FileType = em.FileType,
                        IsThumbnail = em.IsThumbnail,
                        SizeInBytes = em.SizeInBytes,
                        CreatedTime = em.CreatedTime
                    }
                })
                .ToListAsync(cancellationToken);

            var eventDataModels = _mapper.Map<List<EventDataModel>>(events);

            foreach (var eventData in eventDataModels)
            {
                eventData.Medias = eventMedias
                    .Where(em => em.EventId == eventData.Id)
                    .Select(em => em.Media)
                    .ToList();

                eventData.DistrictName = eventData.DistrictId != null
                    ? await _unitOfWork.DistrictRepository.ActiveEntities.Where(x => x.Id == eventData.DistrictId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;

                eventData.LocationName = eventData.LocationId != null
                    ? await _unitOfWork.LocationRepository.ActiveEntities.Where(x => x.Id == eventData.LocationId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;

                eventData.TypeEventName = eventData.TypeEventId != null
                    ? await _unitOfWork.TypeEventRepository.ActiveEntities.Where(x => x.Id == eventData.TypeEventId)
                        .Select(x => x.TypeName)
                        .FirstOrDefaultAsync(cancellationToken)
                    : null;
            }

            foreach (var item in eventDataModels)
            {
                item.CreatedByName = item.CreatedBy != null
                    ? await _unitOfWork.UserRepository.GetUserNameByIdAsync(Guid.Parse(item.CreatedBy))
                    : null;
                item.LastUpdatedByName = item.LastUpdatedBy != null
                    ? await _unitOfWork.UserRepository.GetUserNameByIdAsync(Guid.Parse(item.LastUpdatedBy))
                    : null;
            }

            var result = eventDataModels
                .Where(e => e.StartDate.HasValue)
                .OrderBy(e => e.StartDate)
                .GroupBy(e => e.StartDate.GetValueOrDefault().Year)
                .Select(yearGroup => new EventGroupByYear
                {
                    Year = yearGroup.Key,
                    Months = yearGroup
                        .GroupBy(e => e.StartDate.GetValueOrDefault().Month)
                        .Select(monthGroup => new EventGroupByMonth
                        {
                            Month = monthGroup.Key,
                            Events = monthGroup.ToList()
                        })
                        .ToList()
                })
                .ToList();

            return result.Cast<object>().ToList();
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

    public async Task<PagedResult<EventDataModel>> GetPagedEventsWithSearchAsync(
        string? name, Guid? typeId, Guid? locationId, Guid? districtId, int? month, int? year,
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.EventRepository.GetPageWithFilterAsync(name, typeId, locationId, districtId, month, year, pageNumber, pageSize, cancellationToken);

            var eventDataModels = _mapper.Map<List<EventDataModel>>(pagedResult.Items);

            return new PagedResult<EventDataModel>
            {
                Items = eventDataModels,
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

    public async Task<EventMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingEvent = await _unitOfWork.EventRepository.GetByIdAsync(id, cancellationToken);
            if (existingEvent == null || existingEvent.IsDeleted)
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

                var newEventMedia = new EventMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    EventId = existingEvent.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.EventMediaRepository.AddAsync(newEventMedia);

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

            return new EventMediaResponse
            {
                EventId = existingEvent.Id,
                EventName = existingEvent.Name,
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

    // có update ảnh kèm theo
    public async Task<EventMediaResponse> AddEventWithMediaAsync(EventCreateWithMediaFileModel eventCreateModel, string? thumbnailSelected, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), eventCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var newEvent = _mapper.Map<Event>(eventCreateModel);
            newEvent.CreatedBy = currentUserId;
            newEvent.LastUpdatedBy = currentUserId;
            newEvent.CreatedTime = currentTime;
            newEvent.LastUpdatedTime = currentTime;

            await _unitOfWork.EventRepository.AddAsync(newEvent);
            await _unitOfWork.SaveAsync();

            var mediaResponses = new List<MediaResponse>();

            if (eventCreateModel.ImageUploads != null && eventCreateModel.ImageUploads.Count > 0)
            {
                var imageUrls = await _cloudinaryService.UploadImagesAsync(eventCreateModel.ImageUploads);

                bool isAutoSelectThumbnail = string.IsNullOrEmpty(thumbnailSelected);
                bool thumbnailSet = false;

                for (int i = 0; i < eventCreateModel.ImageUploads.Count; i++)
                {
                    var imageFile = eventCreateModel.ImageUploads[i];
                    var imageUrl = imageUrls[i];

                    var newEventMedia = new EventMedia
                    {
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        EventId = newEvent.Id,
                        MediaUrl = imageUrl,
                        SizeInBytes = imageFile.Length,
                        CreatedBy = currentUserId,
                        CreatedTime = _timeService.SystemTimeNow,
                        LastUpdatedBy = currentUserId,
                    };

                    // Chọn ảnh làm thumbnail
                    if ((isAutoSelectThumbnail && i == 0) || (!isAutoSelectThumbnail && imageFile.FileName == thumbnailSelected))
                    {
                        newEventMedia.IsThumbnail = true;
                        thumbnailSet = true;
                    }

                    await _unitOfWork.EventMediaRepository.AddAsync(newEventMedia);

                    mediaResponses.Add(new MediaResponse
                    {
                        MediaUrl = imageUrl,
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        SizeInBytes = imageFile.Length
                    });
                }

                // Trường hợp người dùng chọn ảnh thumbnail nhưng không tìm thấy ảnh khớp
                if (!thumbnailSet && eventCreateModel.ImageUploads.Count > 0)
                {
                    var firstMedia = mediaResponses.First();
                    var firstEventMedia = await _unitOfWork.EventMediaRepository
                        .GetFirstByEventIdAsync(newEvent.Id);
                    if (firstEventMedia != null)
                    {
                        firstEventMedia.IsThumbnail = true;
                        _unitOfWork.EventMediaRepository.Update(firstEventMedia);
                    }
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new EventMediaResponse
            {
                EventId = newEvent.Id,
                EventName = newEvent.Name,
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
    public async Task UpdateEventAsync(
        Guid id,
        EventUpdateWithMediaFileModel eventUpdateModel,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), eventUpdateModel.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var existingEvent = await _unitOfWork.EventRepository.GetByIdAsync(id, cancellationToken);
            if (existingEvent == null || existingEvent.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("event");
            }

            _mapper.Map(eventUpdateModel, existingEvent);

            existingEvent.LastUpdatedBy = currentUserId;
            existingEvent.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.EventRepository.Update(existingEvent);

            // xu ly anh
            var imageUploads = eventUpdateModel.ImageUploads;
            var allMedia = _unitOfWork.EventMediaRepository.ActiveEntities
                .Where(dm => dm.EventId == existingEvent.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin event
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
                    _unitOfWork.EventMediaRepository.Update(media);
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

                var newEventMedia = new EventMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    EventId = existingEvent.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.EventMediaRepository.AddAsync(newEventMedia);
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
                        _unitOfWork.EventMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.EventMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.EventMediaRepository.Update(firstMediaEntity);
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

    public async Task<bool> DeleteMediaAsync(Guid id, List<string> imageDeleted, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingEvent = await _unitOfWork.EventRepository.GetByIdAsync(id, cancellationToken);
            if (existingEvent == null || existingEvent.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("event");
            }

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), id, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            if (imageDeleted == null || imageDeleted.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var imageUrlsDeleted = await _cloudinaryService.DeleteImagesAsync(imageDeleted);

            if (!imageUrlsDeleted)
            {
                return false;
            }

            foreach (var imageUpload in imageDeleted)
            {
                var eventMedia = await _unitOfWork.EventMediaRepository
                    .ActiveEntities
                    .FirstOrDefaultAsync(m => m.EventId == id && m.MediaUrl == imageUpload, cancellationToken);

                if (eventMedia != null)
                {
                    _unitOfWork.EventMediaRepository.Remove(eventMedia);
                }

                //if (eventMedia != null)
                //{
                //    eventMedia.IsDeleted = true;
                //    eventMedia.DeletedTime = DateTime.UtcNow;
                //}
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

    public async Task<EventMediaResponse> UploadMediaAsync(
        Guid id,
        List<IFormFile>? imageUploads,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingEvent = await _unitOfWork.EventRepository.GetByIdAsync(id, cancellationToken);
            if (existingEvent == null || existingEvent.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("event");
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

            var allMedia = _unitOfWork.EventMediaRepository.Entities
                .Where(dm => dm.EventId == existingEvent.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin event
            if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return new EventMediaResponse
                {
                    EventId = existingEvent.Id,
                    EventName = existingEvent.Name,
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
                    _unitOfWork.EventMediaRepository.Update(media);
                }
                isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
            }

            // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
            if (imageUploads == null || imageUploads.Count == 0)
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return new EventMediaResponse
                {
                    EventId = existingEvent.Id,
                    EventName = existingEvent.Name,
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

                var newEventMedia = new EventMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    EventId = existingEvent.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.EventMediaRepository.AddAsync(newEventMedia);
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
                        _unitOfWork.EventMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.EventMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.EventMediaRepository.Update(firstMediaEntity);
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new EventMediaResponse
            {
                EventId = existingEvent.Id,
                EventName = existingEvent.Name,
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

    public async Task<List<EventDataModel>> GetAllEventAdminAsync()
    {
        try
        {
            // 1. Lấy userId nếu có
            var currentUserId = _userContextService.TryGetCurrentUserId();

            // 2. Nếu không có userId → là khách → trả toàn bộ
            if (string.IsNullOrEmpty(currentUserId))
            {
                var allEvents = await _unitOfWork.EventRepository.GetAllAsync();
                var allEventDataModels = _mapper.Map<List<EventDataModel>>(allEvents);
                await EnrichEventDataModelsAsync(allEventDataModels, new CancellationToken());
                return allEventDataModels;
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

            List<Event> events;

            // 6. Admin toàn quyền
            if (roleNames.Equals(AppRole.ADMIN))
            {
                events = (await _unitOfWork.EventRepository.GetAllAsync()).ToList();
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
                    // Là admin huyện → chỉ được lấy các Event trong danh sách huyện đó
                    events = await _unitOfWork.EventRepository.ActiveEntities
                        .Where(l => l.DistrictId.HasValue && allowedDistrictIds.Contains(l.DistrictId.Value))
                        .ToListAsync();
                }
                else
                {
                    // Không có quyền theo huyện nào → xem là người dùng thường
                    events = (await _unitOfWork.EventRepository.GetAllAsync()).ToList();
                }
            }

            var eventDataModels = _mapper.Map<List<EventDataModel>>(events);
            await EnrichEventDataModelsAsync(eventDataModels, new CancellationToken());
            return eventDataModels;
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

    private async Task EnrichEventDataModelsAsync(List<EventDataModel> eventDataModels, CancellationToken cancellationToken)
    {
        var eventIds = eventDataModels.Select(x => x.Id).ToList();
        var districtIds = eventDataModels.Where(x => x.DistrictId.HasValue).Select(x => x.DistrictId.Value).Distinct().ToList();
        var typeEventIds = eventDataModels.Where(x => x.TypeEventId.HasValue).Select(x => x.TypeEventId.Value).Distinct().ToList();

        var allMedias = await _unitOfWork.EventMediaRepository
            .ActiveEntities
            .Where(m => eventIds.Contains(m.EventId) && !m.FileType.Contains("video"))
            .ToListAsync(cancellationToken);

        var districtNames = await _unitOfWork.DistrictRepository
            .ActiveEntities
            .Where(d => districtIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        var typeEventNames = await _unitOfWork.TypeEventRepository
            .ActiveEntities
            .Where(t => typeEventIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.TypeName, cancellationToken);

        foreach (var eventModel in eventDataModels)
        {
            eventModel.Medias = _mapper.Map<List<MediaResponse>>(allMedias
                .Where(m => m.EventId == eventModel.Id)
                .ToList());

            if (eventModel.DistrictId.HasValue && districtNames.TryGetValue(eventModel.DistrictId.Value, out var districtName))
            {
                eventModel.DistrictName = districtName;
            }

            if (eventModel.TypeEventId.HasValue && typeEventNames.TryGetValue(eventModel.TypeEventId.Value, out var typeEventName))
            {
                eventModel.TypeEventName = typeEventName;
            }

        }
    }

    private async Task<List<MediaResponse>> GetMediaWithoutVideoByIdAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var eventMedias = await _unitOfWork.EventMediaRepository
            .ActiveEntities
            .Where(em => em.EventId == eventId)
            //.Where(em => !EF.Functions.Like(em.FileType, "%video%"))
            .ToListAsync(cancellationToken);

        return eventMedias.Select(x => new MediaResponse
        {
            MediaUrl = x.MediaUrl,
            FileName = x.FileName ?? string.Empty,
            FileType = x.FileType,
            IsThumbnail = x.IsThumbnail,
            SizeInBytes = x.SizeInBytes,
            CreatedTime = x.CreatedTime
        }).ToList();
    }

    //public async Task<EventDataModel?> GetEventByNameAsync(string name, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var existingEvent = await _unitOfWork.EventRepository.GetByNameAsync(name, cancellationToken);
    //        if (existingEvent == null || existingEvent.IsDeleted)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("event");
    //        }

    //        return _mapper.Map<EventDataModel>(existingEvent);
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

    //public async Task<PagedResult<EventDataModel>> GetPagedEventsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var pagedResult = await _unitOfWork.EventRepository.GetPageAsync(pageNumber, pageSize);

    //        var eventDataModels = _mapper.Map<List<EventDataModel>>(pagedResult.Items);

    //        return new PagedResult<EventDataModel>
    //        {
    //            Items = eventDataModels,
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

    //public async Task<PagedResult<EventDataModel>> GetPagedEventsWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var pagedResult = await _unitOfWork.EventRepository.GetPageWithSearchAsync(pageNumber, pageSize, name, cancellationToken);

    //        var eventDataModels = _mapper.Map<List<EventDataModel>>(pagedResult.Items);

    //        return new PagedResult<EventDataModel>
    //        {
    //            Items = eventDataModels,
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
