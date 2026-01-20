using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Models;

public class Language
{
    [MaxLength(Constants.MaxNameLength)] public string Name { get; set; } = string.Empty;
    
    public int Level { get; set;}
}