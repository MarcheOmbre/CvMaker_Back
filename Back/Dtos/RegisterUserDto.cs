using System.ComponentModel.DataAnnotations;

namespace CvBuilderBack.Dtos;

public class RegisterUserDto
{
    [EmailAddress] public string Email { get; init; } = string.Empty;

    [Required] public string Password { get; init; } = string.Empty;

    [Required] public string PasswordConfirmation { get; init; } = string.Empty;
}