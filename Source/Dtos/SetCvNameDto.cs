using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Dtos;

public class SetCvNameDto
{
    [Required] public int CvId { get; init; }
    
    [Required] [MaxLength(Constants.MaxNameLength)] public string Name { get; init; } = string.Empty;
}