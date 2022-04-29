using SportBooking.BLL.Dtos;
using SportBooking.DAL.Entities;

namespace SportBooking.BLL.Interfaces;

public interface IReservationService
{
    Task CreateReservationAsync(Reservation reservation);
    Task UpdateReservationAsync(Reservation reservation);
    Task DeleteReservationAsync(int reservationId);
    Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(string userId);
    Task<IEnumerable<ReservationDto>> GetReservationsByTitleAsync(string title);
}