using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;

namespace CvBuilderBack.Models;

public class Cv
{
    [Required] public int Id { get; init; }
    
    [MaxLength(Constants.MaxUrlLength)] public string SystemLanguage { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxNameLength)] public string Name { get; set; } = string.Empty;

    [MaxLength(Constants.MaxFileSize)] public string Image { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxNameLength)] public string Title { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxNameLength)] public string Profession { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxDescriptionLength)] public string AboutMe { get; set; } = string.Empty;

    [MaxLength(int.MaxValue)] public string Contacts { get; set; } = string.Empty;
    
    [MaxLength(int.MaxValue)] public string Links { get; set; } = string.Empty;
    
    [MaxLength(int.MaxValue)] public string Works { get; set; } = string.Empty;
    
    [MaxLength(4000)] public string Educations { get; set; } = string.Empty;
    
    [MaxLength(int.MaxValue)] public string Projects { get; set; } = string.Empty;
    
    [MaxLength(3000)] public string Languages { get; set; } = string.Empty;
    
    [MaxLength(3000)] public string Skills { get; set; } = string.Empty;
    
    [MaxLength(3000)] public string Hobbies { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxFileSize)] public string CustomHtml { get; set; } = string.Empty;
    
    [MaxLength(Constants.MaxFileSize)] public string CustomCss { get; set; } = string.Empty;

    
    public static bool SetName(Cv cv, string name)
    {
        if(cv.Name == name)
            return false;
        
        cv.Name = name;
        return true;
    }
    
    public static Cv Duplicate(Cv cv)
    {
        ArgumentNullException.ThrowIfNull(cv);

        return new Cv
        {
            SystemLanguage = cv.SystemLanguage,
            Name = cv.Name + " (Copy)",
            Image = cv.Image,
            Title = cv.Title,
            Profession = cv.Profession,
            AboutMe = cv.AboutMe,
            Contacts = cv.Contacts,
            Links = cv.Links,
            Works = cv.Works,
            Educations = cv.Educations,
            Projects = cv.Projects,
            Languages = cv.Languages,
            Skills = cv.Skills,
            Hobbies = cv.Hobbies,
            CustomHtml = cv.CustomHtml,
            CustomCss = cv.CustomCss
        };
    }
}