namespace Travelogue.Service.Commons.Helpers;

public static class AgeConstraints
{
    public const int MinChildAge = 5;
    public const int MaxChildAge = 11;
    public const int MinAdultAge = 12;
}

public static class AgeHelper
{
    public static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        int age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

    public static bool IsChild(DateTime dateOfBirth)
    {
        var age = CalculateAge(dateOfBirth);
        return age >= AgeConstraints.MinChildAge && age <= AgeConstraints.MaxChildAge;
    }

    public static bool IsAdult(DateTime dateOfBirth)
    {
        var age = CalculateAge(dateOfBirth);
        return age >= AgeConstraints.MinAdultAge;
    }
}
