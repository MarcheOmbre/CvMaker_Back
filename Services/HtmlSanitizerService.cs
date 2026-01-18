using Ganss.Xss;

namespace CvBuilderBack.Services;

public class HtmlSanitizerService : IHtmlSanitizerService
{
    private readonly HtmlSanitizer sanitizer = new();
    
    public string Sanitize(string html)
    {
        return sanitizer.Sanitize(html);
    }
}