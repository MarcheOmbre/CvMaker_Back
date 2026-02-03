using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Dtos;

public class LogUserDto
{
    [Required][EmailAddress][MaxLength(Constants.MaxEmailLength)] public string Email { get; init; } = string.Empty;

    [Required][MaxLength(Constants.MaxPasswordLength)] public string Password { get; init; } = string.Empty;
}