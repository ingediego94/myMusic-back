using System.ComponentModel.DataAnnotations;

namespace myMusic.Application.DTOs;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public string Email { get; set; } = string.Empty;
}