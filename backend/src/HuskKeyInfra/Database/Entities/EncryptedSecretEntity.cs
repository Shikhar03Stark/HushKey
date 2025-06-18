using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskKeyInfra.Database.Entities
{
    public class EncryptedSecretEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string EncryptedSecret { get; set; } = string.Empty;
        public EncryptionType EncryptionType { get; set; } = EncryptionType.Aes256Gcm;
        public bool BurnAfterRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; } = null; // Nullable to indicate no expiration
    }

    public enum EncryptionType
    {
        Aes256Gcm = 1,
        Aes256Cbc,
        ChaCha20Poly1305,
    }
}
