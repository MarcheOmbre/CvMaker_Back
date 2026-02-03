using CvBuilderBack.Services;
using CvBuilderBack.Services.Interfaces;

namespace Tests;

public class TestsPasswordService
{
    private const string DefaultPassword = "MyPassword";
    
    private IPasswordService passwordService;
    
    [SetUp]
    public void Setup()
    {
        passwordService = new Pbkdf2PasswordService();
    }

    [TestCase(-5)]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(10)]
    public void GenerateSaltWithInvalidSize(int size)
    {
        Assert.Throws<ArgumentException>(() => passwordService.GenerateSalt(size));
    }
    
    [TestCase(16)]
    [TestCase(32)]
    [TestCase(64)]
    [TestCase(128)]
    [TestCase(256)]
    [TestCase(512)]
    public void GenerateSaltWithValidSize(int size)
    {
        byte[]? salt = null;
        
        Assert.DoesNotThrow(() => salt = passwordService.GenerateSalt(size));
        Assert.That(salt, Is.Not.Null);
        Assert.That(salt, Has.Length.EqualTo(size / 8));
    }

    [Test]
    public void GeneratePasswordWithNullOrEmptySecret()
    {
        Assert.Throws<ArgumentNullException>(() => passwordService.GetPasswordHash(null, DefaultPassword, passwordService.GenerateSalt()));
        Assert.Throws<ArgumentNullException>(() => passwordService.GetPasswordHash("", DefaultPassword, passwordService.GenerateSalt()));
    }

    [Test]
    public void GeneratePasswordWithNullOrEmptySalt()
    {
        Assert.Throws<ArgumentNullException>(() => passwordService.GetPasswordHash(Utils.GenerateSecret(), DefaultPassword, null));
        Assert.Throws<ArgumentNullException>(() => passwordService.GetPasswordHash(Utils.GenerateSecret(), DefaultPassword, []));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    public void GeneratePasswordWithInvalidHashSize(int size)
    {
        Assert.Throws<ArgumentException>(() => passwordService.GetPasswordHash(Utils.GenerateSecret(), DefaultPassword, passwordService.GenerateSalt(), size));
    }
    
    [Test]
    public void GeneratePasswordWithNullOrEmptyPassword()
    {
        Assert.Throws<ArgumentNullException>(() => passwordService.GetPasswordHash(Utils.GenerateSecret(), null, passwordService.GenerateSalt()));
        Assert.Throws<ArgumentNullException>(() => passwordService.GetPasswordHash(Utils.GenerateSecret(), "", passwordService.GenerateSalt()));
    }
    
    [TestCase(DefaultPassword, 32)]
    [TestCase("MyPassword65454946153", 64)]
    [TestCase("MyPassword_545446^&(*&)(&*&^", 128)]
    [TestCase("MyPassword/9870984903+_)+_)+094890394098", 256)]
    [TestCase("MyPassword/9870984903+_)+_)+094890394098", 512)]
    public void GeneratePassword(string value, int hashSize)
    {
        // Create key
        var secret = Utils.GenerateSecret();
        var password = passwordService.GenerateSalt();

        byte[]? passwordHash = null;
        
        Assert.DoesNotThrow(() => passwordHash = passwordService.GetPasswordHash(secret, value, password, hashSize));
        Assert.That(passwordHash, Is.Not.Null);
        Assert.That(passwordHash, Has.Length.EqualTo(hashSize / 8));
    }

    [Test]
    public void TestPasswordMatchesWithNullOrEmptyPasswords()
    {
        Assert.Throws<ArgumentNullException>(() => PasswordMatches(null, DefaultPassword));
        Assert.Throws<ArgumentNullException>(() => PasswordMatches("", DefaultPassword));
        Assert.Throws<ArgumentNullException>(() => PasswordMatches(DefaultPassword, null));
        Assert.Throws<ArgumentNullException>(() => PasswordMatches(DefaultPassword, ""));
    }
    
    [TestCase("MyPassword", "MyPassword")]
    [TestCase("MyPassword", "MyPassword65454946153")]
    [TestCase("MyPassword", "MyPassword_545446^&(*&)(&*&^")]
    [TestCase("MyPassword", "MyPassword/9870984903+_)+_)+094890394098")]
    public void PasswordMatches(string password, string passwordConfirmation)
    {
        if(string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password), "Passwords must not be null or empty");
        
        if(string.IsNullOrEmpty(passwordConfirmation))
            throw new ArgumentNullException(nameof(passwordConfirmation), "Passwords must not be null or empty");
        
        var secret = Utils.GenerateSecret();
        var salt = passwordService.GenerateSalt();
        
        var passwordHash = passwordService.GetPasswordHash(secret, password, salt);
        var passwordConfirmationHash = passwordService.GetPasswordHash(secret, passwordConfirmation, salt);

        Assert.That(passwordHash.SequenceEqual(passwordConfirmationHash), Is.EqualTo(password.Equals(passwordConfirmation)));
    }
}