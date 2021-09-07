using System;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public class ProfileGuardianMessage
    {
        [Required(ErrorMessage = "Surname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Surname must contain only letters, spaces, hyphens and apostrophes")]
        [MaxLength(50, ErrorMessage = "Surname exceeds 50 characters")]
        public string Surname { get; init; }

        [Required(ErrorMessage = "Firstname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "First name must contain only letters, spaces, hyphens and apostrophes")]
        [MaxLength(50, ErrorMessage = "First name exceeds 50 characters")]
        public string FirstName { get; init; }

        [Display(Name = "Email address")]
        [Adms.Shared.Attributes.Email(256)]
        public string EmailAddress { get; init; }

        [Display(Name = "Home phone number")]
        [Adms.Shared.Attributes.PhoneNumber]
        public string HomePhoneNumber { get; init; }

        [Display(Name = "Mobile phone number")]        
        [Adms.Shared.Attributes.PhoneNumber]
        public string Mobile { get; init; }

        [Display(Name = "Work phone number")]
        [Adms.Shared.Attributes.PhoneNumber]
        public string WorkPhoneNumber { get; init; } 

        [Display(Name = "Guardian address")]
        public ProfileAddressMessage Address { get; init; }
    }
}