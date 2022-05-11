using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;

namespace SportBooking.Presentation.Controllers;

public class ReservationController : BaseController
{
    private readonly IReservationService _reservationService;
    private readonly ISportFieldService _sportFieldService;
    private readonly IMailService _mailService;

    public ReservationController(IReservationService reservationService, ISportFieldService sportFieldService, IMailService mailService)
    {
        _reservationService = reservationService;
        _sportFieldService = sportFieldService;
        _mailService = mailService;
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
        var viewModel = new ReservationDto();
        viewModel.Start = DateTime.Today;
        viewModel.End = DateTime.Today.AddDays(1);
        viewModel.SportFieldId = sportFieldId;
        return View(viewModel);
    }

    public IActionResult InvoiceSent()
    {
        return View();
    }
    
    public async Task<IActionResult> PayReservation(int id)
    {
        var result = await _reservationService.PayReservationAsync(id);
        var userEmail = User.Claims.SingleOrDefault(t => t.Type == ClaimTypes.Email).Value;
        await _mailService.SendInvoiceAsync(userEmail, result);
        return RedirectToAction("InvoiceSent");
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IEnumerable<ReservationDto>> GetSportFieldReservationsByTitleAsync(string title, int sportFieldId)
    {
        return await _reservationService.GetSportFieldReservationsByTitleAsync(title, sportFieldId);
    }

    public async Task<IActionResult> DeleteReservation(int id)
    {
        await _reservationService.DeleteReservationAsync(id);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> UpdateReservation(int id)
    {
        var reservation = await _reservationService.GetReservationAsync(id);
        return View(reservation);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateReservation(int id, int sportFieldId, ReservationDto model)
    {
        if (!ModelState.IsValid) return View(model);
        model.Id = id; 
        model.SportFieldId = sportFieldId;
        model.UserId = User?.Claims.SingleOrDefault(t => t.Type.Equals("id"))?.Value;
        var callback = await _reservationService.UpdateReservationAsync(model);
        if (callback.StatusCode != HttpStatusCode.BadRequest) return RedirectToAction("Index");
        ViewBag.ErrorMessage = callback.Error;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> PostReservation(int sportFieldId, ReservationDto model)
    {
        if (!ModelState.IsValid) return View(model);
        model.SportFieldId = sportFieldId;
        model.UserId = User?.Claims.SingleOrDefault(t => t.Type.Equals("id"))?.Value;
        var callback = await _reservationService.CreateReservationAsync(model);
        
        if (callback.StatusCode == HttpStatusCode.OK)
        {
            return RedirectToAction("Index");
        }
        
        ViewBag.ErrorMessage = callback.Error;
        return View(model);
    }
}