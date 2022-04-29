namespace SportBooking.BLL.Dtos;

public class SportFieldDto
{
    public int Id { get; set; }
    public string Address { get; set; }
    public int PricePerHour { get; set; }
    public string ImageUrl { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartProgram { get; set; }
    public DateTime EndProgram { get; set; }
}