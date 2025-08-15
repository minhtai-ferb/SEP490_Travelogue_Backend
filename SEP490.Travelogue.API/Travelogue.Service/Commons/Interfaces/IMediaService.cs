using Microsoft.AspNetCore.Http;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.Commons.Interfaces;

public interface IMediaService
{
    Task<string> UploadImageAsync(IFormFile image);
    Task<List<string>> UploadMultipleImagesAsync(IEnumerable<IFormFile> images);
    Task<bool> DeleteImageAsync(string fileName);
    Task<bool> DeleteImagesAsync(List<string> fileNames);

    Task<bool> DeleteDocumentAsync(string fileName);
    Task<bool> DeleteDocumentsAsync(List<string> fileNames);
    Task<List<MediaResponse>> GetAllImagesAsync();
    Task<string> UploadDocumentAsync(IFormFile document);
    Task<List<string>> UploadMultipleDocumentsAsync(IEnumerable<IFormFile> documents);
}