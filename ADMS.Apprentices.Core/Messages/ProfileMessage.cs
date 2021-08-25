using System;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record ProfileMessage
    {
        [Required(ErrorMessage = "Surname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Surname must contain only letters, spaces, hyphens and apostrophes")]
        [MaxLength(50, ErrorMessage = "Surname cannot have more than 50 characters")]
        public string Surname { get; init; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "First name must contain only letters, spaces, hyphens and apostrophes")]
        [MaxLength(50, ErrorMessage = "First name cannot have more than 50 characters")]
        public string FirstName { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Other names must contain only letters, spaces, hyphens and apostrophes")]
        [MaxLength(50, ErrorMessage = "Other names cannot have more than 50 characters")]
        public string OtherNames { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Preferred name must contain only letters, spaces, hyphens and apostrophes")]
        [MaxLength(50, ErrorMessage = "Preferred name cannot have more than 50 characters")]
        public string PreferredName { get; init; }

        [Required(ErrorMessage = "Birth date is required")]
        public DateTime? BirthDate { get; init; }

        [Display(Name = "Email address")]
        [MaxLength(256, ErrorMessage = "Email address cannot have more than 256 characters")]
        public string EmailAddress { get; init; }

        [Display(Name = "Profile Type")]
        [MaxLength(10, ErrorMessage = "Profile type cannot have more than 10 characters")]
        [Required(ErrorMessage = "Profile Type is required")]
        public string ProfileType { get; init; }

        [MaxLength(5, ErrorMessage = "Phone1 country code cannot have more than 5 characters")]
        [RegularExpression(@"^\s*\+?[0-9 ]+$", ErrorMessage = "Phone1 country code must contain only numbers, spaces or plus sign")]
        public string Phone1CountryCode { get; init; }

        [MaxLength(15, ErrorMessage = "Phone1 cannot have more than 15 characters")]
        [RegularExpression(@"^\s*\+?[0-9 ]+$", ErrorMessage = "Phone1 must contain only numbers, spaces or plus sign")]
        public string Phone1 { get; init; }

        [MaxLength(5, ErrorMessage = "Phone2 country code cannot have more than 5 characters")]
        [RegularExpression(@"^\s*\+?[0-9 ]+$", ErrorMessage = "Phone2 country code must contain only numbers, spaces or plus sign")]
        public string Phone2CountryCode { get; init; }

        [MaxLength(15, ErrorMessage = "Phone2 cannot have more than 15 characters")]
        [RegularExpression(@"^\s*\+?[0-9 ]+$", ErrorMessage = "Phone2 must contain only numbers, spaces or plus sign")]
        public string Phone2 { get; init; }

        [Display(Name = "Residential Address")]
        public ProfileAddressMessage ResidentialAddress { get; init; }

        [Display(Name = "Postal Address")]
        public ProfileAddressMessage PostalAddress { get; init; }

        [MaxLength(10, ErrorMessage = "Indigenous status code cannot have more than 10 characters")]
        public string IndigenousStatusCode { get; init; }

        [RegularExpression("[@NYny]", ErrorMessage = "Invalid self assessed disability code")]
        public string SelfAssessedDisabilityCode { get; init; }

        [MaxLength(10, ErrorMessage = "Citizenship code cannot have more than 10 characters")]
        public string CitizenshipCode { get; init; }

        [Display(Name = "Gender")]
        [RegularExpression("[MFXmfx]", ErrorMessage = "Invalid gender code")]
        public String GenderCode { get; init; }

        [Display(Name = "InterpretorRequired")]
        public bool? InterpretorRequiredFlag { get; init; }

        [Display(Name = "CountryOfBirthCode")]
        [MaxLength(10, ErrorMessage = "Country of birth code cannot have more than 10 characters")]
        public string CountryOfBirthCode { get; init; }

        [Display(Name = "LanguageCode")]
        [MaxLength(10, ErrorMessage = "Language code cannot have more than 10 characters")]
        public string LanguageCode { get; init; }

        [Display(Name = "PreferredContactTypeCode")]
        [MaxLength(10, ErrorMessage = "Preferred contact type code cannot have more than 10 characters")]
        public string PreferredContactTypeCode { get; init; }

        [Display(Name = "HighestSchoolLevelCode")]
        [MaxLength(10, ErrorMessage = "Highest school level code cannot have more than 10 characters")]
        public string HighestSchoolLevelCode { get; init; }

        public DateTime? LeftSchoolDate { get; init; }

        [MaxLength(11, ErrorMessage = "Visa number cannot have more than 11 characters")]
        public string VisaNumber { get; init; }

        [Display(Name = "Apprentice USI")]
        public string USI { get; init; }

        [MaxLength(10, ErrorMessage = "Reason code for not providing USI cannot have more than 10 characters")]
        public string NotProvidingUSIReasonCode { get; init; }
    }
}