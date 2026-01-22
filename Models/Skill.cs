using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Models;

public class Skill
{
    [MaxLength(Constants.MaxNameLength)] public string Name { get; set; } = string.Empty;
    
    public int Level { get; set;}
}