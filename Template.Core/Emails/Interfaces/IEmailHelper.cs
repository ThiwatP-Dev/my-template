namespace Template.Core.Emails.Interfaces;

public interface IEmailHelper
{
    Task<bool> SendAsync(string sendTo, string body);
}