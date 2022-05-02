using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportBooking.BLL.Interfaces;

namespace SportBooking.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AdminController : Controller
{
    private readonly IMailService _mailService;

    public AdminController(IMailService mailService)
    {
        _mailService = mailService;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    public async Task<IActionResult> SendTestMail()
    {
        await _mailService.SendMailAsync("senseandrey@gmail.com", "test", "test");
        return Ok();
    }
}