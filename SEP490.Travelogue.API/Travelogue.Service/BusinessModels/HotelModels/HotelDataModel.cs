namespace Travelogue.Service.BusinessModels.HotelModels;

public class HotelDataModel
{
    public decimal? PricePerNight { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Guid? LocationId { get; set; }
}
