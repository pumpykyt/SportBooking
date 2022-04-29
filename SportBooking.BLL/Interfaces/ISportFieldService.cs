using SportBooking.BLL.Dtos;
using SportBooking.DAL.Entities;

namespace SportBooking.BLL.Interfaces;

public interface ISportFieldService
{
    Task<SportFieldDto> GetSportFieldWithDetailsAsync(int sportFieldId);
    Task<IEnumerable<SportFieldDto>> GetSportFieldsWithDetailsAsync();
}