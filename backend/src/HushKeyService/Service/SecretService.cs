using HushKeyCore.Data.Secret;
using HushKeyCore.HushKeyLogger;
using System.Security.Cryptography;
using System.Text;

namespace HushKeyService.Service
{
    public class SecretService
    {
        private readonly IHushKeyLogger _logger;

        private static readonly Dictionary<string, string> _secretStore = new Dictionary<string, string>();

        public SecretService(IHushKeyLogger logger)
        {
            _logger = logger;
        }

        private string GenerateConsolidatedKey(byte[] nonce, byte[] key, byte[] tag)
        {
            return $"{Convert.ToBase64String(nonce)}.{Convert.ToBase64String(key)}.{Convert.ToBase64String(tag)}";
        }

        public async Task<SecretModel> GenerateSymmetricEncryptSecretAsync(string secretText)
        {
            var key = RandomNumberGenerator.GetBytes(32); // 256 bits key
            var nonce = RandomNumberGenerator.GetBytes(12); // 96 bits nonce
            var tag = new byte[16]; // 128 bits tag
            var secretId = Guid.NewGuid().ToString("N"); // Generate a unique ID for the secret

            try
            {
                _logger.Info($"Generating encrypted secret of length {secretText.Length}");
                using var aes = new AesGcm(key, tag.Length);
                var encryptedSecret = new byte[secretText.Length];

                aes.Encrypt(nonce, Encoding.UTF8.GetBytes(secretText), encryptedSecret, tag);

                var encryptedSecretBase64 = Convert.ToBase64String(encryptedSecret);
                _logger.Debug($"Encrypted secret: {encryptedSecretBase64}");

                _secretStore[secretId] = encryptedSecretBase64;
                var convertedKey = GenerateConsolidatedKey(nonce, key, tag);
                _logger.Info("Secret encrypted successfully using symmetric encryption.");

                return await Task.FromResult(new SecretModel { 
                    EncryptedSecretId = secretId,
                    EncryptionType = EncryptionType.SYMMETRIC,
                    Key = convertedKey,
                });
            } catch (Exception ex)
            {
                _logger.Error("Error generating symmetric key or nonce", ex);
                throw;
            }
        }

        public async Task<SecretModel?> GetSymmetricEncryptedSecretFromId(string secretId)
        {
            if (_secretStore.TryGetValue(secretId, out var encryptedSecretBase64))
            {
                _logger.Info($"Retrieved encrypted secret for ID: {secretId}");
                return await Task.FromResult(new SecretModel
                {
                    EncryptedSecretId = secretId,
                    EncryptedSecret = encryptedSecretBase64,
                    EncryptionType = EncryptionType.SYMMETRIC,
                    Key = string.Empty // Key should not be returned, client already has it.
                });
            }
            else
            {
                _logger.Warning($"No secret found for ID: {secretId}");
                return null;
            }
        }
    }
}
