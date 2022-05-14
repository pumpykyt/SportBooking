using SportBooking.BLL.Dtos;
using SportBooking.DAL.Entities;

namespace SportBooking.BLL.Interfaces;

public interface IReservationService
{
    Task<ReservationDto> PayReservationAsync(int id);
    Task<ReservationCallback> CreateReservationAsync(ReservationDto reservation);
    Task<ReservationCallback> UpdateReservationAsync(ReservationDto reservation);
    Task DeleteReservationAsync(int reservationId);
    Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(string userId);
    Task<IEnumerable<ReservationDto>> GetSportFieldReservationsAsync(int sportFieldId);
    Task<IEnumerable<ReservationDto>> GetSportFieldReservationsByTitleAsync(string title, int sportFieldId);
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
    Task<ReservationDto> GetReservationAsync(int id);
    Task CancelReservationAsync(int reservationId);
}