using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.HotelModels;
public class HotelDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid LocationId { get; set; }
    public string? LocationName { get; set; }
    public string? Address { get; set; }
    public decimal? StarRating { get; set; }
    public decimal? PricePerNight { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
