namespace CvBuilderBack.Services.Interfaces;

public interface IEmailService
{
    public void SendEmail(IConfiguration configuration, string email, string subject, string body);
}