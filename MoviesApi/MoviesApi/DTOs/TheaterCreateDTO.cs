using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class TheaterCreateDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Range(-90, 90)]
        public double langitude { get; set; }

        [Range(-180, 180)]
        public double longitude { get; set; }
    }
}
