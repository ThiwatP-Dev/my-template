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

    public EmailHelper(IOptions<MailjetConfiguration> configurationOptions)
    {
        _configuration = configurationOptions.Value;
        _client = new MailjetClient(_configuration.ApiKey, _configuration.ApiSecret);
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
}