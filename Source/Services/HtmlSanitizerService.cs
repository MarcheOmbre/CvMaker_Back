using CvBuilderBack.Services.Interfaces;
using Ganss.Xss;

namespace CvBuilderBack.Services;

public class HtmlSanitizerService : IHtmlSanitizerService
{
    private readonly HtmlSanitizer sanitizer = new();
    
    public HtmlSanitizerService()
    {
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedAttributes.Add("id");
        sanitizer.AllowedSchemes.Add("data");
    }
    
    public string Sanitize(string html)
    {
        return sanitizer.Sanitize(html);
    }
}