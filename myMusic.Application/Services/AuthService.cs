using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using myMusic.Application.DTOs;
using myMusic.Application.Interfaces;
using myMusic.Domain.Entities;
using myMusic.Domain.Interfaces;

namespace myMusic.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    // Time Configuration (duration):
    private readonly int _jwtMinutes = 40;
    private readonly int _refreshTokenDays = 7;
    
    // Constructor:
    public AuthService(
        IUserRepository userRepository, 
        ITokenRepository tokenRepository,
        IMapper mapper,
        IConfiguration config
        )
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _mapper = mapper;
        _config = config;
    }
    
    // -----------------------------------------------------------------
    
    // REGISTER:
    public async Task<UserRegisterResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (registerDto == null) throw new ArgumentNullException(nameof(registerDto));

        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
            throw new InvalidOperationException($"El email '{registerDto.Email}' ya está registrado.");

        var user = _mapper.Map<User>(registerDto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
        user.IsActive = true;
        
        await _userRepository.CreateAsync(user);
        return _mapper.Map<UserRegisterResponseDto>(user);
    }

    
    // LOGIN:
    public async Task<UserAuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        if (!user.IsActive)
            throw new InvalidOperationException("La cuenta esta desactivada.");

        return await GenerateTokensAndSaveAsync(user);
    }

    
    // REFRESH:
    public async Task<UserAuthResponseDto> RefreshAsync(RefreshDto refreshDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RevokeAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }
    
    
    
    // METODOS PRIVADOS DE APOYO
    private async Task<UserAuthResponseDto> GenerateTokensAndSaveAsync(User user)
    {
        // Generar JWT:
        var jwtToken = CreateJwtToken(user);
    }

    
    // GENERATE TOKEN: jwt
    private JwtSecurityToken CreateJwtToken(User user)
    {
        var secretKey = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            throw new Exception("La clave JWT debe tener al menos 32 caracteres.");
        
        // 1. Key:
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        
        // 2. Algorithm:
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. Claims:
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // 4. Token:
        return new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtMinutes),
            signingCredentials: credentials
        );
    }
}