﻿using AutoMapper;
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

    public async Task<ReservationResponseDto> CreateReservationAsync(ReservationDto reservation)
    {
        var newReservation = _mapper.Map<ReservationDto, Reservation>(reservation);
        var field = await _fieldRepository.GetByIdAsync(newReservation.SportFieldId);
        var dateDifference = newReservation.End - newReservation.Start;
        var totalPrice = (dateDifference.Days * 24 + dateDifference.Hours) * field.PricePerHour;
        newReservation.Total = totalPrice;
        newReservation.Created = DateTime.UtcNow;
        await _repository.InsertAsync(newReservation);

        return new ReservationResponseDto
        {
            Id = newReservation.Id,
            SportFieldTitle = field.Title,
            ReservationTitle = newReservation.Title,
            TotalPrice = newReservation.Total,
            UserId = newReservation.UserId,
            Start = newReservation.Start,
            End = newReservation.End,
            Created = newReservation.Created
        };
    }

    public async Task UpdateReservationAsync(ReservationDto reservation)
    {
        var updatedReservation = _mapper.Map<ReservationDto, Reservation>(reservation);
        await _repository.UpdateAsync(updatedReservation);
    }

    public async Task DeleteReservationAsync(int reservationId)
    {
        var reservation = await _repository.GetByIdAsync(reservationId);
        await _repository.UpdateAsync(reservation);
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

    public async Task<IEnumerable<ReservationDto>> GetReservationsByTitleAsync(string title)
    {
        var reservations = await _repository.GetAllAsync();
        var filtered = reservations.Where(t => t.Title.Contains(title));
        return _mapper.Map<IEnumerable<Reservation>, IEnumerable<ReservationDto>>(filtered);
    }

    public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync()
    {
        var reservations = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<Reservation>, IEnumerable<ReservationDto>>(reservations);
    }
}