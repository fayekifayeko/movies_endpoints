using MoviesApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Helpers
{
    public static class IquerableExtensions
    {
        public static IQueryable<T> paginate<T>(this IQueryable<T> querable, PaginationDTO paginationDTO)
        {
            return querable
                .Skip((paginationDTO.Page - 1) * paginationDTO.RecordsPerPage)
                .Take(paginationDTO.RecordsPerPage);

        }
    }
}
