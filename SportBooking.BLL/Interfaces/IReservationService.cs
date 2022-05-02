using SportBooking.BLL.Dtos;
using SportBooking.DAL.Entities;

namespace SportBooking.BLL.Interfaces;

public interface IReservationService
{
    Task<ReservationResponseDto> CreateReservationAsync(ReservationDto reservation);
    Task UpdateReservationAsync(ReservationDto reservation);
    Task DeleteReservationAsync(int reservationId);
    Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(string userId);
    Task<IEnumerable<ReservationDto>> GetSportFieldReservationsAsync(int sportFieldId);
    Task<IEnumerable<ReservationDto>> GetReservationsByTitleAsync(string title);
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
}