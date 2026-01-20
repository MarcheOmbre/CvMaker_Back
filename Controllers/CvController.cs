using System.Text.Json;
using CvBuilderBack.Common;
using CvBuilderBack.Dtos;
using CvBuilderBack.Models;
using CvBuilderBack.Repositories;
using CvBuilderBack.Services;
using CvBuilderBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CvBuilderBack.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CvController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IUserService userService;
    private readonly IHtmlSanitizerService htmlSanitizerService;

    public CvController(IUserRepository userRepository, IHtmlSanitizerService htmlSanitizerService)
    {
        this.userRepository = userRepository;
        userService = new AspNetUserService(this);
        this.htmlSanitizerService = htmlSanitizerService;
    }

    private bool HasRight(int userId, int cvId) =>
        userRepository.Get<UserCvJoin>(u => u.UserId == userId && u.CvId == cvId) != null;


    #region Gets

    [HttpGet("GetAll")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult GetAll()
    {
        var userCvId = userRepository.GetAll<UserCvJoin>(u => u.UserId == userService.GetId()).Select(x => x.CvId);
        return Ok(userRepository.GetAll<Cv>(cv => userCvId.Contains(cv.Id)));
    }

    [HttpGet("Get")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult Get(int id)
    {
        if (!HasRight(userService.GetId(), id))
            return Unauthorized();

        if (!userRepository.TryGetById<Cv>(id, out var cv) || cv is null)
            return NotFound("Cv not found");

        // Format cv
        var cvDto = new CvDto
        {
            Id = cv.Id,
            Image = htmlSanitizerService.Sanitize(cv.Image),
            Title = htmlSanitizerService.Sanitize(cv.Title),
            Profession = htmlSanitizerService.Sanitize(cv.Profession),
            AboutMe = htmlSanitizerService.Sanitize(cv.AboutMe)
        };

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
        cvDto.Contacts = contacts;

        var links = new List<Link>();
        if (!string.IsNullOrEmpty(cv.Links))
        {
            links.AddRange((JsonSerializer.Deserialize<List<Link>>(cv.Links) ?? []).Select(link => new Link
            {
                Name = htmlSanitizerService.Sanitize(link.Name),
                Url = htmlSanitizerService.Sanitize(link.Url)
            }));
        }
        cvDto.Links = links;

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
        cvDto.Works = works;

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
        cvDto.Educations = educations;

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
        cvDto.Projects = projects;

        var languages = new List<Language>();
        if (!string.IsNullOrEmpty(cv.Languages))
        {
            languages.AddRange((JsonSerializer.Deserialize<List<Language>>(cv.Languages) ?? []).Select(language => new Language
            {
                Name = htmlSanitizerService.Sanitize(language.Name),
                Level = language.Level
            }));
        }
        cvDto.Languages = languages;

        var skills = new List<Skill>();
        if (!string.IsNullOrEmpty(cv.Skills))
        {
            skills.AddRange((JsonSerializer.Deserialize<List<Skill>>(cv.Skills) ?? [])
                .Select(skill => new Skill { Name = htmlSanitizerService.Sanitize(skill.Name) }));
        }
        cvDto.Skills = skills;

        var hobbies = new List<Hobby>();
        if (!string.IsNullOrEmpty(cv.Hobbies))
        {
            hobbies.AddRange((JsonSerializer.Deserialize<List<Skill>>(cv.Hobbies) ?? [])
                .Select(skill => new Hobby { Name = htmlSanitizerService.Sanitize(skill.Name) }));
        }
        cvDto.Hobbies = hobbies;
        cvDto.CustomHtml = htmlSanitizerService.Sanitize(cv.CustomHtml);
        cvDto.CustomCss = htmlSanitizerService.Sanitize(cv.CustomCss);
        
        return Ok(cvDto);
    }

    #endregion


    #region Put

    [HttpPut("Create")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult Create(string name)
    {
        // Get cv count
        var userCvs = userRepository.GetAll<UserCvJoin>(x => x.UserId == userService.GetId());
        if(userCvs.Count >= Constants.MaxCvCount)
            return BadRequest("You have reached the maximum number of cvs");
        
        var cv = new Cv { Name = name };
        if (!userRepository.Add(cv) || 
            !userRepository.SaveChanges() || 
            !userRepository.Add(new UserCvJoin {UserId = userService.GetId(), CvId = cv.Id}) || 
            !userRepository.SaveChanges())
            return BadRequest("An error occured while creating the cv");

        return Ok("Cv created");
    }

    #endregion


    #region Posts

    [HttpPost("SetName")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult SetName(SetCvNameDto setCvNameDto)
    {
        if (!HasRight(userService.GetId(), setCvNameDto.CvId))
            return Unauthorized();

        if (!userRepository.Update<Cv>(setCvNameDto.CvId, cv => cv.Name = setCvNameDto.Name)
            || !userRepository.SaveChanges())
            return BadRequest("An error occured while updating the cv");

        return Ok("Updated");
    }

    [HttpPost("Modify")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult Modify(CvDto cvDto)
    {
        if (!HasRight(userService.GetId(), cvDto.Id))
            return Unauthorized();
        
        var updateSucceed = userRepository.Update<Cv>(cvDto.Id, cv =>
        {
            cv.SystemLanguage = htmlSanitizerService.Sanitize(cvDto.SystemLanguage);
            cv.Image = htmlSanitizerService.Sanitize(cvDto.Image);
            cv.Title = htmlSanitizerService.Sanitize(cvDto.Title);
            Console.WriteLine(cvDto.Title + " " + cv.Title);
            cv.Profession = htmlSanitizerService.Sanitize(cvDto.Profession);
            cv.AboutMe = htmlSanitizerService.Sanitize(cvDto.AboutMe);

            foreach (var contact in cvDto.Contacts) 
                contact.Value = htmlSanitizerService.Sanitize(contact.Value);
            cv.Contacts = JsonSerializer.Serialize(cvDto.Contacts);

            foreach (var link in cvDto.Links)
            {
                link.Name = htmlSanitizerService.Sanitize(link.Name);
                link.Url = htmlSanitizerService.Sanitize(link.Url);
            }
            cv.Links = JsonSerializer.Serialize(cvDto.Links);

            foreach (var work in cvDto.Works)
            {
                work.Title = htmlSanitizerService.Sanitize(work.Title);
                work.Company = htmlSanitizerService.Sanitize(work.Company);
                work.Description = htmlSanitizerService.Sanitize(work.Description);
            }
            cv.Works = JsonSerializer.Serialize(cvDto.Works);
            
            foreach (var education in cvDto.Educations) 
                education.Title = htmlSanitizerService.Sanitize(education.Title);
            cv.Educations = JsonSerializer.Serialize(cvDto.Educations);

            foreach (var project in cvDto.Projects)
            {
                project.Title = htmlSanitizerService.Sanitize(project.Title);
                project.Description = htmlSanitizerService.Sanitize(project.Description);
            }
            cv.Projects = JsonSerializer.Serialize(cvDto.Projects);
            
            foreach (var language in cvDto.Languages)
                language.Name = htmlSanitizerService.Sanitize(language.Name);
            cv.Languages = JsonSerializer.Serialize(cvDto.Languages);
            
            foreach (var skill in cvDto.Skills) 
                skill.Name = htmlSanitizerService.Sanitize(skill.Name);
            cv.Skills = JsonSerializer.Serialize(cvDto.Skills);
            
            foreach (var hobby in cvDto.Hobbies) 
                hobby.Name = htmlSanitizerService.Sanitize(hobby.Name);
            cv.Hobbies = JsonSerializer.Serialize(cvDto.Hobbies);
            
            cv.CustomHtml = htmlSanitizerService.Sanitize(cvDto.CustomHtml);
            cv.CustomCss = htmlSanitizerService.Sanitize(cvDto.CustomCss);
        });
        
        if (!updateSucceed || !userRepository.SaveChanges())
            return BadRequest("An error occured while updating the cv");

        return Ok("Updated");
    }

    #endregion


    #region Deletes

    [HttpDelete("Delete")]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult Delete(int id)
    {
        var userId = userService.GetId();
        if (!HasRight(userId, id))
            return Unauthorized();

        // Delete join table entry
        var cvJoinId = userRepository.Get<UserCvJoin>(x => x.UserId == userId && x.CvId == id)?.Id;
        if (cvJoinId == null || 
            !userRepository.Remove<UserCvJoin>(cvJoinId.Value) || 
            !userRepository.SaveChanges() || 
            !userRepository.Remove<Cv>(id) ||
            !userRepository.SaveChanges())
            return BadRequest("An error occured while deleting the cv join table entry");

        return Ok("CV deleted");
    }

    #endregion
}