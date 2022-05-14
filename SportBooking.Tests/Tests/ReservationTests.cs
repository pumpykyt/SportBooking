using System;
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

public class ReservationTests : IDisposable
{
    private readonly IReservationService _reservationService;
    private readonly DataContext _context;
    
    public ReservationTests()
    {
        var builder = new DbContextOptionsBuilder<DataContext>();
        builder.UseInMemoryDatabase("ReservationTestsDb");
        _context = new DataContext(builder.Options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        var reservationRepository = new GenericRepository<Reservation>(_context);
        var sportFieldRepository = new GenericRepository<SportField>(_context);
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperProfile());
        });
        var mapper = mapperConfig.CreateMapper();
        _reservationService = new ReservationService(reservationRepository, mapper, sportFieldRepository);

        _context.SportFields.Add(new SportField
        {
            Id = 1,
            Title = "Test",
            ImageUrl = "Test",
            PricePerHour = 999
        });

        _context.SaveChanges();

        _context.SportFieldDetails.Add(new SportFieldDetail
        {
            Address = "Test",
            SportFieldId = 1,
            Description = "Test",
            StartProgram = "10-00",
            EndProgram = "23-00"
        });

        _context.Users.Add(new User
        {
            Id = "UserTestId",
            UserName = "TestUser",
            Email = "Test@email.com",
            FirstName = "TestUser",
            LastName = "TestUser",
            PhoneNumber = "Test",
            PasswordHash = "UserTestHash"
        });
        
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateReservation_Test()
    {
        var model = new ReservationDto
        {
            Id = 1,
            Start = new DateTime(2022, 7, 10, 12, 0, 0),
            End = new DateTime(2022, 7, 10, 18, 0, 0),
            UserId = "UserTestId",
            SportFieldId = 1,
            Status = "Pending",
            Title = "Test",
        };

        await _reservationService.CreateReservationAsync(model);

        var result = await _context.Reservations.SingleOrDefaultAsync(t => t.Id == model.Id);
        
        Assert.Equal(model.Title, result.Title);
    }

    [Fact]
    public async Task GetReservation_Test()
    {
        _context.Reservations.Add(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            SportFieldId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();
        var result = await _reservationService.GetReservationAsync(77);
        Assert.Equal("Test", result.Title);
    }
    [Fact]
    public async Task DeleteReservation_Test()
    {
        _context.Reservations.Add(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            SportFieldId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();
        var dataBeforeDelete = await _context.Reservations.ToListAsync();
        var countBeforeDelete = dataBeforeDelete.Count;
        await _reservationService.DeleteReservationAsync(77);
        var dataAfterDelete = await _context.Reservations.ToListAsync();
        var countAfterDelete = dataAfterDelete.Count;
        Assert.Equal(countBeforeDelete - 1, countAfterDelete);
    }

    [Fact]
    public async Task UpdateReservation_Test()
    {
        _context.Reservations.Add(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            SportFieldId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();

        var modelToUpdate = new ReservationDto
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            SportFieldId = 1,
            Title = "Updated",
        };

        await _reservationService.UpdateReservationAsync(modelToUpdate);

        var result = await _context.Reservations.SingleOrDefaultAsync(t => t.Id == modelToUpdate.Id);
        Assert.Equal(modelToUpdate.Title, result.Title);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}