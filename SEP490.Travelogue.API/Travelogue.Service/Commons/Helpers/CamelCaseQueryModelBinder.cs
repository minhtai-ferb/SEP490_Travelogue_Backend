using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Travelogue.Service.Commons.Helpers;

public class CamelCaseQueryModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var model = Activator.CreateInstance(bindingContext.ModelType);

        foreach (var property in bindingContext.ModelType.GetProperties())
        {
            var camelCaseName = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
            var valueProviderResult = bindingContext.ValueProvider.GetValue(camelCaseName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                var value = Convert.ChangeType(valueProviderResult.FirstValue, property.PropertyType);
                property.SetValue(model, value);
            }
        }

        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}