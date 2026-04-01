using AutoMapper;
using myMusic.Domain.Entities;

namespace myMusic.Application.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // JWT
        // 1. Register: Mapeo básico
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        
        CreateMap<User, UserRegisterResponseDto>();
            
        // 2. Auth: El DTO que combina User y Token
        CreateMap<User, UserAuthResponseDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());
        
        // 3. Refresh: Si se necesita mapear la entidad Token a un DTO de refresco
        CreateMap<Token, RefreshDto>()
            .ForMember(dest => dest.Token, opt => opt.Ignore()) // El Access Token se genera en el Service
            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken));
        


        // CreateMap<>()
    }
}