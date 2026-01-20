using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Models;

public class Work
{
    [MaxLength(Constants.MaxNameLength)] public string Title { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxNameLength)] public string Company { get; set; } = string.Empty;
    
    public DateTime From { get; set; }
    
    public DateTime? To { get; set; }
    
    [MaxLength(Constants.MaxDescriptionLength)] public string Description { get; set; } = string.Empty;
}