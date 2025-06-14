namespace HushKeyApi.Models
{
    public class SecretResponse
    {
        public Uri ShareableLink { get; set; }
        public DateTime? ExpiresAt { get; set; } // Nullable DateTime to indicate no expiration if null
    }
}
