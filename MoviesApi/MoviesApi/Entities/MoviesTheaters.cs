using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Entities
{
    public class MoviesTheaters
    {
        public int MovieId { get; set; }
        public int TheaterId { get; set; }
        public Theater Theater { get; set; }
        public Movie Movie { get; set; }
    }
}
