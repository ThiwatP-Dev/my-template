namespace Template.Core.Configs;

public class AzureADConfiguration
{
    public const string AzureAD = "AzureAD";

    public string Instance { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string TenantId { get; set; } = string.Empty;
}