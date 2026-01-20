using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Models;

public class User
{
    public int Id { get; init; }
    
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [MaxLength(Constants.MaxEmailLength)] public string Email { get; init; } = string.Empty;
    
    public byte[] PasswordHash { get; init; } = [];
    
    public byte[] PasswordSalt { get; init; } = [];
}