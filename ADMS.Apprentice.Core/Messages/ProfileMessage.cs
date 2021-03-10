using System;
using System.ComponentModel.DataAnnotations;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Messages
{
    public record ProfileMessage
    {        
        [Required(ErrorMessage = "Surname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Surname must contain only letters, spaces, hyphens and apostrophies")]
        [MaxLength(50, ErrorMessage = "Surname Exceeds 50 Characters")]
        public string Surname { get; init; }

        [Required(ErrorMessage = "Firstname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "First name must contain only letters, spaces, hyphens and apostrophies")]
        [MaxLength(50, ErrorMessage = "First Name Exceeds 50 Characters")]
        public string FirstName { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Other names must contain only letters, spaces, hyphens and apostrophies")]
        [MaxLength(50, ErrorMessage = "Other Names Exceeds 50 Characters")]
        public string OtherNames { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Preferred name must contain only letters, spaces, hyphens and apostrophies")]
        [MaxLength(50, ErrorMessage = "Preferred Name Exceeds 50 Characters")]
        public string PreferredName { get; init; }

        [Required(ErrorMessage = "Birth date is required")]
        public DateTime BirthDate { get; init; }

        [Display(Name = "Email address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(256, ErrorMessage = "Email Address Exceeds 256 Characters")]
        public string EmailAddress { get; init; }

    }
}