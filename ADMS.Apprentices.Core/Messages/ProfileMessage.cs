using System;
using System.ComponentModel.DataAnnotations;
using AdmsAttribute =  Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Messages
{
    public record ProfileMessage
    {
        [Required(ErrorMessage = "Surname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Surname must contain only letters, spaces, hyphens and apostrophes")]
        [AdmsAttribute.MaxLength(50)]
        public string Surname { get; init; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "First name must contain only letters, spaces, hyphens and apostrophes")]        
        [AdmsAttribute.MaxLength(50)]
        public string FirstName { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Other names must contain only letters, spaces, hyphens and apostrophes")]
        [AdmsAttribute.MaxLength(50)]
        public string OtherNames { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Preferred name must contain only letters, spaces, hyphens and apostrophes")]
        [AdmsAttribute.MaxLength(50)]
        public string PreferredName { get; init; }

        [Required(ErrorMessage = "Birth date is required")]
        public DateTime? BirthDate { get; init; }

        [Display(Name = "Email address")]
        [AdmsAttribute.Email(256)]
        public string EmailAddress { get; init; }

        [Display(Name = "Profile Type")]        
        [Required(ErrorMessage = "Profile Type is required")]
        [AdmsAttribute.ReferenceCode(AdmsAttribute.ReferenceCodeType.APPR)]
        public string ProfileType { get; init; }

        [AdmsAttribute.PhoneNumberInternationalPrefix]
        public string Phone1InternationalPrefix { get; init; }

        [AdmsAttribute.PhoneNumber]
        public string Phone1 { get; init; }
        
        [AdmsAttribute.PhoneNumberInternationalPrefix]
        public string Phone2InternationalPrefix { get; init; }
        
        [AdmsAttribute.PhoneNumber]
        public string Phone2 { get; init; }

        [Display(Name = "Residential Address")]
        public ProfileAddressMessage ResidentialAddress { get; init; }

        [Display(Name = "Postal Address")]
        public ProfileAddressMessage PostalAddress { get; init; }

        [AdmsAttribute.ReferenceCode(AdmsAttribute.ReferenceCodeType.INDS)]
        public string IndigenousStatusCode { get; init; }

        [AdmsAttribute.ReferenceCode(AdmsAttribute.ReferenceCodeType.YSNO)]
        public string SelfAssessedDisabilityCode { get; init; }

        [AdmsAttribute.ReferenceCode(AdmsAttribute.ReferenceCodeType.CITZ)]
        public string CitizenshipCode { get; init; }

        [Display(Name = "Gender")]
        [AdmsAttribute.ReferenceCode(AdmsAttribute.ReferenceCodeType.GNDR)]
        public String GenderCode { get; init; }

        [Display(Name = "InterpretorRequired")]
        public bool? InterpretorRequiredFlag { get; init; }

        [Display(Name = "CountryOfBirthCode")]
        [AdmsAttribute.ReferenceCodeAttribute(AdmsAttribute.ReferenceCodeType.CNTY)]
        public string CountryOfBirthCode { get; init; }

        [Display(Name = "LanguageCode")]
        [AdmsAttribute.ReferenceCode(AdmsAttribute.ReferenceCodeType.LANG)]
        public string LanguageCode { get; init; }

        [Display(Name = "PreferredContactTypeCode")]
        [MaxLength(10, ErrorMessage = "Preferred contact type code cannot have more than 10 characters")]
        public string PreferredContactTypeCode { get; init; }

        [Display(Name = "HighestSchoolLevelCode")]
        [AdmsAttribute.ReferenceCode(AdmsAttribute.ReferenceCodeType.SLVL)]
        public string HighestSchoolLevelCode { get; init; }

        public DateTime? LeftSchoolDate { get; init; }

        [MaxLength(11, ErrorMessage = "Visa number cannot have more than 11 characters")]
        public string VisaNumber { get; init; }

        [Display(Name = "Apprentice USI")]
        public string USI { get; init; }

        [AdmsAttribute.ReferenceCode(AdmsAttribute.ReferenceCodeType.USIE)]
        public string NotProvidingUSIReasonCode { get; init; }
    }
}