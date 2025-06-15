using AutoMapper;
using HushKeyCore.Data.Secret;
using HuskKeyInfra.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HushKeyService.Mapper
{
    public class EncryptedSecretProfile : Profile
    {
        public EncryptedSecretProfile() 
        {
            CreateMap<SecretModel, EncryptedSecretEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EncryptedSecretId ?? Guid.NewGuid().ToString("N")))
                .ForMember(dest => dest.EncryptedSecret, opt => opt.MapFrom(src => src.EncryptedSecret ?? string.Empty))
                .ForMember(dest => dest.EncryptionType, opt => opt.MapFrom(src => src.EncryptionType))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt.HasValue ? src.ExpiresAt.Value : (DateTime?)null));

            CreateMap<EncryptedSecretEntity, SecretModel>()
                .ForMember(dest => dest.EncryptedSecretId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EncryptedSecret, opt => opt.MapFrom(src => src.EncryptedSecret))
                .ForMember(dest => dest.EncryptionType, opt => opt.MapFrom(src => src.EncryptionType))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt.HasValue ? src.ExpiresAt.Value : (DateTime?)null));

        }
    }
}
