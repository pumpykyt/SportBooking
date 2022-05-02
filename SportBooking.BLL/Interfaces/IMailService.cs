namespace SportBooking.BLL.Interfaces;

public interface IMailService
{
    Task SendMailAsync(string to, string subject, string message);
}