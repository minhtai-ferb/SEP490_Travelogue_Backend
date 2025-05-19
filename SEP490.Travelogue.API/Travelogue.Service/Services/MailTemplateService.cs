using Travelogue.Repository.Bases.Exceptions;

namespace Travelogue.Service.Services;
public class MailTemplateService
{
    private readonly HttpClient _httpClient;

    public MailTemplateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetTemplateContentAsync(string templateUrl)
    {
        try
        {
            var response = await _httpClient.GetAsync(templateUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
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
}
