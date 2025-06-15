using HuskKeyInfra.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskKeyInfra.Database.Repository
{
    public interface IEncryptedSecretRepository
    {
        public Task CreateAsync(EncryptedSecretEntity encryptedSecretEntity);
        public Task<EncryptedSecretEntity?> GetByIdAsync(string id);
        public Task DeleteByIdAsync(string id);
    }
}
