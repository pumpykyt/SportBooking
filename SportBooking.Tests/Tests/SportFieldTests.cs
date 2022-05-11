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
    private readonly ISportFieldService _sportFieldService;
    private readonly SportFieldDto _testModel;
    private readonly DataContext _context;
    
    public SportFieldTests()
    {
        var builder = new DbContextOptionsBuilder<DataContext>();
        builder.UseInMemoryDatabase("SportFieldTestsDb");
        _context = new DataContext(builder.Options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        var sportFieldRepository = new GenericRepository<SportField>(_context);
        var detailRepository = new GenericRepository<SportFieldDetail>(_context);
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperProfile());
        });
        var mapper = mapperConfig.CreateMapper();
        _sportFieldService = new SportFieldService(sportFieldRepository, detailRepository, mapper);
        _testModel = new SportFieldDto
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

        _sportFieldService.CreateSportField(_testModel).Wait();
        _sportFieldService.CreateSportField(_testModel).Wait();
        _sportFieldService.CreateSportField(_testModel).Wait();
    }

    [Fact]
    public async Task CreateSportField_Test()
    {
        await _sportFieldService.CreateSportField(_testModel);
        var result = await _context.SportFields.SingleOrDefaultAsync(t => t.Id == _testModel.Id);
        Assert.Equal(_testModel.Id, result.Id);
    }

    [Fact]
    public async Task GetSportField_Test()
    {
        var result = await _sportFieldService.GetSportFieldWithDetailsAsync(_testModel.Id);
        Assert.Equal(_testModel.Id, result.Id);
    }

    [Fact]
    public async Task UpdateSportField_Test()
    {
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

        await _sportFieldService.UpdateSportField(modelToUpdate);
        var result = await _context.SportFields.SingleOrDefaultAsync(t => t.Id == modelToUpdate.Id);
        Assert.Equal(modelToUpdate.Title, result.Title);
    }

    [Fact]
    public async Task DeleteSportField_Test()
    {
        var dataBeforeDelete = await _context.SportFields.ToListAsync();
        var countBeforeDelete = dataBeforeDelete.Count;
        await _sportFieldService.DeleteSportField(_testModel.Id);
        var dataAfterDelete = await _context.SportFields.ToListAsync();
        var countAfterDelete = dataAfterDelete.Count;
        Assert.Equal(countBeforeDelete - 1, countAfterDelete);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}