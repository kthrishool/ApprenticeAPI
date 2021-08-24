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
        [MaxLength(256, ErrorMessage = "Email address exceeds 256 characters")]
        public string EmailAddress { get; init; }

        [Display(Name = "Home phone number")]
        [MaxLength(15, ErrorMessage = "Home phone number cannot have more than 15 characters")]
        [RegularExpression(@"^\s*\+?[0-9 ]+$", ErrorMessage = "Home phone number must contain only numbers, spaces or plus sign")]
        public string HomePhoneNumber { get; init; }

        [Display(Name = "Mobile phone number")]
        [MaxLength(15, ErrorMessage = "Mobile phone number cannot have more than 15 characters")]
        [RegularExpression(@"^\s*\+?[0-9 ]+$", ErrorMessage = "Mobile phone number must contain only numbers, spaces or plus sign")]
        public string Mobile { get; init; }

        [Display(Name = "Work phone number")]
        [MaxLength(15, ErrorMessage = "Work phone number cannot have more than 15 characters")]
        [RegularExpression(@"^\s*\+?[0-9 ]+$", ErrorMessage = "Work phone number must contain only numbers, spaces or plus sign")]
        public string WorkPhoneNumber { get; init; } 

        [Display(Name = "Guardian address")]
        public ProfileAddressMessage Address { get; init; }
    }
}