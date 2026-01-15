namespace CvBuilderBack.Services.Interfaces;

public interface IPasswordService
{
    public byte[] GenerateSalt();

    public byte[] GetPasswordHash(IConfiguration configuration, string password, byte[] salt);
}