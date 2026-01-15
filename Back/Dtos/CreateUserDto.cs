using System.ComponentModel.DataAnnotations;

namespace CvBuilderBack.Dtos;

public class CreateUserDto
{
    [Required] public int Id { get; init; }

    [EmailAddress] public string Email { get; init; } = string.Empty;
}