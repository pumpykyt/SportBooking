﻿using System.ComponentModel.DataAnnotations;

namespace SportBooking.BLL.Dtos;

public class ForgotPasswordDto
{
    [EmailAddress(ErrorMessage = "Email must be in correct format!")]
    [Required(ErrorMessage = "That field is required!")]
    public string Email { get; set; }
}