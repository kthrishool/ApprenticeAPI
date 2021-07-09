using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record ProfileMessage
    {
        [Required(ErrorMessage = "Surname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Surname must contain only letters, spaces, hyphens and apostrophies")]
        [MaxLength(50, ErrorMessage = "Surname cannot have more than 50 characters")]
        public string Surname { get; init; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "First name must contain only letters, spaces, hyphens and apostrophies")]
        [MaxLength(50, ErrorMessage = "First nme cannot have more than 50 characters")]
        public string FirstName { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Other names must contain only letters, spaces, hyphens and apostrophies")]
        [MaxLength(50, ErrorMessage = "Other names cannot have more than 50 characters")]
        public string OtherNames { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Preferred name must contain only letters, spaces, hyphens and apostrophies")]
        [MaxLength(50, ErrorMessage = "Preferred name cannot have more than 50 characters")]
        public string PreferredName { get; init; }

        [Required(ErrorMessage = "Birth date is required")]
        public DateTime BirthDate { get; init; }

        [Display(Name = "Email address")]
        [MaxLength(256, ErrorMessage = "Email address cannot have more than 256 characters")]
        public string EmailAddress { get; init; }

        [Display(Name = "Profile Type")]
        [MaxLength(10, ErrorMessage = "Profile type cannot have more than 10 characters")]
        [Required(ErrorMessage = "Profile Type is required")]
        public string ProfileType { get; init; }

        [Display(Name = "Phone number")]
        public List<PhoneNumberMessage> PhoneNumbers { get; init; }

        [Display(Name = "Residential Address")]
        public ProfileAddressMessage ResidentialAddress { get; init; }

        [Display(Name = "Postal Address")]
        public ProfileAddressMessage PostalAddress { get; init; }

        [MaxLength(10, ErrorMessage = "Indigenous status code cannot have more than 10 characters")]
        public string IndigenousStatusCode { get; init; }

        [RegularExpression("[@NYny]", ErrorMessage = "Invalid Self assessed disability code. Valid values are @ - Not stated, N - No, Y - Yes")]
        public string SelfAssessedDisabilityCode { get; init; }

        [MaxLength(10, ErrorMessage = "Citizenship code cannot have more than 10 characters")]
        public string CitizenshipCode { get; init; }

        [Display(Name = "Gender")]
        [RegularExpression("[MFXmfx]", ErrorMessage = "Gender Code is invalid")]
        public String GenderCode { get; init; }

        [Display(Name = "InterpretorRequired")]
        public bool? InterpretorRequiredFlag { get; init; }

        [Display(Name = "CountryOfBirthCode")]
        [MaxLength(10, ErrorMessage = "Country of birth code cannot have more than 10 characters")]
        public string CountryOfBirthCode { get; init; }

        [Display(Name = "LanguageCode")]
        [MaxLength(10, ErrorMessage = "Language code cannot have more than 10 characters")]
        public string LanguageCode { get; init; }

        [Display(Name = "PreferredContactType")]
        [MaxLength(10, ErrorMessage = "Preferred contact type cannot have more than 10 characters")]
        public string PreferredContactType { get; init; }

        [Display(Name = "HighestSchoolLevelCode")]
        [MaxLength(10, ErrorMessage = "Highest school level code cannot have more than 10 characters")]
        public string HighestSchoolLevelCode { get; init; }

        public DateTime? LeftSchoolDate { get; init; }

        [MaxLength(11, ErrorMessage = "Visa number cannot have more than 11 characters")]
        public string VisaNumber { get; init; }

        [Display(Name = "Apprentice USI")]
        public string USI { get; init; }      
    }
}