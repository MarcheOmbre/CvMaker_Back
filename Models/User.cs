using System.ComponentModel.DataAnnotations;

namespace CvBuilderBack.Models;

public class User
{
    public int Id { get; init; }
    
    [MaxLength(50)]
    public string Email { get; init; } = string.Empty;
    
    public byte[] PasswordHash { get; init; } = [];
    
    public byte[] PasswordSalt { get; init; } = [];
}