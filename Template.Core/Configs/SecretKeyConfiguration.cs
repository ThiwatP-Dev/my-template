namespace Template.Core.Configs;

public class SecretKeyConfiguration
{
    public const string ConfigurationSettings = "ClientSecretAuthenticationSetting";

    public string Key { get; set; } = string.Empty;
}