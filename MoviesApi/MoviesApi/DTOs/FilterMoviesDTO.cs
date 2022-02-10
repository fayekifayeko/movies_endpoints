using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class FilterMoviesDTO
    {
        public int Page { get; set; }
        public int RecordPerPage { get; set; }
        public PaginationDTO paginationDTO
        {
            get { return new PaginationDTO() { Page = Page, RecordsPerPage = RecordPerPage };  }
        }
        public string Title { get; set; }
        public bool InTeaters { get; set; }
        public bool UpcomingReleases { get; set; }
        public int GenreId { get; set; }

    }
}
    