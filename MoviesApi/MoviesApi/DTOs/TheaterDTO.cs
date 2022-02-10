using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class TheaterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public double langitude { get; set; }
        public double longitude { get; set; }

    }
}
