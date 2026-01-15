using System.ComponentModel.DataAnnotations;

namespace CvBuilderBack.Dtos;

public class SetCvContentDto
{
    [Required] public int CvId { get; init; }
    
    [MaxLength(int.MaxValue)]
    public string Content { get; init; } = string.Empty;
}