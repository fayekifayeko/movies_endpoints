using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task insertParametersPaginationInHeader<T>(this HttpContext httpContext, IQueryable<T> querable)
        {
            if(httpContext== null) { throw new ArgumentNullException(nameof(httpContext)); }
            double count = await querable.CountAsync();
            httpContext.Response.Headers.Add("totalAmountOfRecords", count.ToString());
        }
    }
}
