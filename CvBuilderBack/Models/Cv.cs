using System.ComponentModel.DataAnnotations;

namespace CvBuilderBack.Models;

public class Cv
{
    public int Id { get; init; }
    
    [MaxLength(50)]
    public string Name { get; init; } = string.Empty;
    
    [MaxLength(int.MaxValue)]
    public string? Image { get; init; }  = string.Empty;
    
    [MaxLength(int.MaxValue)]
    public string? Content { get; init; }  = string.Empty;
}