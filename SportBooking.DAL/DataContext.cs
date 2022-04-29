using Microsoft.EntityFrameworkCore;
using SportBooking.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SportBooking.DAL;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    
    public virtual DbSet<SportFieldDetail> SportFieldDetails { get; set; }
    public virtual DbSet<SportField> SportFields { get; set; }
    public virtual DbSet<Reservation> Reservations { get; set; }
}