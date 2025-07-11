using Microsoft.AspNetCore.Http;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.Commons.Interfaces;

public interface IMediaService
{
    Task<string> UploadImageAsync(IFormFile image);
    Task<List<string>> UploadMultipleImagesAsync(IEnumerable<IFormFile> images);
    Task<bool> DeleteImageAsync(string fileName);
    Task<bool> DeleteImagesAsync(List<string> fileNames);
    Task<List<MediaResponse>> GetAllImagesAsync();
}