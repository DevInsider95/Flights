using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.Binders
{
    public class DoubleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            Double.TryParse(valueResult.FirstValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double actualValue);
            //double actualValue = Convert.ToDouble(valueResult.FirstValue, CultureInfo.InvariantCulture);

            bindingContext.Result = ModelBindingResult.Success(actualValue);

            // Add a ValidationStateEntry with the "correct" ModelMetadata so validation executes on the actual type, not the declared type.
            var modelMetadataProvider = bindingContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();

            bindingContext.ValidationState.Add(actualValue, new ValidationStateEntry
            {
                Metadata = modelMetadataProvider.GetMetadataForType(actualValue.GetType()),
            });

            return Task.CompletedTask;
        }
    }
}
