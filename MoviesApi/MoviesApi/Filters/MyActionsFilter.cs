using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Filters
{
    public class MyActionsFilter : IActionFilter
    {
      private readonly  ILogger<MyActionsFilter> logger;
        public MyActionsFilter(ILogger<MyActionsFilter> logger) {
            this.logger = logger;

}
        public void OnActionExecuted(ActionExecutedContext context)
        {
            this.logger.LogWarning("OnActionExecuted");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            this.logger.LogWarning("OnActionExecuting");
        }
    }
}
