using System.ComponentModel.DataAnnotations;

namespace HushKeyApi.Data.Secret
{
    public class SecretRequest
    {
        [Required]
        [MaxLength(512)]
        public string SecretText { get; set; }
        [Range(3600, 86400, ErrorMessage = "TTL must be between 3600 seconds (1 hour) and 86400 seconds (24 hours).")] // 1 hour to 24 days in seconds
        public int? TTL { get; set; } // Time to live in seconds, null means no expiration
    }
}
