using System.Security.Cryptography;

namespace Tests;

public static class Utils
{
    private const int DefaultBits = 512;
    
    public static string GenerateSecret(int bitsCount = DefaultBits)
    {
        if(bitsCount <= 0)
            throw new ArgumentException("Bits count must be greater than 0");
        
        if(bitsCount % 8 != 0)
            throw new ArgumentException("Bits count must be multiple of 8");
        
        var key = new byte[bitsCount];
        RandomNumberGenerator.Create().GetBytes(key);
        return Convert.ToBase64String(key);
    }
    
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(127)]
    [TestCase(129)]
    public static void GenerateSecretWithInvalidSize(int size)
    {
        Assert.Throws<ArgumentException>(() => GenerateSecret(size));
    }
    
    [TestCase(8)]
    [TestCase(16)]
    [TestCase(32)]
    [TestCase(64)]
    [TestCase(128)]
    [TestCase(256)]
    [TestCase(512)]
    public static void GenerateSecretWithValidSize(int size)
    {
        Assert.DoesNotThrow(() => GenerateSecret(size));

        var base64Size = ((4 * size / 3) + 3) & ~3;
        Assert.That(GenerateSecret(size), Has.Length.EqualTo(base64Size));
    }
}