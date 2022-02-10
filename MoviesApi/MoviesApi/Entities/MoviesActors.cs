using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Entities
{
    public class MoviesActors
    {
        public int MovieId { get; set; }
        public int ActorId { get; set; }
        public Actor Actor { get; set; }
        public Movie Movie { get; set; }

        [StringLength(25)]
        public string Character { get; set; }
        public int Order { get; set; }
    }
}
