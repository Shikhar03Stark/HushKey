
namespace HushKeyCore.Data.Secret
{
    public class SecretModel
    {
        public string EncryptedSecretId { get; set; }
        public string? EncryptedSecret { get; set; }
        public EncryptionType EncryptionType { get; set; }
        public string Key { get; set; }
    }

    public enum EncryptionType
    {
        SYMMETRIC,
        ASYMMETRIC,
    }
}
