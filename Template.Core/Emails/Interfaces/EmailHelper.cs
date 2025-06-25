namespace Template.Core.Emails.Interfaces;

public abstract class EmailHelper
{
    protected readonly string _basePath = Path.Combine(AppContext.BaseDirectory, "Emails");

    public abstract Task SendWithTemplateAsync(string sendTo, string subject, string templateName, Guid requester);

    protected string LoadTemplate(string templateName)
    {
        var sharedStyles = File.ReadAllText(Path.Combine(_basePath, "Templates", "shared-styles.html"));
        var filePath = Path.Combine(_basePath, "Templates", templateName);
        if (!File.Exists(filePath))
        {
            return string.Empty;
        }

        var response = File.ReadAllText(filePath).Replace("<!-- styles-here -->", sharedStyles); ;

        return response;
    }
}