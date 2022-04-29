using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Exceptions;
using SportBooking.BLL.Interfaces;
using SportBooking.DAL.Entities;

namespace SportBooking.BLL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;

    public AuthService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ClaimsIdentity> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new HttpException(HttpStatusCode.NotFound);
        }
        
        var authResult = await _userManager.CheckPasswordAsync(user, password);
        if (!authResult)
        {
            throw new HttpException(HttpStatusCode.Unauthorized);
        }

        var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "user")
            }, CookieAuthenticationDefaults.AuthenticationScheme);

        return identity;
    }

    public async Task RegisterAsync(RegisterDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is not null)
        {
            throw new HttpException(HttpStatusCode.Conflict);
        }

        var newUser = new User
        {
            Email = model.Email,
            UserName = model.UserName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber
        };

        var identityResult = await _userManager.CreateAsync(newUser, model.Password);
        
        if (!identityResult.Succeeded)
        {
            throw new HttpException(HttpStatusCode.BadRequest);
        }
    }
}