namespace HushKeyApi.Models
{
    public class SecretResponse
    {
        public Uri UILink { get; set; }
        public Uri ServiceShareableLink { get; set; }
        public DateTime? ExpiresAt { get; set; } // Nullable DateTime to indicate no expiration if null
    }
}
