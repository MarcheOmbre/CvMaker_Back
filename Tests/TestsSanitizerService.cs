using Ganss.Xss;

namespace Tests;

public class TestsSanitizerService
{
    private IHtmlSanitizer sanitizer;
    
    [SetUp]
    public void Setup()
    {
        sanitizer = new HtmlSanitizer();
    }

    [Test]
    public void SanitizeNullOrEmptyHtml()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sanitizer.Sanitize(""), Is.Empty);
            Assert.That(sanitizer.Sanitize(null), Is.Empty);
        }
    }
    
    [TestCase("<script>alert('test')</script>")]
    [TestCase("<img src='test.png'/>")]
    [TestCase("<a href='test.com'>test</a>")]
    [TestCase("<h1>test")]
    public void SanitizeHtml(string html)
    {
        var sanitizedHtml = sanitizer.Sanitize(html);
        Assert.That(sanitizedHtml, Is.Not.EqualTo(html));
    }
}