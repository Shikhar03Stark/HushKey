using Dapper;
using HushKeyCore.HushKeyLogger;
using HuskKeyInfra.Database.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskKeyInfra.Database.Repository.Postgres
{
    public class EncryptedSecretRepository : IEncryptedSecretRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IHushKeyLogger _logger;

        public EncryptedSecretRepository(IDbConnection dbConnection, IHushKeyLogger logger)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection), "Database connection cannot be null.");
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        public async Task CreateAsync(EncryptedSecretEntity encryptedSecretEntity)
        {

            try
            {
                var (sql, parameters) = GetCreateQueryAndParams(encryptedSecretEntity);

                var result = await _dbConnection.ExecuteScalarAsync<int>(sql, parameters);
                _logger.Debug($"Affected rows: {result}");
            }
            catch (Exception ex)
            {

                _logger.Error("An error occurred while creating EncryptedSecret.", ex);
                throw;
            }

        }

        public async Task DeleteByIdAsync(string id)
        {
            try
            {
                var (sql, parameters) = GetDeleteQueryAndParams(id);

                var result = await _dbConnection.ExecuteScalarAsync<int>(sql, parameters);
                _logger.Debug($"Affected rows: {result}");
            }
            catch (Exception ex)
            {
                _logger.Error("An error occured while deleting ExceryptedSecret", ex);
                throw;
            }
        }

        public async Task<EncryptedSecretEntity?> GetByIdAsync(string id)
        {
            try
            {
                var (sql, parameters) = GetGetByIdQueryAndParams(id);
                var result = await _dbConnection.QueryAsync<EncryptedSecretEntity>(sql, parameters);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while fetching EncryptedSecret by id", ex);
                throw;
            }
        }

        private static (string, Dictionary<string, object>) GetCreateQueryAndParams(EncryptedSecretEntity entity)
        {
            var sql = @"
            INSERT INTO ""EncryptedSecrets"" (""Id"", ""EncryptedSecret"", ""EncryptionType"", ""CreatedAt"", ""ExpiresAt"")
            VALUES (@Id, @EncryptedSecret, @EncryptionType, @CreatedAt, @ExpiresAt)
            ON CONFLICT (""Id"") DO NOTHING;";

            var parameters = new Dictionary<string, object>
            {
                { "Id", entity.Id },
                { "EncryptedSecret", entity.EncryptedSecret },
                { "EncryptionType", entity.EncryptionType },
                { "CreatedAt", entity.CreatedAt },
                { "ExpiresAt", entity.ExpiresAt ?? (object)DBNull.Value } // Handle nullable ExpiresAt
            };

            return (sql, parameters);
        }

        private static (string, Dictionary<string, object>) GetDeleteQueryAndParams(string id)
        {
            var sql = @"
            DELETE FROM ""EncryptedSecrets""
            WHERE ""Id"" = @Id;";
            var parameters = new Dictionary<string, object>
            {
                { "Id", id }
            };
            return (sql, parameters);
        }

        private static (string, Dictionary<string, object>) GetGetByIdQueryAndParams(string id)
        {
            var sql = @"
            SELECT * FROM ""EncryptedSecrets""
            WHERE ""Id"" = @Id;";
            var parameters = new Dictionary<string, object>
            {
                { "Id", id }
            };
            return (sql, parameters);

        }
    }
}
