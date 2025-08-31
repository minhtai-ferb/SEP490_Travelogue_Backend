namespace Travelogue.Service.BusinessModels.TourModels;

public sealed class ScheduleValidationResult
{
    public bool IsValid { get; set; } = true;
    public List<string> Errors { get; set; } = new();

    public static ScheduleValidationResult Ok() => new() { IsValid = true };
    public static ScheduleValidationResult Fail(params string[] messages)
        => new() { IsValid = false, Errors = messages?.ToList() ?? new List<string>() };

    public void AddError(string message)
    {
        IsValid = false;
        Errors.Add(message);
    }
}
