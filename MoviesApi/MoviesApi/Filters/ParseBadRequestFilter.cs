using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Filters
{
    public class ParseBadRequestFilter : IActionFilter
    {
        public ParseBadRequestFilter()
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var result = context.Result as IStatusCodeActionResult;
            if (result == null) return;
            var statusCode = result.StatusCode;
            if(statusCode == 400)
            {
                var response = new List<string>();
                var badRequestResult = context.Result as BadRequestObjectResult;
                if (badRequestResult.Value is string) response.Add(badRequestResult.Value.ToString());
                else if (badRequestResult.Value is IEnumerable<IdentityError> errors)
                {
                    foreach(var err in errors)
                    {
                        response.Add(err.Description);
                    }
                }
                else
                {
                    foreach(var myKey in context.ModelState.Keys)
                    {
                        foreach(var error in context.ModelState[myKey].Errors)
                        {
                            response.Add($"{myKey}: {error.ErrorMessage}");
                        }
                    }
                }

                context.Result = new BadRequestObjectResult(response);

            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
