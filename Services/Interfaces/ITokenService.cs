namespace CvBuilderBack.Services.Interfaces;

public interface ITokenService
{
    public string CreateToken(IConfiguration configuration, int value, TimeSpan timeSpan);
}