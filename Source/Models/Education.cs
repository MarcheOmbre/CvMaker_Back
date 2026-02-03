using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Models;

public class Education
{
    [MaxLength(Constants.MaxNameLength)] public string Title { get; set; } = string.Empty;
    
    public DateTime Date { get; set; }
}