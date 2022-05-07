using System.Net;
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

    public async Task<SportFieldCallback> CreateSportField(SportFieldDto model)
    {
        var startScheduleHours = Convert.ToDouble(model.StartProgram.Split('-')[0]);
        var startScheduleMinutes = Convert.ToDouble(model.StartProgram.Split('-')[1]);
        
        var endScheduleHours = Convert.ToDouble(model.EndProgram.Split('-')[0]);
        var endScheduleMinutes = Convert.ToDouble(model.EndProgram.Split('-')[1]);

        if (startScheduleHours is < 0 or > 24 || 
            endScheduleHours is < 0 or > 24 || 
            startScheduleMinutes is < 0 or > 59 || 
            endScheduleMinutes is < 0 or > 59)
        {
            return new SportFieldCallback
            {
                StatusCode = HttpStatusCode.BadRequest,
                Error = "Enter valid time"
            };
        }
        
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

        return new SportFieldCallback
        {
            StatusCode = HttpStatusCode.OK
        };
    }

    public async Task DeleteSportField(int id)
    {
        var field = await _repository.GetByIdAsync(id);
        _repository.DetachLocal(field, id);
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