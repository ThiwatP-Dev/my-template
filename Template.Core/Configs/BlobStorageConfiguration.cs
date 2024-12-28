namespace Template.Core.Configs;

public class BlobStorageConfiguration
{
    public const string Blob = "Blob";

    public string ConnectionString { get; set; } = string.Empty;

    public string ContainerName { get; set; } = string.Empty;
}