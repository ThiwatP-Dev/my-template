namespace Template.Core.Emails.Interfaces;

public interface IEmailHelper
{
    Task SendWithTemplateAsync(string sendTo, string subject, string templateName);
}