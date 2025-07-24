namespace Travelogue.Service.Commons.Interfaces;
public interface IMediaCloudService
{
    /// <summary>
    /// Chuyển đổi một key (tên tệp hoặc mã nhận diện) thành URL đầy đủ trên AWS S3.
    /// </summary>
    /// <param name="key">Mã nhận diện hoặc tên file trong S3.</param>
    /// <returns>URL đầy đủ có thể truy cập của hình ảnh.</returns>
    string GetImageUrl(string key);

    /// <summary>
    /// Upload file lên AWS S3 và trả về key của nó.
    /// </summary>
    /// <param name="fileStream">Dữ liệu của file.</param>
    /// <param name="fileName">Tên file.</param>
    /// <param name="contentType">Loại nội dung (MIME type).</param>
    /// <returns>Key của file đã được lưu trên S3.</returns>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);

    /// <summary>
    /// Xóa một file khỏi AWS S3 theo key.
    /// </summary>
    /// <param name="key">Mã nhận diện hoặc tên file trong S3.</param>
    /// <returns>True nếu xóa thành công, ngược lại False.</returns>
    Task<bool> DeleteFileAsync(string key);

    /// <summary>
    /// Lấy thông tin metadata của file trên S3.
    /// </summary>
    /// <param name="key">Mã nhận diện hoặc tên file trong S3.</param>
    /// <returns>Thông tin metadata của file.</returns>
    Task<Dictionary<string, string>> GetFileMetadataAsync(string key);

    /// <summary>
    /// Kiểm tra xem một file có tồn tại trên S3 hay không.
    /// </summary>
    /// <param name="key">Mã nhận diện hoặc tên file trong S3.</param>
    /// <returns>True nếu file tồn tại, ngược lại False.</returns>
    Task<bool> FileExistsAsync(string key);
    string GetFileUrl(string fileKey);
}
