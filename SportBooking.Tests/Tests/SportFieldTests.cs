using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SportBooking.BLL.Dtos;
using SportBooking.BLL.Interfaces;
using SportBooking.BLL.Profiles;
using SportBooking.BLL.Services;
using SportBooking.DAL;
using SportBooking.DAL.Entities;
using SportBooking.DAL.Repositories;
using Xunit;

namespace SportBooking.Tests.Tests;

public class SportFieldTests : IDisposable
{
    private ISportFieldService _sportFieldService;
    private readonly IMapper _mapper;
    private DataContext _context;

    private void InitTempInstances()
    {
        var builder = new DbContextOptionsBuilder<DataContext>();
        var dbName = Guid.NewGuid().ToString();
        builder.UseInMemoryDatabase(dbName);
        _context = new DataContext(builder.Options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        var sportFieldRepository = new GenericRepository<SportField>(_context);
        var detailRepository = new GenericRepository<SportFieldDetail>(_context);
        _sportFieldService = new SportFieldService(sportFieldRepository, detailRepository, _mapper);
    }

    private async Task InitTempSportField()
    {
        await _context.SportFields.AddAsync(new SportField
        {
            Id = 1,
            ImageUrl = "Test",
            PricePerHour = 999,
            Title = "Test"
        });
        await _context.SaveChangesAsync();
        await _context.SportFieldDetails.AddAsync(new SportFieldDetail
        {
            Address = "Test",
            Description = "Test",
            EndProgram = "22-00",
            StartProgram = "11-00",
            SportFieldId = 1
        });
        await _context.SaveChangesAsync();
    }
    
    public SportFieldTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperProfile());
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task CreateSportField_Test()
    {
        //arrange
        InitTempInstances();
        var model = new SportFieldDto
        {
            Id = 1,
            ImageUrl = "Test",
            Address = "Test",
            Description = "Test",
            EndProgram = "22-00",
            PricePerHour = 999,
            StartProgram = "11-00",
            Title = "Test"
        };
        
        //act
        await _sportFieldService.CreateSportField(model);
        
        var result = await _context.SportFields.SingleOrDefaultAsync(t => t.Id == model.Id);
        
        //assert
        Assert.Equal(model.Id, result.Id);
    }

    [Fact]
    public async Task GetSportField_Test()
    {
        //arrange
        InitTempInstances();
        await InitTempSportField();
        
        //act
        var result = await _sportFieldService.GetSportFieldWithDetailsAsync(1);
        
        //assert
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task UpdateSportField_Test()
    {
        //arrange
        InitTempInstances();
        await InitTempSportField();
        var modelToUpdate = new SportFieldDto
        {
            Id = 1,
            SportFieldDetailId = 1,
            ImageUrl = "UpdatedTest",
            Address = "UpdatedTest",
            Description = "UpdatedTest",
            EndProgram = "22-00",
            PricePerHour = 999,
            StartProgram = "11-00",
            Title = "UpdatedTest"
        };
        
        //act
        await _sportFieldService.UpdateSportField(modelToUpdate);
        
        //assert
        var result = await _context.SportFields.SingleOrDefaultAsync(t => t.Id == modelToUpdate.Id);
        Assert.Equal(modelToUpdate.Title, result.Title);
    }

    [Fact]
    public async Task DeleteSportField_Test()
    {
        InitTempInstances();
        await InitTempSportField();
        var dataBeforeDelete = await _context.SportFields.ToListAsync();
        var countBeforeDelete = dataBeforeDelete.Count;
        await _sportFieldService.DeleteSportField(1);
        var dataAfterDelete = await _context.SportFields.ToListAsync();
        var countAfterDelete = dataAfterDelete.Count;
        Assert.Equal(countBeforeDelete - 1, countAfterDelete);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}