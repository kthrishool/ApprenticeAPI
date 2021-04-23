using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [MaxLength(256, ErrorMessage = "Email Address Exceeds 256 Characters")]
        public string EmailAddress { get; init; }

        [Display(Name = "Profile Type")]
        [MaxLength(10, ErrorMessage = "Profile Type Exceeds 10 Characters")]
        [Required(ErrorMessage = "Profile Type is required")]
        public string ProfileType { get; init; }

        [Display(Name = "Phone number")]
        public List<PhoneNumberMessage> PhoneNumbers { get; init; }

        [Display(Name = "Residential Address")]
        public ProfileAddressMessage ResidentialAddress { get; init; }

        [Display(Name = "Postal Address")]
        public ProfileAddressMessage PostalAddress { get; init; }

        [RegularExpression("[@1234]", ErrorMessage = "Invalid Indigenous status code. " +
                                                     "Valid values are @ - Not stated, 1 - Aboriginal, 2 - Torres Strait Islander, 3 - Aboriginal and Torres Strait Islander, 4 - Not Aboriginal OR Torres Strait Islander")]
        public string IndigenousStatusCode { get; init; }

        [RegularExpression("[@NYny]", ErrorMessage = "Invalid Self assessed disability code. Valid values are @ - Not stated, N - No, Y - Yes")]
        public string SelfAssessedDisabilityCode { get; init; }

        [RegularExpression("[ANOano]", ErrorMessage = "Invalid Citizenship code. " +
                                                      "Valid values are A - Aus citizen or PR, N - NZ passport holder who has been resident in Australia 6 months or more , O - Other")]
        public string CitizenshipCode { get; init; }

        [Display(Name = "Gender")]
        [RegularExpression("[MFXmfx]", ErrorMessage = "Gender Code is Invalid")]
        public String GenderCode { get; init; }

        [Display(Name = "InterpretorRequired")]
        public bool? InterpretorRequiredFlag { get; init; }

        [Display(Name = "CountryOfBirthCode")]
        [MaxLength(10, ErrorMessage = "Country of birth code Exceeds 10 Characters")]
        public string CountryOfBirthCode { get; init; }

        [Display(Name = "LanguageCode")]
        [MaxLength(10, ErrorMessage = "Language code Exceeds 10 Characters")]
        public string LanguageCode { get; init; }

        [Display(Name = "PreferredContactType")]
        [MaxLength(10, ErrorMessage = "Preferred Contact Exceeds 10 Characters")]
        public string PreferredContactType { get; init; }

        [Display(Name = "HighestSchoolLevelCode")]
        [MaxLength(10, ErrorMessage = "Highest School Level Code Exceeds 10 Characters")]
        public string HighestSchoolLevelCode { get; init; }

        [MaxLength(10, ErrorMessage = "Left School Month code Exceeds 10 Characters")]
        public string LeftSchoolMonthCode { get; init; }

        [MaxLength(10, ErrorMessage = "Left School Year code Exceeds 10 Characters")]
        public string LeftSchoolYearCode { get; init; }

        [MaxLength(11, ErrorMessage = "Visa number exceeds 11 Characters")]
        [RegularExpression("^[a-zA-Z0-9]{11}$", ErrorMessage = "Visa number must be a 11 character string with only alphanumeric characters")]
        public string VisaNumber { get; init; }

        [Display(Name = "Qualifications")]
        public List<ProfileQualificationMessage> Qualifications { get; init; }
    }
}