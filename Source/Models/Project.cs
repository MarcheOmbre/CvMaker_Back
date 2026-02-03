using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Models;

public class Project
{
    [MaxLength(Constants.MaxNameLength)] public string Title { get; set; } = string.Empty;

    public DateTime Date { get; set; }
    
    [MaxLength(Constants.MaxDescriptionLength)] public string Description { get; set; } = string.Empty;
}