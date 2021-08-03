using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record BasicDetailsMessage
    {
        [Required(ErrorMessage = "Surname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Surname must contain only letters, spaces, hyphens and apostrophes")]
        [MaxLength(50, ErrorMessage = "Surname Exceeds 50 Characters")]
        public string Surname { get; init; }

        [Required(ErrorMessage = "Firstname is required")]
        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "First name must contain only letters, spaces, hyphens and apostrophes")]
        [MaxLength(50, ErrorMessage = "First Name Exceeds 50 Characters")]
        public string FirstName { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Other names must contain only letters, spaces, hyphens and apostrophes")]
        [MaxLength(50, ErrorMessage = "Other Names Exceeds 50 Characters")]
        public string OtherNames { get; init; }

        [RegularExpression("^(?i)[a-z-' ]+$", ErrorMessage = "Preferred name must contain only letters, spaces, hyphens and apostrophes")]
        [MaxLength(50, ErrorMessage = "Preferred Name Exceeds 50 Characters")]
        public string PreferredName { get; init; }

        [Required(ErrorMessage = "Birth date is required")]
        public DateTime BirthDate { get; init; }

        [Display(Name = "Gender")]
        [RegularExpression("[MFXmfx]", ErrorMessage = "Gender Code is Invalid")]
        public String GenderCode { get; init; }

        [Display(Name = "Profile Type")]
        [MaxLength(10, ErrorMessage = "Profile Type Exceeds 10 Characters")]
        [Required(ErrorMessage = "Profile Type is required")]
        public string ProfileType { get; init; }
    }
}