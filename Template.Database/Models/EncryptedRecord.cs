namespace Template.Database.Models
{
    public class EncryptedRecord
    {
        public Guid Id { get; set; }

        public required string SensitiveData { get; set; }
    }
}