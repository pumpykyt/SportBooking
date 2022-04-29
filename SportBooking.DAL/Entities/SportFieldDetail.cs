namespace SportBooking.DAL.Entities;

public class SportFieldDetail : BaseEntity
{
    public string Address { get; set; }
    public string Description { get; set; }
    public DateTime StartProgram { get; set; }
    public DateTime EndProgram { get; set; }
    public int SportFieldId { get; set; }
    public virtual SportField SportField { get; set; }
}