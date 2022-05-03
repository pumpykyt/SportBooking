﻿using System.ComponentModel.DataAnnotations;

namespace SportBooking.BLL.Dtos;

public class RegisterDto
{
    [EmailAddress(ErrorMessage = "Email must be in correct format!")]
    [Required(ErrorMessage = "That field is required!")]
    public string Email { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [RegularExpression(@"^(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", 
        ErrorMessage = "Password must contain: both upper and lowercase characters, at least on digit, one special character")]
    public string Password { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string LastName { get; set; }
}