using System;
using System.ComponentModel.DataAnnotations;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Messages
{
    public record ProfileMessage
    {        
        [Required(ErrorMessage = "Surname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Surname must contain only letters, spaces, hyphens and apostrophies")]
        public string Surname { get; init; }

        [Required(ErrorMessage = "Firstname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "First name must contain only letters, spaces, hyphens and apostrophies")]
        public string FirstName { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Other names must contain only letters, spaces, hyphens and apostrophies")]
        public string OtherNames { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Preferred name must contain only letters, spaces, hyphens and apostrophies")]
        public string PreferredName { get; init; }

        [Required(ErrorMessage = "Birth date is required")]
        public DateTime BirthDate { get; init; }
        
    }
}