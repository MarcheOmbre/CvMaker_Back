using System.ComponentModel.DataAnnotations;
using CvBuilderBack.Common;
using CvBuilderBack.Models;
using CvBuilderBack.Services.Interfaces;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CvBuilderBack.Dtos;

public class CvDto
{
    [Required] public int Id { get; init; }

    [MaxLength(Constants.MaxNameLength)] public string Name { get; init; } = string.Empty;

    [MaxLength(Constants.MaxUrlLength)] public string SystemLanguage { get; init; } = string.Empty;

    [MaxLength(Constants.MaxFileSize)] public string Image { get; init; } = string.Empty;

    [MaxLength(Constants.MaxNameLength)] public string Title { get; init; } = string.Empty;

    [MaxLength(Constants.MaxNameLength)] public string Profession { get; init; } = string.Empty;

    [MaxLength(Constants.MaxDescriptionLength)] public string AboutMe { get; init; } = string.Empty;

    [MaxLength(Constants.MaxItems)] public IList<Contact> Contacts { get; init; } = [];

    [MaxLength(Constants.MaxItems)] public IList<Link> Links { get; init; } = [];

    [MaxLength(Constants.MaxItems)] public IList<Work> Works { get; init; } = [];

    [MaxLength(Constants.MaxItems)] public IList<Education> Educations { get; init; } = [];

    [MaxLength(Constants.MaxItems)] public IList<Project> Projects { get; init; } = [];

    [MaxLength(Constants.MaxItems)] public IList<Language> Languages { get; init; } = [];

    [MaxLength(Constants.MaxItems)] public IList<Skill> Skills { get; init; } = [];

    [MaxLength(Constants.MaxItems)] public IList<Hobby> Hobbies { get; init; } = [];

    [MaxLength(int.MaxValue)] public string CustomHtml { get; init; } = string.Empty;

    [MaxLength(int.MaxValue)] public string CustomCss { get; init; } = string.Empty;


    [JsonConstructor]
    public CvDto()
    {
        
    }
    
