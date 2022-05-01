using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;

namespace SportBooking.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class ReservationController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly ISportFieldService _sportFieldService;

    public ReservationController(IReservationService reservationService, ISportFieldService sportFieldService)
    {
        _reservationService = reservationService;
        _sportFieldService = sportFieldService;
    }

    public async Task<IActionResult> PostReservation(int sportFieldId)
    {
        var model = await _sportFieldService.GetSportFieldWithDetailsAsync(sportFieldId);
        ViewData["SportField"] = model;
        return View(new ReservationDto());
    }

    [HttpPost]
    public async Task<IActionResult> PostReservation(ReservationDto model)
    {
        model.UserId = User?.Claims.SingleOrDefault(t => t.Type.Equals("Id"))?.Value;
        await _reservationService.CreateReservationAsync(model);
        return View();
    }
}