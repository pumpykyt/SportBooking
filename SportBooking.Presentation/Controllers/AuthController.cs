using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;

namespace SportBooking.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IMailService _mailService;

    public AuthController(IAuthService authService, IMailService mailService)
    {
        _authService = authService;
        _mailService = mailService;
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }
    
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
        var model = new ResetPasswordDto
        {
            Token = token, 
            Email = email
        };
        return View(model);
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
    {
        if (!ModelState.IsValid) return View(model);
        await _authService.ResetPasswordAsync(model);
        return RedirectToAction(nameof(ResetPasswordConfirmation));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ConfirmationLinkSent()
    {
        return View();
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        if (!ModelState.IsValid) return View(model);
        var token = await _authService.GenerateResetPasswordTokenAsync(model.Email);
        var callback = Url.Action(nameof(ResetPassword), 
            "Auth", new { token, email = model.Email }, Request.Scheme);

        await _mailService.SendMailAsync(model.Email, "Reset password", callback);

        return RedirectToAction("ConfirmationLinkSent");
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto user)
    {
        if (!ModelState.IsValid) return View();
        var authCallback = await _authService.LoginAsync(user.Email, user.Password);
        if (authCallback.StatusCode == HttpStatusCode.Unauthorized)
        {
            ViewBag.ErrorMessage = "Wrong login or Password";
            return View();
        }
        var principal = new ClaimsPrincipal(authCallback.ClaimsIdentity);
        HttpContext.User = principal;
        Thread.CurrentPrincipal = principal;  
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto user)
    {
        if (!ModelState.IsValid) return View();
        
        var authCallback = await _authService.RegisterAsync(user);
        if (authCallback.StatusCode == HttpStatusCode.Conflict)
        {
            ViewBag.ErrorMessage = "User with that email already exists";
        }
        if (authCallback.StatusCode == HttpStatusCode.InternalServerError)
        {
            ViewBag.ErrorMessage = "Server error";
        }

        var token = await _authService.GenerateEmailConfirmationTokenAsync(user.Email);
        var emailConfirmationLink = Url.Action(nameof(ConfirmEmail), 
                                                    "Auth", 
                                                    new { token, email = user.Email }, 
                                                    Request.Scheme);
        await _mailService.SendMailAsync(user.Email, "Email.Confirmation", emailConfirmationLink);
        
        
        return RedirectToAction(nameof(RegisterSuccess));
    }
    
    [AllowAnonymous]
    [HttpGet]
    public IActionResult RegisterSuccess()
    {
        return View();
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        await _authService.ConfirmEmailAsync(token, email);
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}