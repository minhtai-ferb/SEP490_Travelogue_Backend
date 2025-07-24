using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Commons.Implementations;

public class TimeService : ITimeService
{
    public DateTimeOffset SystemTimeNow => ConvertToUtcPlus7(DateTimeOffset.Now);

    public static DateTimeOffset ConvertToUtcPlus7(DateTimeOffset dateTime)
    {
        return dateTime.ToOffset(TimeSpan.FromHours(7));
    }
}
