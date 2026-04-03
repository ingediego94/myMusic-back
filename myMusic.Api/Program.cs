using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using myMusic.Application.DTOs;
using myMusic.Application.Interfaces;
using myMusic.Application.Services;
using myMusic.Domain.Interfaces;
using myMusic.Infrastructure.Extensions;
using myMusic.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Inyección de Infraestructura (DB Context y Extensiones)
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Registro de AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 3. Registro de Repositorios (Capa de Infraestructura)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<IPlaylistRespository, PlaylistRepository>();
builder.Services.AddScoped<IPlaylistSongRepository, PlaylistSongRepository>();
builder.Services.AddScoped<IPlayHistoryRepository, PlayHistoryRepository>();

// 4. Registro de Servicios (Capa de Aplicación)
builder.Services.AddScoped<IAuthService, AuthService>();
// Aquí irás agregando IUserService, ISongService, etc., a medida que los crees.

// 5. Controladores y Explorador de Endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 6. Configuración de Seguridad (JWT)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        RoleClaimType = ClaimTypes.Role,
        ClockSkew = TimeSpan.Zero // Para que el token expire exactamente cuando debe
    };
});

// 7. Configuración de Swagger con Botón de Autorización
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "myMusic API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese su token JWT. Ejemplo: Bearer eyJhbGci..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// 8. Configuración de CORS (Abierto para cualquier Front-end en desarrollo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// --- CONFIGURACIÓN DEL PIPELINE DE MIDDLEWARES ---

// A. Swagger (Habilitado en desarrollo y producción para pruebas)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "myMusic API v1");
    c.RoutePrefix = string.Empty; // Swagger aparecerá en la raíz: http://localhost:PORT/
});

// B. Seguridad y Redirección
app.UseHttpsRedirection();
app.UseCors("DevCorsPolicy");

// C. Autenticación y Autorización (ORDEN CRÍTICO)
app.UseAuthentication(); 
app.UseAuthorization();

// D. Mapeo de Controladores
app.MapControllers();

app.Run();