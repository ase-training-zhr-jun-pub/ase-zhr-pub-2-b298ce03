namespace Calvin.BookingService.Domain;

public class Workplace
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Floor { get; set; } = "";
    public List<string> Equipment { get; set; } = new();
    public bool Occupied { get; set; }
    public string LocationId { get; set; } = "";
}
