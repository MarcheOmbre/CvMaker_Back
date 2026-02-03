using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Models;

public class Link
{
    [MaxLength(Constants.MaxNameLength)]public string Name { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxUrlLength)]public string Url { get; set; } = string.Empty;
}