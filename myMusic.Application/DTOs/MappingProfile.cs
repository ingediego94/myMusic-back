using AutoMapper;
using myMusic.Domain.Entities;

namespace myMusic.Application.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // JWT
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            // CreateMap<User, RegisterDto>();
        
        CreateMap<User, UserAuthResponseDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());
        // CreateMap<UserRegisterResponseDto, User>();

        CreateMap<Token, RefreshDto>()
            .ForMember(dest => dest.Token, src => src.MapFrom(s => s.RefreshToken));

        // CreateMap<UserAuthResponseDto, User>();
        // CreateMap<User, UserAuthResponseDto>();


        // CreateMap<>()
    }
}