using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;

namespace Travelogue.Service.Commons.Implementations;

public class CloudinarySettings
{
    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
}

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<List<string>> UploadImagesAsync(List<IFormFile> files);
    //Task<string> UploadImageAsync(IFormFile file, string fileName);
    Task<List<string>> UploadVideoAsync(List<IFormFile> files);

    Task<bool> DeleteImageAsync(string imageUrl);
    Task<bool> DeleteImagesAsync(List<string> imageUrls);
}

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        try
        {
            if (file.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream())
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.Url.ToString();
            }
            throw new Exception("File is empty");
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

    //public async Task<string> UploadImageAsync(IFormFile file)
    //{
    //    using var stream = file.OpenReadStream();
    //    var uploadParams = new ImageUploadParams
    //    {
    //        File = new FileDescription(file.FileName, stream),
    //        Folder = "districts" 
    //    };
    //    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
    //    return uploadResult.SecureUrl.ToString();
    //}

    public async Task<List<string>> UploadImagesAsync(List<IFormFile> files)
    {
        try
        {
            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream())
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    uploadedUrls.Add(uploadResult.Url.ToString());
                }
            }

            return uploadedUrls;
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

    public async Task<List<string>> UploadVideoAsync(List<IFormFile> files)
    {
        var uploadedUrls = new List<string>();

        foreach (var file in files)
        {
            if (file.Length == 0)
                continue;

            var extension = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv" };

            if (!allowedExtensions.Contains(extension))
                throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, $"Định dạng video không hợp lệ: {extension}");

            await using var stream = file.OpenReadStream();

            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = false
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == HttpStatusCode.OK)
            {
                uploadedUrls.Add(uploadResult.SecureUrl.ToString());
            }
            else
            {
                throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.FAILED, $"Không thể upload video: {file.FileName}");
            }
        }

        return uploadedUrls;
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(imageUrl)) return false;

            var publicId = ExtractPublicId(imageUrl);
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
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

    public async Task<bool> DeleteImagesAsync(List<string> imageUrls)
    {
        try
        {
            if (imageUrls == null || imageUrls.Count == 0) return false;

            var results = new List<bool>();

            foreach (var imageUrl in imageUrls)
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    results.Add(false);
                    continue;
                }

                var publicId = ExtractPublicId(imageUrl);
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                results.Add(result.Result == "ok");
            }

            return results.All(r => r);
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

    private string ExtractPublicId(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var fileName = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
            return fileName;
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

    //public async Task<string> UploadImageAsync(IFormFile file, string fileName)
    //{
    //    var uploadParams = new ImageUploadParams
    //    {
    //        File = new FileDescription(file.FileName, file.OpenReadStream()),
    //        PublicId = Path.GetFileNameWithoutExtension(fileName), 
    //        Overwrite = true 
    //    };

    //    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
    //    return uploadResult.SecureUrl.ToString();
    //}

}
