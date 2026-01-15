using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CvBuilderBack.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace CvBuilderBack.Services;

public class JwtTokenService : ITokenService
{
    private const string UserKey = "UserId";
    
    public string CreateToken(IConfiguration configuration, int value, TimeSpan timeSpan)
    {
        var tokenSecretKey = configuration["AppSettings:JWTSecret"] ?? throw new Exception("No JWTSecret not defined");

        // Create claims
        var claims = new Claim[] { new(UserKey, value.ToString()) };

        // Crete a security key from the secret key
        var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecretKey));

        // Generate credentials
        var signingCredentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

        // Create a token descriptor to be used for creating the token
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = signingCredentials,
            Expires = DateTime.UtcNow + timeSpan
        };

        // Create token
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        // Convert token to string
        return jwtSecurityTokenHandler.WriteToken(jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor));
    }
}