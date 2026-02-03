namespace CvBuilderBack.Services.Interfaces;

public interface IPasswordService
{
    protected const int DefaultSaltSize = 128;
    protected const int DefaultHashSize = 256;
    
    public byte[] GenerateSalt(int size = DefaultSaltSize);

    public byte[] GetPasswordHash(string secretKey, string password, byte[] salt, int hashSize = DefaultHashSize);
}