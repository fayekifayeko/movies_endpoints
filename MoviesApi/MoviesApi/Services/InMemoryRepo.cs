using MoviesApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public class InMemoryRepo :IRepository
    {
        private List<Genre> _genres;
        public InMemoryRepo()
        {
            _genres = new List<Genre>()
            {
                new Genre(){Id = 1, Name = "Comedy"},
                new Genre(){Id = 2, Name = "Drama"}

            };
        }

        public List<Genre> GetAllGenres()
        {
            return _genres;
        }

        public Genre GetGenreById(int Id)
        {
            return _genres.FirstOrDefault(item => item.Id == Id);
        }
    }
}
