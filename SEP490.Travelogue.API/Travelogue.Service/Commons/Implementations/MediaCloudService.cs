using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Commons.Implementations;
public class MediaCloudService : IMediaCloudService
{
    public Task<bool> DeleteFileAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FileExistsAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, string>> GetFileMetadataAsync(string key)
    {
        throw new NotImplementedException();
    }

    // dung dể test
    public string GetFileUrl(string fileKey)
    {
        return $"https://link-anh--vi-du-sau-khi-viet-service/{fileKey}";
    }

    public string GetImageUrl(string key)
    {
        throw new NotImplementedException();
    }

    public Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        throw new NotImplementedException();
    }
}
