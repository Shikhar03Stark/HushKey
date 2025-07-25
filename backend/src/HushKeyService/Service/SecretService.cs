using AutoMapper;
using HushKeyCore.Data.Secret;
using HushKeyCore.HushKeyLogger;
using HuskKeyInfra.Database.Entities;
using HuskKeyInfra.Database.Repository;
using System.Security.Cryptography;
using System.Text;
using EncryptionType = HushKeyCore.Data.Secret.EncryptionType;

namespace HushKeyService.Service
{
    public class SecretService
    {
        private readonly IHushKeyLogger _logger;
        private readonly IMapper _mapper;
        private readonly IEncryptedSecretRepository _encryptedSecretRepository;
        private readonly IEncryptedSecretStatRepository _encryptedSecretStatRepository;

        public SecretService(IHushKeyLogger logger, IMapper mapper, IEncryptedSecretRepository encryptedSecretRepository, IEncryptedSecretStatRepository encryptedSecretStatRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _encryptedSecretRepository = encryptedSecretRepository;
            _encryptedSecretStatRepository = encryptedSecretStatRepository;
        }

        private string GenerateConsolidatedKey(byte[] nonce, byte[] key, byte[] tag)
        {
            return $"{Convert.ToBase64String(nonce)}.{Convert.ToBase64String(key)}.{Convert.ToBase64String(tag)}";
        }

        public async Task<SecretModel> GenerateSymmetricEncryptSecretAsync(string secretText, int ttlInSeconds = 86400, bool burnAfterRead = false)
        {
            var key = RandomNumberGenerator.GetBytes(32); // 256 bits key
            var nonce = RandomNumberGenerator.GetBytes(12); // 96 bits nonce
            var tag = new byte[16]; // 128 bits tag
            var secretId = Guid.NewGuid().ToString("N"); // Generate a unique ID for the secret
            var expirationTime = DateTime.UtcNow.AddSeconds(ttlInSeconds);

            try
            {
                var secretModel = EncryptSecretWithAesGcm(secretText, key, nonce, tag, secretId, expirationTime);

                secretModel.BurnAfterRead = burnAfterRead;

                await SaveSecret(secretModel);
                await SaveStatForSecret(secretModel);

                return secretModel;


            }
            catch (Exception ex)
            {
                _logger.Error("Error generating symmetric key or nonce", ex);
                throw;
            }
        }

        private async Task SaveStatForSecret(SecretModel secretModel)
        {
            var statEntity = new EncryptedSecretStatEntity
            {
                EncryptedSecretId = secretModel.EncryptedSecretId,
                AccessCount = 0, // Initialize access count to 0
                LastAccessedAt = DateTime.UtcNow // Set last accessed time to now
            };
            _logger.Info($"Saving secret stat for secret ID: {secretModel.EncryptedSecretId}");
            try
            {
                await _encryptedSecretStatRepository.CreateAsync(statEntity);
                _logger.Debug($"Secret stat for secret ID: {secretModel.EncryptedSecretId} saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error saving secret stat for secret ID: {secretModel.EncryptedSecretId}", ex);
                throw;
            }
        }

        private SecretModel EncryptSecretWithAesGcm(string secretText, byte[] key, byte[] nonce, byte[] tag, string secretId, DateTime expirationTime)
        {
            _logger.Info($"Generating encrypted secret of length {secretText.Length}");
            using var aes = new AesGcm(key, tag.Length);
            var encryptedSecret = new byte[Encoding.UTF8.GetBytes(secretText).Length];

            aes.Encrypt(nonce, Encoding.UTF8.GetBytes(secretText), encryptedSecret, tag);

            var encryptedSecretBase64 = Convert.ToBase64String(encryptedSecret);
            _logger.Debug($"Encrypted secret: {encryptedSecretBase64}");

            var convertedKey = GenerateConsolidatedKey(nonce, key, tag);
            _logger.Info("Secret encrypted successfully using symmetric encryption.");


            return new SecretModel
            {
                EncryptedSecretId = secretId,
                EncryptedSecret = encryptedSecretBase64,
                EncryptionType = EncryptionType.SYMMETRIC,
                Key = convertedKey,
                ExpiresAt = expirationTime,
            };
        }

        public async Task<SecretModel?> GetSymmetricEncryptedSecretFromId(string secretId)
        {
            try
            {
                var entity = await _encryptedSecretRepository.GetByIdAsync(secretId);
                if (entity == null)
                {
                    _logger.Warning($"Secret with ID {secretId} not found/expired/burned.");
                    return null;
                }
                if (entity.BurnAfterRead)
                {
                    _logger.Info($"Secret with ID {secretId} is marked for burn after read. Deleting it from the database.");
                    await _encryptedSecretRepository.DeleteByIdAsync(secretId);
                }
                else
                {
                    await IncrementAccessCount(secretId);
                }
                var secretModel = _mapper.Map<SecretModel>(entity);
                return secretModel;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching entity from database", ex);
                throw;
            }
        }

        private async Task IncrementAccessCount(string secretId)
        {
            _logger.Info($"Incrementing access count for secret ID: {secretId}");
            try
            {
                await _encryptedSecretStatRepository.IncrementVisitCount(secretId);
                _logger.Debug($"Access count for secret ID: {secretId} incremented successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error incrementing access count for secret ID: {secretId}", ex);
                throw;
            }
        }

        private async Task SaveSecret(SecretModel secretModel)
        {
            _logger.Info($"Saving secret with ID: {secretModel.EncryptedSecretId} to repository.");
            var entity = _mapper.Map<EncryptedSecretEntity>(secretModel);
            entity.CreatedAt = DateTime.UtcNow;
            entity.ExpiresAt = secretModel.ExpiresAt ?? DateTime.UtcNow.AddDays(1); // Default to 1 day if not specified

            _logger.Debug($"Entity to save: {entity}");
            await _encryptedSecretRepository.CreateAsync(entity);
        }
    }
}
