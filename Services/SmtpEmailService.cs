using CvBuilderBack.Services.Interfaces;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;

namespace CvBuilderBack.Services;

public class SmtpEmailService : IEmailService
{
    public async void SendRetrievePasswordEmail(IConfiguration configuration, string email, string resetLink)
    {
        try
        {
            var client = new MailjetClient(configuration["MailSettings:ApiKey"],
                configuration["MailSettings:SecretKey"]);

            var template = new TransactionalEmailBuilder()
                .WithTo(new SendContact(email))
                .WithTemplateId(7685178)
                .WithVariable("ResetLink", resetLink)
                .WithTemplateLanguage(true)
                .Build();

            var response = await client.SendTransactionalEmailAsync(template);
            Console.WriteLine(response.Messages?[0]?.Errors.FirstOrDefault()?.ErrorMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}