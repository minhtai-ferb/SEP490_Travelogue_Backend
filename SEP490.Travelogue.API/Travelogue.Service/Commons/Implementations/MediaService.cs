using Microsoft.AspNetCore.Http;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Commons.Implementations;

public class MediaService : IMediaService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITimeService _timeService;
    private readonly string _uploadPath;
    private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB 
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

    public MediaService(IHttpContextAccessor httpContextAccessor, ITimeService timeService)
    {
        _httpContextAccessor = httpContextAccessor;
        _timeService = timeService;
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "images");

        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        try
        {
            ValidateFile(image);

            var fileName = GenerateUniqueFileName(image.FileName);
            var filePath = Path.Combine(_uploadPath, fileName);

            await SaveFileAsync(image, filePath);

            return CreateMediaResponse_2(fileName, image);
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

    public async Task<List<string>> UploadMultipleImagesAsync(IEnumerable<IFormFile> images)
    {
        try
        {
            if (images == null || !images.Any())
                throw CustomExceptionFactory.CreateNotFoundError("images");

            var responses = new List<string>();

            foreach (var image in images)
            {
                var response = await UploadImageAsync(image);
                responses.Add(response);
            }

            return responses;
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

    public async Task<bool> DeleteImageAsync(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
                throw CustomExceptionFactory.CreateNotFoundError("fileName");

            var filePath = Path.Combine(_uploadPath, fileName);

            if (!File.Exists(filePath))
                throw CustomExceptionFactory.CreateNotFoundError("image");

            await Task.Run(() => File.Delete(filePath));
            return true;
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

    public async Task<bool> DeleteImagesAsync(List<string> fileNames)
    {
        try
        {
            if (fileNames == null || !fileNames.Any())
                throw CustomExceptionFactory.CreateNotFoundError("fileNames list");

            var deletionTasks = new List<Task>();
            var failedFiles = new List<string>();

            foreach (var fileName in fileNames)
            {
                if (string.IsNullOrEmpty(fileName))
                    continue;

                var filePath = Path.Combine(_uploadPath, fileName);

                if (!File.Exists(filePath))
                {
                    failedFiles.Add(fileName);
                    continue;
                }

                deletionTasks.Add(Task.Run(() => File.Delete(filePath)));
            }

            await Task.WhenAll(deletionTasks);

            if (failedFiles.Any())
                throw CustomExceptionFactory.CreateNotFoundError($"Some images not found: {string.Join(", ", failedFiles)}");

            return true;
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

    public async Task<List<MediaResponse>> GetAllImagesAsync()
    {
        try
        {
            var files = Directory.GetFiles(_uploadPath);
            var responses = new List<MediaResponse>();

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                responses.Add(new MediaResponse
                {
                    FileName = Path.GetFileName(file),
                    MediaUrl = GenerateImageUrl(Path.GetFileName(file)),
                    FileType = MimeTypeMap.GetMimeType(Path.GetExtension(file)),
                    SizeInBytes = fileInfo.Length,
                    IsThumbnail = false,
                    CreatedTime = fileInfo.CreationTime
                });
            }

            return await Task.FromResult(responses.OrderByDescending(x => x.CreatedTime).ToList());
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw CustomExceptionFactory.CreateNotFoundError("image");

        // if (file.Length > _maxFileSize)
        //     throw CustomExceptionFactory.CreateBadRequestError("File size exceeds maximum limit");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            throw CustomExceptionFactory.CreateBadRequestError("Invalid file extension");
    }

    private string GenerateUniqueFileName(string originalFileName)
    {
        return $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
    }

    private async Task SaveFileAsync(IFormFile file, string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
    }

    private string GenerateImageUrl(string fileName)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        return request != null
            ? $"{request.Scheme}://{request.Host}/images/{fileName}"
            : $"/images/{fileName}";
    }

    private MediaResponse CreateMediaResponse(string fileName, IFormFile image)
    {
        return new MediaResponse
        {
            FileName = fileName,
            MediaUrl = GenerateImageUrl(fileName),
            FileType = image.ContentType,
            SizeInBytes = image.Length,
            IsThumbnail = false,
            CreatedTime = _timeService.SystemTimeNow
        };
    }

    private string CreateMediaResponse_2(string fileName, IFormFile image)
    {
        return
            GenerateImageUrl(fileName);
    }
}

// Helper class for MIME types
public static class MimeTypeMap
{
    private static readonly Dictionary<string, string> _mimeTypeMap = new Dictionary<string, string>
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" }
    };

    public static string GetMimeType(string extension)
    {
        return _mimeTypeMap.TryGetValue(extension.ToLowerInvariant(), out var mimeType)
            ? mimeType
            : "application/octet-stream";
    }
}
