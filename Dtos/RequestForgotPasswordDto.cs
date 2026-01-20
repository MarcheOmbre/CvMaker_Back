using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Dtos;

public class RequestForgotPasswordDto
{
    [Required][EmailAddress][MaxLength(Constants.MaxEmailLength)] public string Email { get; init; } = string.Empty;
    [Required][MaxLength(Constants.MaxUrlLength)] public string PagePath { get; init; } = string.Empty;
}