    public CvDto(Cv cv, IHtmlSanitizerService htmlSanitizerService)
    {
        ArgumentNullException.ThrowIfNull(cv);
        
        Console.WriteLine("Is trying to convert");

        Id = cv.Id;
        SystemLanguage = htmlSanitizerService.Sanitize(cv.SystemLanguage);
        Name = htmlSanitizerService.Sanitize(cv.Name);
        Image = htmlSanitizerService.Sanitize(cv.Image);
        Title = htmlSanitizerService.Sanitize(cv.Title);
        Profession = htmlSanitizerService.Sanitize(cv.Profession);
        AboutMe = htmlSanitizerService.Sanitize(cv.AboutMe);
        
        var contacts = new List<Contact>();
        Console.WriteLine(cv.Contacts);
        if (!string.IsNullOrEmpty(cv.Contacts))
        {
            contacts.AddRange((JsonSerializer.Deserialize<List<Contact>>(cv.Contacts) ?? []).Select(contact => new Contact
            {
                Type = contact.Type,
                Value = htmlSanitizerService.Sanitize(contact.Value)
            }));
        }
        Contacts = contacts;

        var links = new List<Link>();
        if (!string.IsNullOrEmpty(cv.Links))
        {
            links.AddRange((JsonSerializer.Deserialize<List<Link>>(cv.Links) ?? []).Select(link => new Link
            {
                Name = htmlSanitizerService.Sanitize(link.Name),
                Url = htmlSanitizerService.Sanitize(link.Url)
            }));
        }
        Links = links;

        var works = new List<Work>();
        if (!string.IsNullOrEmpty(cv.Works))
        {
            works.AddRange((JsonSerializer.Deserialize<List<Work>>(cv.Works) ?? []).Select(work => new Work
            {
                Title = htmlSanitizerService.Sanitize(work.Title),
                Company = htmlSanitizerService.Sanitize(work.Company),
                From = work.From,
                To = work.To,
                Description = htmlSanitizerService.Sanitize(work.Description)
            }));
        }
        Works = works;

        var educations = new List<Education>();
        if (!string.IsNullOrEmpty(cv.Educations))
        {
            educations.AddRange((JsonSerializer.Deserialize<List<Education>>(cv.Educations) ?? [])
                .Select(education => new Education
                {
                    Title = htmlSanitizerService.Sanitize(education.Title), 
                    Date = education.Date
                }));
        }
        Educations = educations;

        var projects = new List<Project>();
        if (!string.IsNullOrEmpty(cv.Projects))
        {
            projects.AddRange((JsonSerializer.Deserialize<List<Project>>(cv.Projects) ?? []).Select(project => new Project
            {
                Title = htmlSanitizerService.Sanitize(project.Title),
                Date = project.Date,
                Description = htmlSanitizerService.Sanitize(project.Description)
            }));
        }
        Projects = projects;

        var languages = new List<Language>();
        if (!string.IsNullOrEmpty(cv.Languages))
        {
            languages.AddRange((JsonSerializer.Deserialize<List<Language>>(cv.Languages) ?? []).Select(language => new Language
            {
                Name = htmlSanitizerService.Sanitize(language.Name),
                Level = language.Level
            }));
        }
        Languages = languages;

        var skills = new List<Skill>();
        if (!string.IsNullOrEmpty(cv.Skills))
        {
            skills.AddRange((JsonSerializer.Deserialize<List<Skill>>(cv.Skills) ?? [])
                .Select(skill => new Skill
                {
                    Name = htmlSanitizerService.Sanitize(skill.Name),
                    Level = skill.Level
                }));
        }
        Skills = skills;

        var hobbies = new List<Hobby>();
        if (!string.IsNullOrEmpty(cv.Hobbies))
        {
            hobbies.AddRange((JsonSerializer.Deserialize<List<Skill>>(cv.Hobbies) ?? [])
                .Select(skill => new Hobby { Name = htmlSanitizerService.Sanitize(skill.Name) }));
        }
        Hobbies = hobbies;
        
        CustomHtml = htmlSanitizerService.Sanitize(cv.CustomHtml);
        CustomCss = htmlSanitizerService.Sanitize(cv.CustomCss);
    }
    
    
    public void InjectModifiedFieldsInto(Cv cv, IHtmlSanitizerService htmlSanitizerService)
    {
        ArgumentNullException.ThrowIfNull(cv);

        var systemLanguage = htmlSanitizerService.Sanitize(SystemLanguage);
        if (cv.SystemLanguage != systemLanguage) cv.SystemLanguage = systemLanguage;

        var name = htmlSanitizerService.Sanitize(Name);
        if (cv.Name != name) 
            cv.Name = name;

        var image = htmlSanitizerService.Sanitize(Image);
        if (cv.Image != image) 
            cv.Image = image;

        var title = htmlSanitizerService.Sanitize(Title);
        if (cv.Title != title) 
            cv.Title = title;

        var profession = htmlSanitizerService.Sanitize(Profession);
        if (cv.Profession != profession) 
            cv.Profession = profession;

        var aboutMe = htmlSanitizerService.Sanitize(AboutMe);
        if (cv.AboutMe != aboutMe) 
            cv.AboutMe = aboutMe;

        foreach (var contact in Contacts)
            contact.Value = htmlSanitizerService.Sanitize(contact.Value);
        var contacts = JsonSerializer.Serialize(Contacts);
        if (cv.Contacts != contacts) 
            cv.Contacts = contacts;

        foreach (var link in Links)
        {
            link.Name = htmlSanitizerService.Sanitize(link.Name);
            link.Url = htmlSanitizerService.Sanitize(link.Url);
        }

        var links = JsonSerializer.Serialize(Links);
        if (cv.Links != links) 
            cv.Links = links;

        foreach (var work in Works)
        {
            work.Title = htmlSanitizerService.Sanitize(work.Title);
            work.Company = htmlSanitizerService.Sanitize(work.Company);
            work.Description = htmlSanitizerService.Sanitize(work.Description);
        }

        var works = JsonSerializer.Serialize(Works);
        if (cv.Works != works) 
            cv.Works = works;

        foreach (var education in Educations)
            education.Title = htmlSanitizerService.Sanitize(education.Title);
        var educations = JsonSerializer.Serialize(Educations);
        if (cv.Educations != educations) 
            cv.Educations = educations;

        foreach (var project in Projects)
        {
            project.Title = htmlSanitizerService.Sanitize(project.Title);
            project.Description = htmlSanitizerService.Sanitize(project.Description);
        }

        var projects = JsonSerializer.Serialize(Projects);
        if (cv.Projects != projects) 
            cv.Projects = projects;

        foreach (var language in Languages)
            language.Name = htmlSanitizerService.Sanitize(language.Name);
        var languages = JsonSerializer.Serialize(Languages);
        if (cv.Languages != languages) 
            cv.Languages = languages;

        foreach (var skill in Skills)
            skill.Name = htmlSanitizerService.Sanitize(skill.Name);
        var skills = JsonSerializer.Serialize(Skills);
        if (cv.Skills != skills) 
            cv.Skills = skills;

        foreach (var hobby in Hobbies)
            hobby.Name = htmlSanitizerService.Sanitize(hobby.Name);
        var hobbies = JsonSerializer.Serialize(Hobbies);
        if (cv.Hobbies != hobbies) 
            cv.Hobbies = hobbies;

        var customHtml = htmlSanitizerService.Sanitize(CustomHtml);
        if (cv.CustomHtml != customHtml) 
            cv.CustomHtml = customHtml;

        var customCss = htmlSanitizerService.Sanitize(CustomCss);
        if (cv.CustomCss != customCss) 
            cv.CustomCss = customCss;
    }
}