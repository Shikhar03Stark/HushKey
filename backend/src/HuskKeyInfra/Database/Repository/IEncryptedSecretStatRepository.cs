using HuskKeyInfra.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskKeyInfra.Database.Repository
{
    public interface IEncryptedSecretStatRepository
    {
        public Task CreateAsync(EncryptedSecretStatEntity encryptedSecretStatEntity);
        public Task IncrementVisitCount(string secretId);
        public Task<EncryptedSecretStatEntity?> GetBySecretIdAsync(string secretId);
    }
}
