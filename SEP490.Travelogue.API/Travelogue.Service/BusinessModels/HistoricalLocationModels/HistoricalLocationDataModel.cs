using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.HistoricalLocationModels;

public class HistoricalLocationDataModel
{
    // public Guid HistoricalLocationId { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public HeritageRank HeritageRank { get; set; }
    public string? HeritageRankName { get; set; } = string.Empty;
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}