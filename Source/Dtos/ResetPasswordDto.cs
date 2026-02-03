using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Dtos;

public class ResetPasswordDto
{
    [Required][MaxLength(Constants.MaxPasswordLength)] public string Password { get; init; } = string.Empty;

    [Required][MaxLength(Constants.MaxPasswordLength)] public string PasswordConfirmation { get; init; } = string.Empty;
}