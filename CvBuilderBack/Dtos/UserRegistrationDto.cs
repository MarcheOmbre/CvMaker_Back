namespace CvBuilderBack.Dtos;

public class UserRegistrationDto
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string PasswordConfirmation { get; init; } = string.Empty;
}