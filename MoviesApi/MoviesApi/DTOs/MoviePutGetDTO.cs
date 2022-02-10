using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class MoviePutGetDTO
    {
        public MovieDTO Movie {get; set;}
        public List<GenreDTO> NonSelectedGenres { get; set; }
        public List<TheaterDTO> NonSelectedTheaters { get; set; }
        public List<GenreDTO> SelectedGenres { get; set; }
        public List<TheaterDTO> SelectedTheaters { get; set; }
        public List<ActorDTO> Actors { get; set; }

    }
}
