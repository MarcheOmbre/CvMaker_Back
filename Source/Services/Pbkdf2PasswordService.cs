using System.Security.Cryptography;
using System.Text;
using CvBuilderBack.Services.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CvBuilderBack.Services;

public class Pbkdf2PasswordService : IPasswordService
{
    private const int Iterations = 100000;

    public byte[] GenerateSalt(int size = IPasswordService.DefaultSaltSize)
    {
        if(size <= 0)
            throw new ArgumentException("Size must be greater than 0");
        
        if(size % 8 != 0)
            throw new ArgumentException("Size must be multiple of 8");
        
        var salt = new byte[size / 8];
        
        using var rng = RandomNumberGenerator.Create();
        rng.GetNonZeroBytes(salt);

        return salt;
    }

    public byte[] GetPasswordHash(string secretKey, string password, byte[] salt, int hashSize = IPasswordService.DefaultHashSize)
    {
        if(string.IsNullOrEmpty(secretKey))
            throw new ArgumentNullException(nameof(secretKey), "Secret key must not be null or empty");
        
        if(string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(secretKey), "Secret key must not be null or empty");
        
        if(salt is null || salt.Length == 0)
            throw new ArgumentNullException(nameof(secretKey), "Secret key must not be null or empty");
        
        if(hashSize <= 0)
            throw new ArgumentException("Hash size must be greater than 0");
        
        if(hashSize % 8 != 0)
            throw new ArgumentException("Hash size must be multiple of 8");
        
        // Add secret
        var secretKeyPlusSalt = secretKey + Convert.ToBase64String(salt);

        // Hash
        return KeyDerivation.Pbkdf2(password, Encoding.ASCII.GetBytes(secretKeyPlusSalt), KeyDerivationPrf.HMACSHA256,
            Iterations, hashSize / 8);
    }
}