using System.Security.Cryptography;
using System.Text;
using CvBuilderBack.Services.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CvBuilderBack.Services;

public class Pbkdf2PasswordService : IPasswordService
{
    private const int Iterations = 100000;
    private const int HashSize = 256 / 8;
    private const int SaltSize = 128 / 8;

    public byte[] GenerateSalt()
    {
        var salt = new byte[SaltSize];
        
        using var rng = RandomNumberGenerator.Create();
        rng.GetNonZeroBytes(salt);

        return salt;
    }

    public byte[] GetPasswordHash(IConfiguration configuration, string password, byte[] salt)
    {
        var secretKey = configuration["AppSettings:SecretKey"] ?? throw new Exception("No secret defined");

        // Add secret
        var secretKeyPlusSalt = secretKey + Convert.ToBase64String(salt);

        // Hash
        return KeyDerivation.Pbkdf2(password, Encoding.ASCII.GetBytes(secretKeyPlusSalt), KeyDerivationPrf.HMACSHA256,
            Iterations, HashSize);
    }
}