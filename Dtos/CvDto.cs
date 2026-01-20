using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;
using CvBuilderBack.Models;

namespace CvBuilderBack.Dtos;

public class CvDto
{
    [Required] public int Id { get; init; }
    
    [MaxLength(Constants.MaxUrlLength)] public string SystemLanguage { get; set; } = string.Empty;

    [MaxLength(Constants.MaxFileSize)] public string Image { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxNameLength)] public string Title { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxNameLength)] public string Profession { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxDescriptionLength)] public string AboutMe { get; set; } = string.Empty;

    [MaxLength(Constants.MaxItems)] public IList<Contact> Contacts { get; set; } = [];
    
    [MaxLength(Constants.MaxItems)] public IList<Link> Links { get; set; } = [];
    
    [MaxLength(Constants.MaxItems)] public IList<Work> Works { get; set; } = [];
    
    [MaxLength(Constants.MaxItems)] public IList<Education> Educations { get; set; } = [];
    
    [MaxLength(Constants.MaxItems)] public IList<Project> Projects { get; set; } = [];
    
    [MaxLength(Constants.MaxItems)] public IList<Language> Languages { get; set; } = [];
    
    [MaxLength(Constants.MaxItems)] public IList<Skill> Skills { get; set; } = [];
    
    [MaxLength(Constants.MaxItems)] public IList<Hobby> Hobbies { get; set; } = [];
    
    [MaxLength(int.MaxValue)] public string CustomHtml { get; set; } = string.Empty;
    
    [MaxLength(int.MaxValue)] public string CustomCss { get; set; } = string.Empty;
}