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
public class AuthentificationController : ControllerBase
{
    private static readonly TimeSpan LoginTokenTimeSpan = new(1, 0, 0, 0);
    private static readonly TimeSpan PasswordForgotTokenTimeSpan = new(0, 0, 5, 0);

    private readonly IConfiguration configuration;
    private readonly IUserRepository userRepository;
    private readonly IEmailService emailService = new SmtpEmailService();
    private readonly IPasswordService passwordService = new Pbkdf2PasswordService();
    private readonly ITokenService tokenService = new JwtTokenService();
    private readonly IUserService userService;

    // Add constructor injection
    public AuthentificationController(IConfiguration configuration, IUserRepository userRepository)
    {
        this.configuration = configuration;
        this.userRepository = userRepository;
        userService = new AspNetUserService(this);
    }
    
    
    #region Gets

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        return Ok(tokenService.CreateToken(configuration, userService.GetId(), LoginTokenTimeSpan));
    }

    #endregion

    #region Posts

    /// <reponse code="200">User registered</reponse>
    /// <reponse code="400">Bad request: check the error message</reponse>
    [AllowAnonymous]
    [HttpPost("Register")]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult Register(RegisterUserDto registerUserDto)
    {
        if (registerUserDto.Password != registerUserDto.PasswordConfirmation)
            return BadRequest("Passwords don't match");

        var salt = passwordService.GenerateSalt();
        var passwordHash = passwordService.GetPasswordHash(configuration, registerUserDto.Password, salt);

        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.AuthentificationSchema}.spUserAdd",
            new Tuple<string, object>("email", registerUserDto.Email),
            new Tuple<string, object>("passwordHash", passwordHash),
            new Tuple<string, object>("passwordSalt", salt));

        var resultCode = result.Length > 0 ? result[0] : -1;
        return resultCode switch
        {
            1 => BadRequest("One of the parameters is null"),
            2 => BadRequest("Email already registered"),
            0 => Ok("User created"),
            _ => BadRequest("An unexpected error occured when subscribing the user")
        };
    }

    /// <reponse code="200">User logged</reponse>
    /// <reponse code="400">Bad request: check the error message</reponse>
    [AllowAnonymous]
    [HttpPost("Log")]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult Log(LogUserDto logUserDto)
    {
        var userAuthentification = userRepository.Get<User>(x => x.Email == logUserDto.Email);
        if (userAuthentification != null)
        {
            var passwordHash = passwordService.GetPasswordHash(configuration, logUserDto.Password,
                userAuthentification.PasswordSalt);

            if (passwordHash.SequenceEqual(userAuthentification.PasswordHash))
                return Ok(tokenService.CreateToken(configuration, userAuthentification.Id, LoginTokenTimeSpan));
        }

        return BadRequest("User or password incorrect");
    }

    [AllowAnonymous]
    [HttpPost("ForgotPassword")]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult ForgotPassword(RequestForgotPasswordDto requestForgotPasswordDto)
    {
        var userAuthentification = userRepository.Get<User>(x => x.Email == requestForgotPasswordDto.Email);
        if (userAuthentification != null)
        {
            var token = tokenService.CreateToken(configuration, userAuthentification.Id, PasswordForgotTokenTimeSpan);
            
            // Send mail
            var resetLink = $"{requestForgotPasswordDto.PagePath}?token={token}";
            emailService.SendRetrievePasswordEmail(configuration, requestForgotPasswordDto.Email, resetLink);
        }

        return Ok("If the email exists, an email has been sent");
    }
    
    [HttpPost("ResetPassword")]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        if(resetPasswordDto.Password != resetPasswordDto.PasswordConfirmation)
            return BadRequest("Passwords don't match");
        
        var salt = passwordService.GenerateSalt();
        var passwordHash = passwordService.GetPasswordHash(configuration, resetPasswordDto.Password, salt);

        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.AuthentificationSchema}.spUserPasswordReset",
            new Tuple<string, object>("user", userService.GetId()),
            new Tuple<string, object>("passwordHash", passwordHash),
            new Tuple<string, object>("passwordSalt", salt));

        var resultCode = result.Length > 0 ? result[0] : -1;
        return resultCode switch
        {
            1 => BadRequest("One of the parameters is null"),
            0 => Ok("Password reset"),
            _ => BadRequest("An unexpected error occured when subscribing the user")
        };
    }

    #endregion
    
    #region Deletes

    /// <reponse code="200">User edited</reponse>
    /// <reponse code="400">Bad request: One of the fields not valid</reponse>
    [HttpDelete("Delete")]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult Delete()
    {
        var result = userRepository.ExecuteStoreProcedure<int>($"{Constants.AuthentificationSchema}.spUserDelete", 
            new Tuple<string, object>("user", userService.GetId()));
        
        var resultCode = result.Length > 0 ? result[0] : -1;
        return resultCode switch
        {
            1 => BadRequest("One of the parameters is null"),
            2 => BadRequest("The user does not exist"),
            0 => Ok("User deleted"),
            _ => BadRequest("An unexpected error occured")
        };
    }

    #endregion
}