namespace SportBooking.BLL.Dtos;

public class ReservationDto
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Title { get; set; }
    public int Total { get; set; }
    public string PrimaryColor { get; set; }
    public string SecondaryColor { get; set; }
    public string UserId { get; set; }
    public int SportFieldId { get; set; }
}