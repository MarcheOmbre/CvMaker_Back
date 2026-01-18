using System.ComponentModel.DataAnnotations;

namespace CvBuilderBack.Dtos;

public class SetCvNameDto
{
    [Required] public int CvId { get; init; }
    
    [MaxLength(50)]
    [Required] public string Name { get; init; } = string.Empty;
}