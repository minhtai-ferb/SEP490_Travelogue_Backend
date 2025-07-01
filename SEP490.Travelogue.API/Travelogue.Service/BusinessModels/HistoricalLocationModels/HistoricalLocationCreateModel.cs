using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.HistoricalLocationModels;

public class HistoricalLocationCreateModel
{
    public HeritageRank HeritageRank { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public Guid? TypeHistoricalLocationId { get; set; }
}