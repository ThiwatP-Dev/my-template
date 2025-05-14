using Mailjet.Client;
using Template.Core.Emails.Interfaces;
using Template.Core.Configs;
using Microsoft.Extensions.Options;
using Mailjet.Client.TransactionalEmails;

namespace Template.Core.Emails;

public class EmailHelper : IEmailHelper
{
    private readonly IMailjetClient _client;
    private readonly MailjetConfiguration _configuration;
    private readonly string _basePath;

    public EmailHelper(IOptions<MailjetConfiguration> configurationOptions)
    {
        _configuration = configurationOptions.Value;
        _client = new MailjetClient(_configuration.ApiKey, _configuration.ApiSecret);
        _basePath = Path.Combine(AppContext.BaseDirectory, "Emails");
    }

    public async Task<bool> SendWithTemplateAsync(string sendTo, string templateName)
    {
        var body = ReadTemplate(templateName);

        var builder = new TransactionalEmailBuilder()
            .WithFrom(new SendContact(_configuration.SenderEmail, _configuration.SenderName))
            .WithSubject("Test Subject")
            .WithHtmlPart(body)
            .WithTo(new SendContact(sendTo))
            .WithInlinedAttachments(GetAttachments());

        // invoke API to send email
        var email = builder.Build();

        var response = await _client.SendTransactionalEmailAsync(email);
        
        if (response.Messages.Length == 0 || response.Messages.First().Status != "success")
        {
            return false;
        }

        return true;
    }

    public async Task<bool> SendAsync(string sendTo, string body)
    {
        var builder = new TransactionalEmailBuilder()
            .WithFrom(new SendContact(_configuration.SenderEmail, _configuration.SenderName))
            .WithSubject("Test Subject")
            .WithHtmlPart(body)
            .WithTo(new SendContact(sendTo));

        // invoke API to send email
        var email = builder.Build();

        var response = await _client.SendTransactionalEmailAsync(email);
        
        if (response.Messages.Length == 0 || response.Messages.First().Status != "success")
        {
            return false;
        }

        return true;
    }

    private string ReadTemplate(string templateName)
    {
        var sharedStyles = File.ReadAllText(Path.Combine(_basePath, "Templates", "shared-styles.html"));
        var filePath = Path.Combine(_basePath, "Templates", templateName);
        if (!File.Exists(filePath))
        {
            return string.Empty;
        }

        var response = File.ReadAllText(filePath).Replace("<!-- styles-here -->", sharedStyles);;

        return response;
    }

    private List<Attachment> GetAttachments()
    {
        var response = new List<Attachment>
        {
            new("image.png", "image/png", Convert.ToBase64String(File.ReadAllBytes(Path.Combine(_basePath, "Images", "image.png"))), "image.png")
        };
        
        return response;
    }
}