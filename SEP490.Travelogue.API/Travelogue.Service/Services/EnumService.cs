using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Travelogue.Service.BusinessModels;

namespace Travelogue.Service.Services;
public interface IEnumService
{
    List<EnumResponse> GetEnumValues<T>() where T : Enum;
    string GetEnumDisplayName<T>(T enumValue) where T : Enum;
}

public class EnumService : IEnumService
{
    public string GetEnumDisplayName<T>(T enumValue) where T : Enum
    {
        return enumValue.GetType()
                   .GetMember(enumValue.ToString())
                   .First()
                   .GetCustomAttribute<DisplayAttribute>()?
                   .Name ?? enumValue.ToString();
    }

    public List<EnumResponse> GetEnumValues<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .Select(e => new EnumResponse
                   {
                       Id = Convert.ToInt32(e),
                       Name = e.ToString(),
                       DisplayName = e.GetType()
                                      .GetMember(e.ToString())
                                      .First()
                                      .GetCustomAttribute<DisplayAttribute>()?
                                      .Name ?? e.ToString()
                   })
                   .ToList();
    }

}
