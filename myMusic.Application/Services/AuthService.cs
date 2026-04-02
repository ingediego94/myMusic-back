using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
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
        // 1. Obtener el token de la DB con su usuario
        var savedToken = await _tokenRepository.GetTokenWithUserAsync(refreshDto.RefreshToken);

        if (savedToken == null || savedToken.RefreshTokenExpire <= DateTime.UtcNow)
            throw new SecurityException("Refresh Token inválido o expirado.");
        
        // 2. Validar que el Access Token pertenezca a este usuario (aunque esté expirado)
        var principal = GetPrincipalFromExpiredToken(refreshDto.Token);
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || int.Parse(userIdClaim) != savedToken.UserId)
            throw new SecurityException("Token inconsistente.");
        
        // 3. Opcional: Borrar el refresh token viejo para "rotarlo"
        await _tokenRepository.DeleteAsync(savedToken.RefreshToken!);

        return await GenerateTokensAndSaveAsync(savedToken.User);
    }

    
    // REVOKE:
    public async Task<bool> RevokeAsync(RevokeTokenDto revokeTokenDto)
    {
        return await _tokenRepository.DeleteAsync(revokeTokenDto.RefreshToken);
    }
    
    
    // ----------------------------------------------------------------
    // METODOS PRIVADOS DE APOYO
    private async Task<UserAuthResponseDto> GenerateTokensAndSaveAsync(User user)
    {
        // Generar JWT:
        var jwtToken = CreateJwtToken(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        
        // Generar Refresh Token
        var refreshTokenValue = GenerateRefreshTokenString();
        
        // Guardar en la tabla Tokens
        var tokenEntity = new Token
        {
            UserId = user.Id,
            RefreshToken = refreshTokenValue,
            RefreshTokenExpire = DateTime.UtcNow.AddDays(_refreshTokenDays)
        };
        await _tokenRepository.AddAsync(tokenEntity);
        
        //Mapear respuesta:
        var response = _mapper.Map<UserAuthResponseDto>(user);
        response.Token = accessToken;
        response.RefreshToken = refreshTokenValue;

        return response;
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

    
    // REFRESH TOKEN:
    private string GenerateRefreshTokenString()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    
    // EXPIRE TOKEN:
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
            ValidateLifetime = false // no validar tiempo aqui ¡
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityException("Token no válido.");

        return principal;
    }
}