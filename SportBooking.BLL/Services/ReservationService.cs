using AutoMapper;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;
using SportBooking.DAL.Entities;
using SportBooking.DAL.Interfaces;

namespace SportBooking.BLL.Services;

public class ReservationService : IReservationService
{
    private readonly IGenericRepository<Reservation> _repository;
    private readonly IMapper _mapper;

    public ReservationService(IGenericRepository<Reservation> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task CreateReservationAsync(ReservationDto reservation)
    {
        var newReservation = _mapper.Map<ReservationDto, Reservation>(reservation);
        await _repository.InsertAsync(newReservation);
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

    public async Task<IEnumerable<ReservationDto>> GetReservationsByTitleAsync(string title)
    {
        var reservations = await _repository.GetAllAsync();
        var filtered = reservations.Where(t => t.Title.Contains(title));
        return _mapper.Map<IEnumerable<Reservation>, IEnumerable<ReservationDto>>(filtered);
    }
}