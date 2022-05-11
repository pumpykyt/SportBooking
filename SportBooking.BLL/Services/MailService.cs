using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SportBooking.BLL.Configs;
using SportBooking.BLL.Dtos;
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

    public async Task SendInvoiceAsync(string to, ReservationDto model)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "Invoice.html");
        var htmlString = await File.ReadAllTextAsync(path);

        var body = htmlString.Replace("@ReservationId", model.Id.ToString())
                             .Replace("@ReservationTitle", model.Title)
                             .Replace("@DateTimeUtcNow", DateTime.UtcNow.ToString())
                             .Replace("@TotalPrice", model.Total.ToString())
                             .Replace("@SportFieldId", model.SportFieldId.ToString())
                             .Replace("@Start", model.Start.ToString())
                             .Replace("@End", model.End.ToString());

        var mail = new MailMessage
        {
            IsBodyHtml = true,
            From = new MailAddress("sportbookingproject@gmail.com"),
            Body = body,
            Subject = "Reservation invoice",
            To = { to }
        };
        
        await _smtpClient.SendMailAsync(mail);
    }
}