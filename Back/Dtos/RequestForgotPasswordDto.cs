using System.ComponentModel.DataAnnotations;

namespace CvBuilderBack.Dtos;

public class RequestForgotPasswordDto
{
    [EmailAddress] public string Email { get; init; } = string.Empty;
    [Required] public string PagePath { get; init; } = string.Empty;
}