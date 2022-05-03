using System.Security.Claims;
using SportBooking.BLL.Dtos;

namespace SportBooking.BLL.Interfaces;

public interface IAuthService
{
    Task<AuthCallback> LoginAsync(string email, string password);
    Task<AuthCallback> RegisterAsync(RegisterDto model);
    Task<string> GenerateResetPasswordTokenAsync(string email);
    Task<string> GenerateEmailConfirmationTokenAsync(string email);
    Task ResetPasswordAsync(ResetPasswordDto model);
    Task ConfirmEmailAsync(string token, string email);
}