using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var userId = User?.Claims.SingleOrDefault(t => t.Type.Equals("id"))?.Value;
        var result = await _reservationService.GetUserReservationsAsync(userId);
        return View(result);
    }
    
    [AllowAnonymous]
    public async Task<IEnumerable<ReservationDto>> GetSportFieldReservations(int sportFieldId)
    {
        return await _reservationService.GetSportFieldReservationsAsync(sportFieldId);
    }

    [AllowAnonymous]
    public async Task<IActionResult> PostReservation(int sportFieldId)
    {
        var model = await _sportFieldService.GetSportFieldWithDetailsAsync(sportFieldId);
        var userId = User?.Claims.SingleOrDefault(t => t.Type.Equals("id"))?.Value;
        var userReservations = await _reservationService.GetUserReservationsAsync(userId);
        ViewData["UserReservations"] = userReservations;
        ViewData["SportField"] = model;
        var viewModel = new ReservationDto();
        viewModel.SportFieldId = model.Id;
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> PostReservation(ReservationDto model)
    {
        model.UserId = User?.Claims.SingleOrDefault(t => t.Type.Equals("id"))?.Value;
        var result = await _reservationService.CreateReservationAsync(model);
        return RedirectToAction("Index");
    }
    
}