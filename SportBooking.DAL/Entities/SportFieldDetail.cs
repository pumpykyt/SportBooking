namespace SportBooking.DAL.Entities;

public class SportFieldDetail : BaseEntity
{
    public string Address { get; set; }
    public string Description { get; set; }
    public string StartProgram { get; set; }
    public string EndProgram { get; set; }
    public int SportFieldId { get; set; }
    public virtual SportField SportField { get; set; }
}