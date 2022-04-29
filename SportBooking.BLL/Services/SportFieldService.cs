using AutoMapper;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;
using SportBooking.DAL.Entities;
using SportBooking.DAL.Interfaces;

namespace SportBooking.BLL.Services;

public class SportFieldService : ISportFieldService
{
    private readonly IGenericRepository<SportField> _repository;
    private readonly IMapper _mapper;

    public SportFieldService(IGenericRepository<SportField> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SportFieldDto> GetSportFieldWithDetailsAsync(int sportFieldId)
    {
        var field = await _repository.GetByIdAsync(sportFieldId);
        return _mapper.Map<SportField, SportFieldDto>(field);
    }

    public async Task<IEnumerable<SportFieldDto>> GetSportFieldsWithDetailsAsync()
    {
        var fields = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<SportField>, IEnumerable<SportFieldDto>>(fields);
    }
}