using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.ApiBehavior
{
    public class BadRequestBehavior
    {
        public static void parse(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var response = new List<string>();

                foreach (var myKey in context.ModelState.Keys)
                {
                    foreach (var error in context.ModelState[myKey].Errors)
                    {
                        response.Add($"{myKey}: {error.ErrorMessage}");
                    }
                }

                return new BadRequestObjectResult(response);
            };
        }
    }
}
