using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportBooking.BLL.Interfaces;
using SportBooking.DAL.Entities;

namespace SportBooking.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class SportFieldController : Controller
{
    private readonly ISportFieldService _sportFieldService;

    public SportFieldController(ISportFieldService sportFieldService)
    {
        _sportFieldService = sportFieldService;
    }

    public async Task<IActionResult> Index()
    {
        var sportFields = await _sportFieldService.GetSportFieldsWithDetailsAsync();
        return View(sportFields);
    }
}