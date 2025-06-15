using Dapper;
using HushKeyCore.HushKeyLogger;
using HuskKeyInfra.Database.Entities;
using System.Data;

namespace HuskKeyInfra.Database.Repository.Postgres
{
    public class EncryptedSecretStatRepository : IEncryptedSecretStatRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IHushKeyLogger _logger;

        public EncryptedSecretStatRepository(IDbConnection dbConnection, IHushKeyLogger logger)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection), "Database connection cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        public async Task CreateAsync(EncryptedSecretStatEntity encryptedSecretStatEntity)
        {
            try
            {
                var (sql, parameters) = GetCreateQueryAndParams(encryptedSecretStatEntity);
                var result = await _dbConnection.ExecuteScalarAsync<int>(sql, parameters);
                _logger.Debug($"Affected rows: {result}");
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while creating EncryptedSecretStat.", ex);
                throw;
            }
        }

        public async Task<EncryptedSecretStatEntity?> GetBySecretIdAsync(string secretId)
        {
            try
            {
                var (sql, parameters) = GetGetBySecretIdQueryAndParams(secretId);
                var result = await _dbConnection.QueryAsync<EncryptedSecretStatEntity>(sql, parameters);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while fetching EncryptedSecretStat by secretId.", ex);
                throw;
            }
        }

        public async Task IncrementVisitCount(string secretId)
        {
            try
            {
                var (sql, parameters) = GetIncrementVisitCountQueryAndParams(secretId);
                var result = await _dbConnection.ExecuteScalarAsync<int>(sql, parameters);
                _logger.Debug($"Affected rows: {result}");
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while incrementing visit count.", ex);
                throw;
            }
        }

        private static (string, Dictionary<string, object>) GetCreateQueryAndParams(EncryptedSecretStatEntity entity)
        {
            var sql = @"
            INSERT INTO ""EncryptedSecretStats"" (""Id"", ""EncryptedSecretId"", ""AccessCount"", ""LastAccessedAt"")
            VALUES (@Id, @EncryptedSecretId, @AccessCount, @LastAccessedAt)
            ON CONFLICT (""EncryptedSecretId"") DO NOTHING;";

            var parameters = new Dictionary<string, object>
            {
                { "Id", entity.Id },
                { "EncryptedSecretId", entity.EncryptedSecretId },
                { "AccessCount", entity.AccessCount },
                { "LastAccessedAt", entity.LastAccessedAt }
            };

            return (sql, parameters);
        }

        private static (string, Dictionary<string, object>) GetGetBySecretIdQueryAndParams(string secretId)
        {
            var sql = @"
            SELECT * FROM ""EncryptedSecretStats""
            WHERE ""EncryptedSecretId"" = @EncryptedSecretId;";
            var parameters = new Dictionary<string, object>
            {
                { "EncryptedSecretId", secretId }
            };
            return (sql, parameters);
        }

        private static (string, Dictionary<string, object>) GetIncrementVisitCountQueryAndParams(string secretId)
        {
            var sql = @"
            UPDATE ""EncryptedSecretStats""
            SET ""AccessCount"" = ""AccessCount"" + 1,
                ""LastAccessedAt"" = @LastAccessedAt
            WHERE ""EncryptedSecretId"" = @EncryptedSecretId
            RETURNING 1;";

            var parameters = new Dictionary<string, object>
            {
                { "EncryptedSecretId", secretId },
                { "LastAccessedAt", DateTime.UtcNow }
            };
            return (sql, parameters);
        }
    }
}
