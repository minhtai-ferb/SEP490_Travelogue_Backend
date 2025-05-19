namespace Travelogue.Service.BusinessModels.EventModels;
public class EventGroupByMonth
{
    public int Month { get; set; }
    public List<EventDataModel> Events { get; set; } = new List<EventDataModel>();
}

public class EventGroupByYear
{
    public int Year { get; set; }
    public List<EventGroupByMonth> Months { get; set; } = new List<EventGroupByMonth>();
}

