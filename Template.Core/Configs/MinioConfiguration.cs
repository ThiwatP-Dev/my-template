namespace Template.Core.Configs;

public class MinioConfiguration
{
    public const string Minio = "Minio";

    public string Endpoint { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string BucketName { get; set; } = string.Empty;

    public string PublicEndpoint { get; set; } = string.Empty;

    public bool Secure { get; set; }
}