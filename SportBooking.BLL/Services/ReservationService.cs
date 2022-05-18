using System.Globalization;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;
using SportBooking.DAL.Entities;
using SportBooking.DAL.Interfaces;

namespace SportBooking.BLL.Services;

public class ReservationService : IReservationService
{
    private readonly IGenericRepository<Reservation> _repository;
    private readonly IGenericRepository<SportField> _fieldRepository;
    private readonly IMapper _mapper;

    public ReservationService(IGenericRepository<Reservation> repository, IMapper mapper, IGenericRepository<SportField> fieldRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _fieldRepository = fieldRepository;
    }

    public async Task<ReservationDto> PayReservationAsync(int id)
    {
        var reservation = await _repository.GetByIdAsync(id);
        reservation.Status = "Payed";
        await _repository.UpdateAsync(reservation);
        return _mapper.Map<Reservation, ReservationDto>(reservation);
    }

    public async Task<ReservationCallback> CreateReservationAsync(ReservationDto reservation)
    {
        var allReservations = await _repository.GetAllAsync();
        var overlaps = allReservations.Any(t => t.Start < reservation.End && 
                                                reservation.Start < t.End && 
                                                t.SportFieldId == reservation.SportFieldId &&
                                                t.Id != reservation.Id);
        if (overlaps)
        {
            return new ReservationCallback
            {
                StatusCode = HttpStatusCode.Conflict,
                Error = "There is already another reservation on that day"
            };
        }

        if (reservation.Start < DateTime.Now)
        {
            return new ReservationCallback
            {
                StatusCode = HttpStatusCode.Forbidden,
                Error = "You can`t make a reservation on date before today"
            };
        }

        var newReservation = _mapper.Map<ReservationDto, Reservation>(reservation);
        var field = await _fieldRepository.GetByIdAsync(newReservation.SportFieldId);
        var startSchedule = new DateTime().AddHours(Convert.ToDouble(field.SportFieldDetail.StartProgram.Split('-')[0]))
                                          .AddMinutes(Convert.ToDouble(field.SportFieldDetail.StartProgram.Split('-')[1]));

        var endSchedule = new DateTime().AddHours(Convert.ToDouble(field.SportFieldDetail.EndProgram.Split('-')[0]))
                                        .AddMinutes(Convert.ToDouble(field.SportFieldDetail.EndProgram.Split('-')[1]));
        if (TimeSpan.Compare(startSchedule.TimeOfDay, newReservation.Start.TimeOfDay) == 1 || 
            TimeSpan.Compare(endSchedule.TimeOfDay, newReservation.End.TimeOfDay) == -1 || 
            newReservation.Start.Day != newReservation.End.Day)
        {
            return new ReservationCallback
            {
                StatusCode = HttpStatusCode.Forbidden,
                Error = "Check sportfield schedule"
            };
        }
        var dateDifference = newReservation.End - newReservation.Start;
        var totalPrice = (double)((dateDifference.Days * 24 + 
                                   dateDifference.Hours + 
                                   (double)dateDifference.Minutes * (1d / 60d)) * field.PricePerHour);
        newReservation.Total = totalPrice;
        newReservation.Status = "Pending";
        newReservation.Created = DateTime.UtcNow;
        await _repository.InsertAsync(newReservation);

        return new ReservationCallback
        {
            StatusCode = HttpStatusCode.OK
        };
    }

    public async Task<ReservationCallback> UpdateReservationAsync(ReservationDto reservation)
    {
        var allReservations = await _repository.GetAllAsync();
        var overlaps = allReservations.Any(t => t.Start < reservation.End && 
                                                reservation.Start < t.End && 
                                                t.SportFieldId == reservation.SportFieldId &&
                                                t.Id != reservation.Id);
        if (overlaps)
        {
            return new ReservationCallback
            {
                StatusCode = HttpStatusCode.BadRequest,
                Error = "There is already another reservation on that day"
            };
        }
        var updatedReservation = _mapper.Map<ReservationDto, Reservation>(reservation);
        var dateDifference = updatedReservation.End - updatedReservation.Start;
        var field = await _fieldRepository.GetByIdAsync(updatedReservation.SportFieldId);
        var totalPrice = (dateDifference.Days * 24 + dateDifference.Hours) * field.PricePerHour;
        updatedReservation.Total = totalPrice;
        updatedReservation.Status = "Pending";
        await _repository.UpdateAsync(updatedReservation);
        
        return new ReservationCallback
        {
            StatusCode = HttpStatusCode.OK
        };
    }

    public async Task DeleteReservationAsync(int reservationId)
    {
        var reservation = await _repository.GetByIdAsync(reservationId);
        _repository.DetachLocal(reservation, reservationId);
        await _repository.DeleteAsync(reservation);
    }

    public async Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(string userId)
    {
        var reservations = await _repository.GetAllAsync();
        var filtered = reservations.Where(t => t.UserId == userId);
        return _mapper.Map<IEnumerable<Reservation>, IEnumerable<ReservationDto>>(filtered);
    }

    public async Task<IEnumerable<ReservationDto>> GetSportFieldReservationsAsync(int sportFieldId)
    {
        var reservations = await _repository.GetAllAsync();
        var filtered = reservations.Where(t => t.SportFieldId == sportFieldId);
        return _mapper.Map<IEnumerable<Reservation>, IEnumerable<ReservationDto>>(filtered);
    }

    public async Task<IEnumerable<ReservationDto>> GetSportFieldReservationsByTitleAsync(string? title, int sportFieldId)
    {
        var reservations = await _repository.GetAllAsync();
        if (title is null) return null;
        var filtered = reservations.Where(t => t.Title.ToUpper().Contains(title.ToUpper()) && 
                                               t.SportFieldId == sportFieldId);
        return _mapper.Map<IEnumerable<Reservation>, IEnumerable<ReservationDto>>(filtered);
    }

    public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync()
    {
        var reservations = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<Reservation>, IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<ReservationDto> GetReservationAsync(int id)
    {
        var reservation = await _repository.GetByIdAsync(id);
        return _mapper.Map<Reservation, ReservationDto>(reservation);
    }

    public async Task CancelReservationAsync(int reservationId)
    {
        var reservation = await _repository.GetByIdAsync(reservationId);
        await _repository.DeleteAsync(reservation);
    }
}