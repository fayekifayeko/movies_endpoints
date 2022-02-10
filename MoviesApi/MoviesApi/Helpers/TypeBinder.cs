using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindContext)
        {
            var propName = bindContext.ModelName;
            var value = bindContext.ValueProvider.GetValue(propName);
            if (value == ValueProviderResult.None) return Task.CompletedTask;
            else
            {

                try
                {
                    var desireableValue = JsonConvert.DeserializeObject<T>(value.FirstValue);
                    bindContext.Result = ModelBindingResult.Success(desireableValue);

                } catch
                {
                    bindContext.ModelState.TryAddModelError(propName, "The given value is not of correct type");
                }
                return Task.CompletedTask;

            }
        }
    }
}
