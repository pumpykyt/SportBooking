﻿using System.ComponentModel.DataAnnotations;

namespace SportBooking.BLL.Dtos;

public class SportFieldDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string Address { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public int PricePerHour { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string ImageUrl { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string Title { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string Description { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public DateTime StartProgram { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public DateTime EndProgram { get; set; }
}