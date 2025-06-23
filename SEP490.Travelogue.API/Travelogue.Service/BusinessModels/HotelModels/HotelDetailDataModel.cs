using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.HotelModels;

public class HotelDetailDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Address { get; set; }
    public decimal? StarRating { get; set; }
    public decimal? PricePerNight { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public Guid? LocationId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
