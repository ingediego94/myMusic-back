using System.ComponentModel.DataAnnotations;
using myMusic.Domain.Enums;

namespace myMusic.Application.DTOs;


// Register:
public class RegisterDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = string.Empty;
}


// Response for register:
public class UserRegisterResponseDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string LastName { get; set; }
    
    public string Email { get; set; }
    public Role Role { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


// Login:
public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}


// Login response Dto:
public class UserAuthResponseDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    
    public Role Role { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


// To refresh:
public class RefreshDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
    
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}


// Revoke Token:
public class RevokeTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
