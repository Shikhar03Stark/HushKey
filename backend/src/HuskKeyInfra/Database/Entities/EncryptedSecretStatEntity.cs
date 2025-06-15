using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskKeyInfra.Database.Entities
{
    public class EncryptedSecretStatEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public required string EncryptedSecretId { get; set; } = string.Empty; // Reference to the EncryptedSecretEntity
        public int AccessCount { get; set; } = 0; // Number of times the secret has been accessed
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow; // Timestamp of the last access
    }
}
