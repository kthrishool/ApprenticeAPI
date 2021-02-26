using System;
using System.ComponentModel.DataAnnotations;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Messages
{
    public record ProfileMessage
    {
        //[Required(ErrorMessage = "Type is required")]
        //public ClaimType? Type { get; init; }

        //[Required(ErrorMessage = "Category is required")]
        //public ClaimCategory? Category { get; init; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; init; }

        [Required(ErrorMessage = "Firstname is required")]
        public string FirstName { get; init; }
        
        public string OtherNames { get; init; }
        
        public string PreferredName { get; init; }

        [Required(ErrorMessage = "Birth date is required")]
        public DateTime BirthDate { get; init; }

        //public string GenderCode { get; init; }
    }
}