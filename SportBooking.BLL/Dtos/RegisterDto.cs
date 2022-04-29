using System.ComponentModel.DataAnnotations;

namespace SportBooking.BLL.Dtos;

public class RegisterDto
{
    [EmailAddress(ErrorMessage = "Email must be in correct format!")]
    [Required(ErrorMessage = "That field is required!")]
    public string Email { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string Password { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string LastName { get; set; }
}