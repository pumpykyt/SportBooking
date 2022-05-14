namespace SportBooking.DAL.Entities;

public class Reservation : BaseEntity
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Title { get; set; }
    public int Total { get; set; }
    public DateTime Created { get; set; }
    public string Status { get; set; } = "Pending";
    public string UserId { get; set; }
    public int SportFieldId { get; set; }
    public virtual User User { get; set; }
    public virtual SportField SportField { get; set; }
}