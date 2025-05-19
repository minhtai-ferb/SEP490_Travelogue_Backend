using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.HotelModels;
public class HotelMediaResponse
{
    public Guid HotelId { get; set; }
    public string HotelName { get; set; }
    public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
}
