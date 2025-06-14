using System.ComponentModel.DataAnnotations;

namespace HushKeyApi.Data.Secret
{
    public class SecretRequest
    {
        [Required]
        public string SecretText { get; set; }
        public int? TTL { get; set; } // Time to live in seconds, null means no expiration
    }
}
