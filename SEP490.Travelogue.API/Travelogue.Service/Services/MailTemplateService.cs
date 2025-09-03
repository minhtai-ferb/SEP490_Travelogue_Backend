using Microsoft.AspNetCore.Hosting;
using Travelogue.Repository.Bases.Exceptions;

namespace Travelogue.Service.Services;

public class MailTemplateService
{
    private readonly HttpClient _httpClient;
    private readonly IWebHostEnvironment _env;

    public MailTemplateService(HttpClient httpClient, IWebHostEnvironment env)
    {
        _httpClient = httpClient;
        _env = env;
    }

    public async Task<string> GetTemplateContentAsync(string pathOrUrl)
    {
        try
        {
            if (Uri.TryCreate(pathOrUrl, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }

            var fullPath = ResolvePath(pathOrUrl);
            return await File.ReadAllTextAsync(fullPath);
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

    private string ResolvePath(string path)
    {
        if (Path.IsPathRooted(path) && File.Exists(path)) return path;

        var candidates = new[]
        {
            Path.Combine(_env.ContentRootPath, path),                                              // gốc dự án
            Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), path),// wwwroot
            Path.Combine(AppContext.BaseDirectory, path),                                          // thư mục bin
        };

        var resolved = candidates.FirstOrDefault(File.Exists);
        if (resolved is null)
            throw new FileNotFoundException($"Không tìm thấy file template: {path}");

        return resolved;
    }
}
