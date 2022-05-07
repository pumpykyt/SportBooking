using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;

namespace SportBooking.Presentation.Controllers;

public class SportFieldController : BaseController
{
    private readonly ISportFieldService _sportFieldService;
    private readonly IReservationService _reservationService;

    public SportFieldController(ISportFieldService sportFieldService, IReservationService reservationService)
    {
        _sportFieldService = sportFieldService;
        _reservationService = reservationService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> SportFieldWithDetail(int id, string? query)
    {
        var field = await _sportFieldService.GetSportFieldWithDetailsAsync(id);
        var reservations = await _reservationService.GetSportFieldReservationsByTitleAsync(query, id);
        ViewBag.SearchedReservations = reservations;
        return View(field);
    }

    [AllowAnonymous]
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
        var result = await _sportFieldService.CreateSportField(model);
        if (result.StatusCode != HttpStatusCode.BadRequest) return RedirectToAction("Index");
        ViewBag.ErrorMessage = result.Error;
        return View(model);
    }
    
    public async Task<IActionResult> DeleteSportField(int id)
    {
        await _sportFieldService.DeleteSportField(id);
        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> UpdateSportField(int id)
    {
        var sportField = await _sportFieldService.GetSportFieldWithDetailsAsync(id);
        return View(sportField);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateSportField(SportFieldDto model)
    {
        if (!ModelState.IsValid) return View(model);
        await _sportFieldService.UpdateSportField(model);
        return RedirectToAction("Index");
    }
}