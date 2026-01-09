using CvBuilderBack.Dtos;
using CvBuilderBack.Helpers;
using CvBuilderBack.Models;
using CvBuilderBack.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CvBuilderBack.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CvController(IUserRepository userRepository) : ControllerBase
{
    #region Gets
    
    [HttpGet("GetAll")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult GetAll()
    {
        if (!TokenHelper.CheckToken(User, userRepository, out var id))
            return Unauthorized();

        return Ok(userRepository.ExecuteStoreProcedure<GetAllCvsDto>($"{Constants.PublicSchema}.spCvGetAll",
            new Tuple<string, object>("user", id)));
    }

    [HttpGet("Get")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult Get(int id)
    {
        if (!TokenHelper.CheckToken(User, userRepository, out var userId))
            return Unauthorized();
        
        try
        {
            var result = userRepository.ExecuteStoreProcedure<Cv>($"{Constants.PublicSchema}.spCvGet",
                new Tuple<string, object>("user", userId),
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
        if (!TokenHelper.CheckToken(User, userRepository, out var userId))
            return Unauthorized();
        
        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.PublicSchema}.spCvCreate",
            new Tuple<string, object>("user", userId),
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
    
    [HttpPost("SetImage")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult SetImage(SetCvDto setCvDto)
    {
        if (!TokenHelper.CheckToken(User, userRepository, out var userId))
            return Unauthorized();
        
        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.PublicSchema}.spCvImageSet",
            new Tuple<string, object>("user", userId),
            new Tuple<string, object>("cv", setCvDto.CvId),
            new Tuple<string, object>("image", setCvDto.Content));
        
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
    public IActionResult SetContent(SetCvDto setCvDto)
    {
        if (!TokenHelper.CheckToken(User, userRepository, out var userId))
            return Unauthorized();
        
        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.PublicSchema}.spCvContentSet",
            new Tuple<string, object>("user", userId),
            new Tuple<string, object>("cv", setCvDto.CvId),
            new Tuple<string, object>("content", setCvDto.Content));
        
        var resultCode = result.Length > 0 ? result[0] : -1;
        return resultCode switch
        {
            1 => BadRequest("One of the parameters is null"),
            2 => BadRequest("CV does not exist"),
            0 => Ok("Content updated"),
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
        if (!TokenHelper.CheckToken(User, userRepository, out var userId))
            return Unauthorized();

        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.PublicSchema}.spCvDelete",
            new Tuple<string, object>("user", userId),
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