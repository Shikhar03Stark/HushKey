
namespace HushKeyCore.Data.Secret
{
    public class SecretModel
    {
        public string EncryptedSecretId { get; set; }
        public string? EncryptedSecret { get; set; }
        public EncryptionType EncryptionType { get; set; }
        public string Key { get; set; }
        public bool BurnAfterRead { get; set; } = false;
        public DateTime? ExpiresAt { get; set; } // Nullable DateTime to indicate no expiration if null
    }

    public enum EncryptionType
    {
        SYMMETRIC = 1,
        ASYMMETRIC,
    }
}
