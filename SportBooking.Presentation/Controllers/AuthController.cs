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
        var identity = await _authService.LoginAsync(user.Email, user.Password);
        var principal = new ClaimsPrincipal(identity);
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
        await _authService.RegisterAsync(user);
        return RedirectToAction("Login", "Auth");
    }
    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}