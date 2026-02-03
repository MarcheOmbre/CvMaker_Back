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
    
    private bool HasSpace(int userId) => userRepository.GetAll<UserCvJoin>(u => u.UserId == userId).Count < Constants.MaxCvCount;

    #region Gets

    [HttpGet("GetAll")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult GetAll()
    {
        var userCvId = userRepository.GetAll<UserCvJoin>(u => u.UserId == userService.GetId()).Select(x => x.CvId);
        return Ok(userRepository.GetAll<Cv>(cv => userCvId.Contains(cv.Id)).Select(x => new GetAllCvsDto {Id = x.Id, Name = x.Name}));
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
        
        return Ok(new CvDto(cv, htmlSanitizerService));
    }

    #endregion


    #region Put

    [HttpPut("Create")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult Create(string name)
    {
        var userId = userService.GetId();
        
        if(!HasSpace(userId))
            return BadRequest("You have reached the maximum number of cvs");
        
        var cv = new Cv { Name = name };
        if (!userRepository.Add(cv) || 
            !userRepository.SaveChanges() || 
            !userRepository.Add(new UserCvJoin {UserId = userId, CvId = cv.Id}) || 
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
        
        if(!userRepository.TryGetById<Cv>(setCvNameDto.CvId, out var cv) || cv is null)
            return BadRequest("Cv not found");
        
        if(!Cv.SetName(cv, setCvNameDto.Name))
            return Ok("Name already set");

        if (!userRepository.SaveChanges())
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
        
        if(!userRepository.TryGetById<Cv>(cvDto.Id, out var cv) || cv is null)
            return BadRequest("Cv not found");
        
        cvDto.InjectModifiedFieldsInto(cv, htmlSanitizerService);
        
        if(userRepository.GetModifiedColumns(cvDto.Id, cv).Length <= 0)
            return Ok("No changes");
        
        if(!userRepository.SaveChanges())
            return BadRequest("An error occured while saving changes");
        
        return Ok("Updated");
    }

    [HttpPut("Duplicate")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult Duplicate(int id)
    {
        var userId = userService.GetId();
        
        if (!HasRight(userId, id))
            return Unauthorized();
        
        if(!userRepository.TryGetById<Cv>(id, out var cv))
            return BadRequest("Cv not found");
        
        if(!HasSpace(userId))
            return BadRequest("You have reached the maximum number of cvs");
        
        // Create the cv
        var duplicateCv = Cv.Duplicate(cv);
        
        if(!userRepository.Add(duplicateCv) || !userRepository.SaveChanges())
            return BadRequest("An error occured while duplicating the cv");
        
        // Link it to the user
        if(!userRepository.Add(new UserCvJoin {UserId = userId, CvId = duplicateCv.Id}) || !userRepository.SaveChanges())
            return BadRequest("An error occured while linking the cv to the user");
        
        return Ok("Duplicated");
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