using MoviesApi.validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Entities
{
    public class Genre // : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="the filed name {0} is required")]
        [MaxLength(50)]
        [FirstLetterUpperCase]
        public string Name { get; set; }

        //[Range(20,60)]
        //public int Age { get; set; }

        //[CreditCard]
        //public string CreditCard { get; set; }

        //[Url]
        //public string Url { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) // will not trigger if one of the above main validations is triggered 
        //{
        //    if (!string.IsNullOrEmpty(Name))
        //    {

        //        var firstLetter = Name[0].ToString();

        //        if (firstLetter != firstLetter.ToUpper())
        //        {
        //            yield return new ValidationResult(
        //                                "Firstletter is not uppercase",
        //                                new string[] { nameof(Name) });
        //        }
                
        //    }
        //}
    }

}
