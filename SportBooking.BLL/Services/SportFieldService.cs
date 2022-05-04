using AutoMapper;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;
using SportBooking.DAL.Entities;
using SportBooking.DAL.Interfaces;

namespace SportBooking.BLL.Services;

public class SportFieldService : ISportFieldService
{
    private readonly IGenericRepository<SportField> _repository;
    private readonly IGenericRepository<SportFieldDetail> _detailRepository;
    private readonly IMapper _mapper;

    public SportFieldService(IGenericRepository<SportField> repository, IGenericRepository<SportFieldDetail> detailRepository, IMapper mapper)
    {
        _repository = repository;
        _detailRepository = detailRepository;
        _mapper = mapper;
    }

    public async Task<SportFieldDto> GetSportFieldWithDetailsAsync(int sportFieldId)
    {
        var field = await _repository.GetByIdAsync(sportFieldId);
        return _mapper.Map<SportField, SportFieldDto>(field);
    }

    public async Task<List<SportFieldDto>> GetSportFieldsWithDetailsAsync()
    {
        var fields = await _repository.GetAllAsync();
        return _mapper.Map<List<SportField>, List<SportFieldDto>>(fields);
    }

    public async Task CreateSportField(SportFieldDto model)
    {
        var field = new SportField
        {
            Title = model.Title,
            PricePerHour = model.PricePerHour,
            ImageUrl = model.ImageUrl
        };
        await _repository.InsertAsync(field);

        var detail = new SportFieldDetail
        {
            Description = model.Description,
            Address = model.Address,
            EndProgram = model.EndProgram,
            StartProgram = model.StartProgram,
            SportFieldId = field.Id
        };
        await _detailRepository.InsertAsync(detail);
    }

    public async Task DeleteSportField(int id)
    {
        var field = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(field);
    }

    public async Task UpdateSportField(SportFieldDto model)
    {
        var updatedField = new SportField
        {
            Id = model.Id,
            Title = model.Title,
            PricePerHour = model.PricePerHour,
            ImageUrl = model.ImageUrl
        };
        await _repository.UpdateAsync(updatedField);
        
        var updatedDetail = new SportFieldDetail
        {
            Id = model.SportFieldDetailId,
            Description = model.Description,
            Address = model.Address,
            EndProgram = model.EndProgram,
            StartProgram = model.StartProgram,
            SportFieldId = model.Id
        };
        await _detailRepository.UpdateAsync(updatedDetail);
    }
}