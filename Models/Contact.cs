using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Models;

public class Contact
{
    public int Type { get; set; }
    
    [MaxLength(Constants.MaxEmailLength)] public string Value { get; set; } = string.Empty;
}