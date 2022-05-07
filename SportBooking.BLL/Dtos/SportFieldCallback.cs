using System.Net;

namespace SportBooking.BLL.Dtos;

public class SportFieldCallback
{
    public HttpStatusCode StatusCode { get; set; }
    public string Error { get; set; }
}