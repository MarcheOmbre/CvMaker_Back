using System.ComponentModel.DataAnnotations;

namespace CvBuilderBack.Dtos;

public class GetAllCvsDto
{
    [Required] public int Id { get; init; }
    
    [Required] public string Name { get; init; } = string.Empty;
}