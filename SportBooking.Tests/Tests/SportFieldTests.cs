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

public class SportFieldTests
{
    private readonly ISportFieldService _sportFieldService;
    private readonly SportFieldDto _testModel;

    public SportFieldTests()
    {
        var builder = new DbContextOptionsBuilder<DataContext>();
        builder.UseInMemoryDatabase("TestsDb");
        var context = new DataContext(builder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        var sportFieldRepository = new GenericRepository<SportField>(context);
        var detailRepository = new GenericRepository<SportFieldDetail>(context);
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
            EndProgram = "Test",
            PricePerHour = 999,
            StartProgram = "Test",
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
        var result = await _sportFieldService.GetSportFieldWithDetailsAsync(_testModel.Id);
        Assert.Equal(_testModel.Id, result.Id);
    }

    [Fact]
    public async Task GetSportField_Test()
    {
        var result = await _sportFieldService.GetSportFieldWithDetailsAsync(_testModel.Id);
        Assert.Equal(_testModel.Id, result.Id);
    }
    

    [Fact]
    public async Task DeleteSportField_Test()
    {
        var dataBeforeDelete = await _sportFieldService.GetSportFieldsWithDetailsAsync();
        var countBeforeDelete = dataBeforeDelete.Count();
        await _sportFieldService.DeleteSportField(_testModel.Id);
        var dataAfterDelete = await _sportFieldService.GetSportFieldsWithDetailsAsync();
        var countAfterDelete = dataAfterDelete.Count();
        Assert.Equal(countBeforeDelete, countAfterDelete + 1);
    }
}