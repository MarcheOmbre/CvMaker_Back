namespace CvBuilderBack.Services.Interfaces;

public interface IEmailService
{
    public void SendRetrievePasswordEmail(IConfiguration configuration, string email, string resetLink);
}