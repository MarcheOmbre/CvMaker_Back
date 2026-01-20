using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Dtos;

public class GetAllCvsDto
{
    [Required] public int Id { get; init; }
    
    [Required][MaxLength(Constants.MaxNameLength)] public string Name { get; init; } = string.Empty;
}