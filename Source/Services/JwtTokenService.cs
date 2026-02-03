using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CvBuilderBack.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace CvBuilderBack.Services;

public class JwtTokenService : ITokenService
{
    private const string UserKey = "UserId";
    
    public string CreateToken(string secret, int value, TimeSpan timeSpan)
    {
        if(string.IsNullOrEmpty(secret))
            throw new ArgumentNullException(nameof(secret), "Secret key must not be null or empty");
        
        if(timeSpan.TotalSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeSpan), "Timespan must be greater than 0");

        // Create claims
        var claims = new Claim[] { new(UserKey, value.ToString()) };

        // Create a security key from the secret key
        var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

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

    public int GetId(string token)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtSecurityTokenHandler.ReadJwtToken(token);
        return int.Parse(jwtToken.Claims.First(claim => claim.Type == UserKey).Value);
    }

    public DateTime GetExpiration(string token)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtSecurityTokenHandler.ReadJwtToken(token);
        return jwtToken.ValidTo;
    }
}