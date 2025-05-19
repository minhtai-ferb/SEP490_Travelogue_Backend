using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.Services;

namespace Travelogue.Service.Commons.Implementations;

public interface IEmailService
{
    Task<bool> SendEmailAsync(IEnumerable<string> toList, string subject, string body);
    Task<bool> SendEmailWithTemplateAsync(IEnumerable<string> toList, string subject, string templatePath, object model);
}

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _senderEmail;
    private readonly ILogger<EmailService> _logger;
    private readonly HttpClient _httpClient;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger, HttpClient httpClient)
    {
        _senderEmail = configuration["EmailSettings:Sender"]
            ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Email Sender is not configured.");
        var password = configuration["EmailSettings:Password"];
        var host = configuration["EmailSettings:Host"];
        var port = int.Parse(configuration["EmailSettings:Port"]
            ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Email port is not configured."));

        _smtpClient = new SmtpClient(host, port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_senderEmail, password)
        };

        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<bool> SendEmailAsync(IEnumerable<string> toList, string subject, string body)
    {
        try
        {
            foreach (var to in toList)
            {
                var mailMessage = new MailMessage(_senderEmail, to, subject, body);
                await _smtpClient.SendMailAsync(mailMessage);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email.");
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<bool> SendEmailWithTemplateAsync(IEnumerable<string> toList, string subject, string templateUrl, object model)
    {
        try
        {
            var mailTemplateService = new MailTemplateService(_httpClient);
            var templateContent = await mailTemplateService.GetTemplateContentAsync(templateUrl);

            var body = RenderTemplate(templateContent, model);

            foreach (var to in toList)
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await _smtpClient.SendMailAsync(mailMessage);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with template.");
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    private string RenderTemplate(string template, object model)
    {
        foreach (var property in model.GetType().GetProperties())
        {
            var placeholder = $"{{{{ {property.Name} }}}}";
            template = template.Replace(placeholder, property.GetValue(model)?.ToString());
        }
        return template;
    }
}
