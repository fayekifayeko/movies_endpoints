using MoviesApi.validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class GenreCreateDTO
    {
        [Required(ErrorMessage = "the filed name {0} is required")]
        [MaxLength(50)]
        [FirstLetterUpperCase]
        public string Name { get; set; }
    }
}
