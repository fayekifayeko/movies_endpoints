using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Trailer { get; set; }
        public string Poster { get; set; }

        public bool InTheaters{ get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<MoviesGenres> MoviesGenres { get; set; }
        public List<MoviesTheaters> MoviesTheaters { get; set; }
        public List<MoviesActors> MoviesActors { get; set; }


    }
}
