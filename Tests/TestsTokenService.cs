using CvBuilderBack.Services;
using CvBuilderBack.Services.Interfaces;

namespace Tests;

public class TestsTokenService
{
    private const int DefaultTimeSpan = 10;
    private const int DefaultId = 1;
    
    private ITokenService tokenService;
    

    [SetUp]
    public void Setup()
    {
        tokenService = new JwtTokenService();
    }
    
    [TestCase(0, DefaultTimeSpan)]
    [TestCase(DefaultId, 1000)]
    [TestCase(2, 10000)]
    [TestCase(3, 100000)]
    [TestCase(4, 1000000)]
    [TestCase(5, 10000000)]
    [TestCase(6, 100000000)]
    [TestCase(7, 1000000000)]
    public void GenerateToken(int value, int timeSpan)
    {
        // Create key
        var secret = Utils.GenerateSecret();
        
        // Create time
        var currentDateTime = DateTime.UtcNow;
        var timeSpanValue = TimeSpan.FromSeconds(timeSpan);

        // Create expected expiration removing milliseconds
        var expectedExpiration = currentDateTime + timeSpanValue;
        expectedExpiration = expectedExpiration.AddTicks( - (expectedExpiration.Ticks % TimeSpan.TicksPerSecond)); 
        
        var token = tokenService.CreateToken(secret, value, timeSpanValue);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(token, Is.Not.Null);
            Assert.That(tokenService.GetId(token), Is.EqualTo(value));
            Assert.That(tokenService.GetExpiration(token), Is.EqualTo(expectedExpiration));
        }
    }
    
    [Test]
    public void GenerateTokenWithNullOrEmptySecret()
    {
        Assert.Throws<ArgumentNullException>(() => tokenService.CreateToken(null, DefaultId, TimeSpan.FromSeconds(DefaultTimeSpan)));
        Assert.Throws<ArgumentNullException>(() => tokenService.CreateToken("", DefaultId, TimeSpan.FromSeconds(DefaultTimeSpan)));
    }
    
    [Test]
    public void GenerateTokenWithNegativeOrZeroTimeSpan()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => tokenService.CreateToken(Utils.GenerateSecret(), DefaultId, TimeSpan.FromSeconds(-1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => tokenService.CreateToken(Utils.GenerateSecret(), DefaultId, TimeSpan.FromSeconds(0)));
    }
}