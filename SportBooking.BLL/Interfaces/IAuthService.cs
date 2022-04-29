using System.Security.Claims;
using SportBooking.BLL.Dtos;

namespace SportBooking.BLL.Interfaces;

public interface IAuthService
{
    Task<ClaimsIdentity> LoginAsync(string email, string password);
    Task RegisterAsync(RegisterDto model);
}