using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CvBuilderBack.Services.Interfaces;

namespace CvBuilderBack.Services;

public class SmtpEmailService : IEmailService
{
    public void SendEmail(IConfiguration configuration, string email, string subject, string body)
    {
        var host = configuration["MailSettings:Host"] ?? throw new Exception("No host defined");
        var port  = int.Parse(configuration["MailSettings:Port"] ?? throw new Exception("No port defined"));

        using var smtpClient = new SmtpClient(host, port);

        var mailApi = configuration["MailSettings:Api"] ?? throw new Exception("No api defined");
        var secretMailApi =configuration["MailSettings:SecretKey"] ?? throw new Exception("No secret defined");
        var sender = configuration["MailSettings:Sender"] ?? throw new Exception("No sender defined");
        
        smtpClient.EnableSsl = true;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.UseDefaultCredentials = false;
        
        smtpClient.Credentials = new NetworkCredential(mailApi , secretMailApi);
        smtpClient.Send(sender, email, subject, body);
    }
}