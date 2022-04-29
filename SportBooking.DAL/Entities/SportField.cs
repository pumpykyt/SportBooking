namespace SportBooking.DAL.Entities;

public class SportField : BaseEntity
{
    public int PricePerHour { get; set; }
    public string ImageUrl { get; set; }
    public string Title { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; }
    public virtual SportFieldDetail SportFieldDetail { get; set; }
}