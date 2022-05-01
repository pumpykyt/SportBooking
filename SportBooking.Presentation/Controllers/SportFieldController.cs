using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;

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
    
    public IActionResult PostSportField()
    {
        return View();
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> PostSportField(SportFieldDto model)
    {
        if (!ModelState.IsValid) return View(model);
        await _sportFieldService.CreateSportField(model);
        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> DeleteSportField(int id)
    {
        await _sportFieldService.DeleteSportField(id);
        return RedirectToAction("Index");
    }
    
    public IActionResult UpdateSportField()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateSportField(SportFieldDto model)
    {
        if (!ModelState.IsValid) return View(model);
        await _sportFieldService.UpdateSportField(model);
        return RedirectToAction("Index");
    }
}