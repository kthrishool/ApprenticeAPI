using System;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record PriorApprenticeshipQualificationMessage
    {
        [Required]
        [Adms.Shared.Attributes.MaxLength(200, "Employer name")]
        public string EmployerName { get; init; }

        [Required]
        [Adms.Shared.Attributes.MaxLength(10, "Qualification code")]
        public string QualificationCode { get; init; }

        [Required]
        [Adms.Shared.Attributes.MaxLength(200, "Qualification description")]
        public string QualificationDescription { get; init; }

        [Required]
        [Adms.Shared.Attributes.MaxLength(10, "Qualification level")]
        public string QualificationLevel { get; init; }

        [Required]
        [Adms.Shared.Attributes.MaxLength(10, "ANZSCO code")]
        public string QualificationANZSCOCode { get; init; }

        [Required]
        public bool? NotOnTrainingGovAu { get; init; }

        [Required]
        public DateTime? StartDate { get; init; }

        public string StateCode { get; init; }

        [Required]
        public string CountryCode { get; init; }

        [Adms.Shared.Attributes.MaxLength(30, "Apprenticeship reference")]
        public string ApprenticeshipReference { get; init; }
    }
}