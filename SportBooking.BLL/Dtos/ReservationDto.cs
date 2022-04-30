using System.ComponentModel.DataAnnotations;

namespace SportBooking.BLL.Dtos;

public class ReservationDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public DateTime Start { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public DateTime End { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string Title { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public int Total { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string PrimaryColor { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string SecondaryColor { get; set; }
    public string UserId { get; set; }
    public int SportFieldId { get; set; }
}