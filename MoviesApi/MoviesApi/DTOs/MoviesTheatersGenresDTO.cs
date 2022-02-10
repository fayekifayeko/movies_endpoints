using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class MoviesTheatersGenresDTO
    {
        public List<GenreDTO> Genres { get; set; }
        public List<TheaterDTO> Theaters { get; set; }
    }
}
