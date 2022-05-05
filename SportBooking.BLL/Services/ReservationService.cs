﻿using System.Net;
using AutoMapper;
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

    public async Task<ReservationCallback> CreateReservationAsync(ReservationDto reservation)
    {
        var allReservations = await _repository.GetAllAsync();
        var overlaps = allReservations.Any(t => t.Start < reservation.End && 
                                                             reservation.Start < t.End);
        if (overlaps)
        {
            return new ReservationCallback
            {
                StatusCode = HttpStatusCode.BadRequest,
                Error = "There is already another reservation on that day"
            };
        }
        var newReservation = _mapper.Map<ReservationDto, Reservation>(reservation);
        var field = await _fieldRepository.GetByIdAsync(newReservation.SportFieldId);
        var dateDifference = newReservation.End - newReservation.Start;
        var totalPrice = (dateDifference.Days * 24 + dateDifference.Hours) * field.PricePerHour;
        newReservation.Total = totalPrice;
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
        var filtered = reservations.Where(t => t.Title.Contains(title) && 
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
}