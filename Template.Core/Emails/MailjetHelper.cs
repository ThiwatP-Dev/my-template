using Mailjet.Client;
using Template.Core.Emails.Interfaces;
using Template.Core.Configs;
using Microsoft.Extensions.Options;
using Mailjet.Client.TransactionalEmails;
using Template.Database;
using Template.Database.Models;
using Template.Database.Enums;

namespace Template.Core.Emails;

public class MailjetHelper : EmailHelper
{
    private readonly IMailjetClient _client;
    private readonly MailjetConfiguration _configuration;
    private readonly DatabaseContext _dbContext;

    public MailjetHelper(IOptions<MailjetConfiguration> configurationOptions,
                         IMailjetClient client,
                         DatabaseContext dbContext)
    {
        _configuration = configurationOptions.Value;
        _client = new MailjetClient(_configuration.ApiKey, _configuration.ApiSecret);
        _client = client;
        _dbContext = dbContext;
    }

    public override async Task SendWithTemplateAsync(string sendTo, string subject, string templateName, Guid requester)
    {
        var body = LoadTemplate(templateName);

        await SendAsync(sendTo, subject, body, GetAttachments(), requester);
    }

    private async Task SendAsync(string sendTo, string subject, string body, IEnumerable<Attachment>? attachments, Guid requester)
    {
        var log = new EmailLog
        {
            ToEmail = sendTo,
            FromEmail = _configuration.SenderEmail,
            Subject = subject,
            Body = body,
            Status = EmailLogStatus.PENDING,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = requester,
            Attachments = attachments is null
                          || !attachments.Any() ? null
                                                : string.Join(",", attachments.Select(x => x.Filename))
        };

        await _dbContext.EmailLogs.AddAsync(log);
        await _dbContext.SaveChangesAsync();

        try
        {
            var builder = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(_configuration.SenderEmail, _configuration.SenderName))
                .WithSubject(subject)
                .WithHtmlPart(body)
                .WithTo(new SendContact(sendTo));

            if (attachments is not null && attachments.Any())
            {
                builder.WithAttachments(attachments);
            }

            // invoke API to send email
            var email = builder.Build();

            var result = await _client.SendTransactionalEmailAsync(email);

            log.Status = EmailLogStatus.SENT;
            log.ProviderMessageId = result.Messages.FirstOrDefault()?.To.FirstOrDefault()?.MessageUUID;
            log.SentAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            log.Status = EmailLogStatus.FAILED;
            log.ErrorMessage = ex.Message;
        }

        log.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }
    
    protected List<Attachment> GetAttachments()
    {
        var response = new List<Attachment>
        {
            new("image.png", "image/png", Convert.ToBase64String(File.ReadAllBytes(Path.Combine(_basePath, "Images", "image.png"))), "image.png")
        };
        
        return response;
    }
}