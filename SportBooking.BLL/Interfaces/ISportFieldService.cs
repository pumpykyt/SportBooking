using SportBooking.BLL.Dtos;
using SportBooking.DAL.Entities;

namespace SportBooking.BLL.Interfaces;

public interface ISportFieldService
{
    Task<SportFieldDto> GetSportFieldWithDetailsAsync(int sportFieldId);
    Task<List<SportFieldDto>> GetSportFieldsWithDetailsAsync();
    Task<SportFieldCallback> CreateSportField(SportFieldDto model);
    Task DeleteSportField(int id);
    Task UpdateSportField(SportFieldDto model);
}