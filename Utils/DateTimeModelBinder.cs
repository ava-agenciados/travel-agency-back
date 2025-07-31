using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace travel_agency_back.Utils
{
    public class DateTimeModelBinder : IModelBinder
    {
        private readonly string _format;
        public DateTimeModelBinder(string format = "dd/MM/yyyy")
        {
            _format = format;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }
            var value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }
            if (DateTime.TryParseExact(value, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                bindingContext.Result = ModelBindingResult.Success(date);
                return Task.CompletedTask;
            }
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Data em formato inválido. Esperado: {_format}");
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
    }
}
