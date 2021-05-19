using System;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.Core.Messages
{
    public class ProfileGuardianMessage
    {
        [Required(ErrorMessage = "Surname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Surname must contain only letters, spaces, hyphens and apostrophies")]
        [MaxLength(50, ErrorMessage = "Surname exceeds 50 characters")]
        public string Surname { get; init; }

        [Required(ErrorMessage = "Firstname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "First name must contain only letters, spaces, hyphens and apostrophies")]
        [MaxLength(50, ErrorMessage = "First name exceeds 50 characters")]
        public string FirstName { get; init; }


        [Display(Name = "Email address")]
        [MaxLength(256, ErrorMessage = "Email address exceeds 256 characters")]
        public string EmailAddress { get; init; }

        [Display(Name = "Home phone number")]
        public string HomePhoneNumber { get; init; }

        [Display(Name = "Mobile phone number")]
        public string Mobile { get; init; }

        [Display(Name = "Work phone number")]
        public string WorkPhoneNumber { get; init; }
 

        [Display(Name = "Guardian address")]
        public ProfileAddressMessage Address { get; init; }
    }
}