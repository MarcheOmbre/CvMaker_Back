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
    private readonly IHtmlSanitizerService htmlSanitizerService;
    private readonly IUserRepository userRepository;
    private readonly IUserService userService;

    public CvController(IUserRepository userRepository, IHtmlSanitizerService htmlSanitizerService)
    {
        this.userRepository = userRepository;
        userService = new AspNetUserService(this);
        this.htmlSanitizerService = htmlSanitizerService;
    }
    
    
    #region Gets
    
    [HttpGet("GetAll")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult GetAll()
    {
        return Ok(userRepository.ExecuteStoreProcedure<GetAllCvsDto>($"{Constants.PublicSchema}.spCvGetAll",
            new Tuple<string, object>("user", userService.GetId())));
    }

    [HttpGet("Get")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult Get(int id)
    {
        try
        {
            var result = userRepository.ExecuteStoreProcedure<Cv>($"{Constants.PublicSchema}.spCvGet",
                new Tuple<string, object>("user", userService.GetId()),
                new Tuple<string, object>("cv", id));
            
            return Ok(result[0]);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
    
    #endregion
    
    
    #region Put

    [HttpPut("Create")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult Create(string name)
    {
        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.PublicSchema}.spCvCreate",
            new Tuple<string, object>("user", userService.GetId()),
            new Tuple<string, object>("name", name));
        
        var resultCode = result.Length > 0 ? result[0] : -1;
        return resultCode switch
        {
            1 => BadRequest("One of the parameters is null"),
            0 => Ok("CV created"),
            _ => BadRequest("An unexpected error occured")
        };
    }
    
    #endregion
    
    
    #region Posts
    
    [HttpPost("SetName")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult SetName(SetCvNameDto setCvNameDto)
    {
        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.PublicSchema}.spCvNameSet",
            new Tuple<string, object>("user", userService.GetId()),
            new Tuple<string, object>("cv", setCvNameDto.CvId),
            new Tuple<string, object>("name", htmlSanitizerService.Sanitize(setCvNameDto.Name)));
        
        var resultCode = result.Length > 0 ? result[0] : -1;
        return resultCode switch
        {
            1 => BadRequest("One of the parameters is null"),
            2 => BadRequest("The cv does not exist"),
            0 => Ok("Name updated"),
            _ => BadRequest("An unexpected error occured")
        };
    }
    
    [HttpPost("SetImage")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult SetImage(SetCvContentDto setCvContentDto)
    {
        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.PublicSchema}.spCvImageSet",
            new Tuple<string, object>("user", userService.GetId()),
            new Tuple<string, object>("cv", setCvContentDto.CvId),
            new Tuple<string, object>("image", htmlSanitizerService.Sanitize(setCvContentDto.Content)));
        
        var resultCode = result.Length > 0 ? result[0] : -1;
        return resultCode switch
        {
            1 => BadRequest("One of the parameters is null"),
            2 => BadRequest("The cv does not exist"),
            0 => Ok("Image updated"),
            _ => BadRequest("An unexpected error occured")
        };
    }
    
    [HttpPost("SetContent")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult SetContent(SetCvContentDto setCvContentDto)
    {
        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.PublicSchema}.spCvContentSet",
            new Tuple<string, object>("user", userService.GetId()),
            new Tuple<string, object>("cv", setCvContentDto.CvId),
            new Tuple<string, object>("content", htmlSanitizerService.Sanitize(setCvContentDto.Content)));
        
        var resultCode = result.Length > 0 ? result[0] : -1;
        return resultCode switch
        {
            1 => BadRequest("One of the parameters is null"),
            2 => BadRequest("CV does not exist"),
            0 => Ok("Content updated"),
            _ => BadRequest("An unexpected error occured")
        };
    }

    [HttpPost("SetCustomCss")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult SetCustomCss(SetCvContentDto setCvContentDto)
    {
        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.PublicSchema}.spCvCustomCssSet",
            new Tuple<string, object>("user", userService.GetId()),
            new Tuple<string, object>("cv", setCvContentDto.CvId),
            new Tuple<string, object>("content", htmlSanitizerService.Sanitize(setCvContentDto.Content)));
        
        var resultCode = result.Length > 0 ? result[0] : -1;
        return resultCode switch
        {
            1 => BadRequest("One of the parameters is null"),
            2 => BadRequest("CV does not exist"),
            0 => Ok("Html updated"),
            _ => BadRequest("An unexpected error occured")
        };
    }
    
    #endregion


    #region Deletes
    
    [HttpDelete("Delete")]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult Delete(int id)
    {
        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.PublicSchema}.spCvDelete",
            new Tuple<string, object>("user", userService.GetId()),
            new Tuple<string, object>("cv", id));
        
        var resultCode = result.Length > 0 ? result[0] : -1;
        return resultCode switch
        {
            1 => BadRequest("One of the parameters is null"),
            2 => BadRequest("Cv does not exist"),
            0 => Ok("CV deleted"),
            _ => BadRequest("An unexpected error occured")
        };
    }

    #endregion
}