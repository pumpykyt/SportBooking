using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SportBooking.BLL.Configs;
using SportBooking.BLL.Interfaces;

namespace SportBooking.BLL.Services;

public class MailService : IMailService
{
    private readonly SmtpClient _smtpClient;

    public MailService(SmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public async Task SendMailAsync(string to, string subject, string message)
    {
        var mail = new MailMessage
        {
            To = { to },
            From = new MailAddress("sportbookingproject@gmail.com"),
            Subject = subject,
            Body = message
        };

        await _smtpClient.SendMailAsync(mail);
    }
}