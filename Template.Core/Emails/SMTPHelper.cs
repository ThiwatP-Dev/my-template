using System.Net;
using System.Net.Mail;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Template.Core.Configs;
using Template.Core.Emails.Interfaces;
using Template.Database;
using Template.Database.Enums;
using Template.Database.Models;

namespace Template.Core.Emails;

public class SMTPHelper(IOptions<SMTPConfiguration> configurationOptions,
                        DatabaseContext dbContext) : EmailHelper
{
    private readonly SMTPConfiguration _configuration = configurationOptions.Value;

    public override async Task SendWithTemplateAsync(string sendTo, string subject, string templateName, Guid requester)
    {
        var body = LoadTemplate(templateName);

        await SendAsync(sendTo, subject, body, requester);
    }

    private async Task SendAsync(string sendTo, string subject, string body, Guid requester)
    {
        var log = new EmailLog
        {
            ToEmail = sendTo,
            FromEmail = _configuration.SenderEmail,
            Subject = subject,
            Body = body,
            Status = EmailLogStatus.PENDING,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = requester
        };

        await dbContext.EmailLogs.AddAsync(log);
        await dbContext.SaveChangesAsync();

        var email = new MailMessage
        {
            From = new MailAddress(_configuration.SenderEmail, _configuration.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        email.To.Add(sendTo);

        try
        {
            using var client = new SmtpClient(_configuration.Server)
            {
                Port = _configuration.Port,
                Credentials = new NetworkCredential(_configuration.Username, _configuration.Password),
                EnableSsl = true
            };

            client.Send(email);
        }
        catch (Exception ex)
        {
            log.Status = EmailLogStatus.FAILED;
            log.ErrorMessage = ex.Message;
        }

        log.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
    }
}