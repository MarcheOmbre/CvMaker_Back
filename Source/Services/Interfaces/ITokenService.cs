namespace CvBuilderBack.Services.Interfaces;

public interface ITokenService
{
    public string CreateToken(string secret, int value, TimeSpan timeSpan);
    
    public int GetId(string token);
    
    public DateTime GetExpiration(string token);
}