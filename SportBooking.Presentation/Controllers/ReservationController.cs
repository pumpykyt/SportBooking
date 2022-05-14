using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;
using SportBooking.DAL.Entities;

namespace SportBooking.Presentation.Controllers;

public class ReservationController : BaseController
{
    private readonly IReservationService _reservationService;
    private readonly ISportFieldService _sportFieldService;
    private readonly IMailService _mailService;
    private readonly UserManager<User> _userManager;

    public ReservationController(IReservationService reservationService, ISportFieldService sportFieldService, IMailService mailService, UserManager<User> userManager)
    {
        _reservationService = reservationService;
        _sportFieldService = sportFieldService;
        _mailService = mailService;
        _userManager = userManager;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var userId = User?.Claims.SingleOrDefault(t => t.Type.Equals("id"))?.Value;
        var result = await _reservationService.GetUserReservationsAsync(userId);
        var user = await _userManager.FindByIdAsync(userId);
        ViewBag.User = user;
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
    
    public async Task<IActionResult> CancelReservation(int reservationId)
    {
        var userEmail = User?.Claims.SingleOrDefault(t => t.Type.Equals(ClaimTypes.Email)).Value;
        await _reservationService.CancelReservationAsync(reservationId);
        await _mailService.SendReservationCancelMailAsync(userEmail, reservationId);
        return RedirectToAction("Index");
    }
}