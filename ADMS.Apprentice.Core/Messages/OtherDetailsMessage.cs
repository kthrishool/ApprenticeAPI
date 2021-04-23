using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.Core.Messages
{
    public record OtherDetailsMessage
    {
        [RegularExpression("[@1234]", ErrorMessage = "Invalid Indigenous status code. " +
                                                     "Valid values are @ - Not stated, 1 - Aboriginal, 2 - Torres Strait Islander, 3 - Aboriginal and Torres Strait Islander, 4 - Not Aboriginal OR Torres Strait Islander")]
        public string IndigenousStatusCode { get; init; }

        [RegularExpression("[@NYny]", ErrorMessage = "Invalid Self assessed disability code. Valid values are @ - Not stated, N - No, Y - Yes")]
        public string SelfAssessedDisabilityCode { get; init; }

        [RegularExpression("[ANOano]", ErrorMessage = "Invalid Citizenship code. " +
                                                      "Valid values are A - Aus citizen or PR, N - NZ passport holder who has been resident in Australia 6 months or more , O - Other")]
        public string CitizenshipCode { get; init; }

        [Display(Name = "InterpretorRequired")]
        public bool? InterpretorRequiredFlag { get; init; }

        [Display(Name = "CountryOfBirthCode")]
        [MaxLength(10, ErrorMessage = "Country of birth code exceeds 10 Characters")]
        public string CountryOfBirthCode { get; init; }

        [Display(Name = "LanguageCode")]
        [MaxLength(10, ErrorMessage = "Language code exceeds 10 Characters")]
        public string LanguageCode { get; init; }
        
        [MaxLength(11, ErrorMessage = "Visa number exceeds 11 Characters")]
        [RegularExpression("^[a-zA-Z0-9]{11}$", ErrorMessage = "Visa number must be a 11 character string with only alphanumeric characters")]
        public string VisaNumber { get; init; }
    }
}