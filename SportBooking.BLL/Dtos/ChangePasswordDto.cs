using System.ComponentModel.DataAnnotations;

namespace SportBooking.BLL.Dtos;

public class ChangePasswordDto
{
    [MinLength(8, ErrorMessage = "Password length should be 8 or more")]
    [Required(ErrorMessage = "Enter new password!")]
    public string Password { get; set; }
}