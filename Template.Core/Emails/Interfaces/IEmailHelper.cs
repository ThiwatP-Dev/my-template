namespace Template.Core.Emails.Interfaces;

public interface IEmailHelper
{
    Task<bool> SendWithTemplateAsync(string sendTo, string templateName);
    Task<bool> SendAsync(string sendTo, string body);
}