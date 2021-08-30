using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ADMS.Services.Infrastructure.Model.Interface;
using Adms.Shared;

namespace ADMS.Apprentices.Core.Entities
{
    public class Profile : IAmAnAggregateRoot<int>, IAuditableIdentifier, ITimestampEnabled
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string PreferredName { get; set; }
        public string GenderCode { get; set; }
        public DateTime BirthDate { get; set; }
        public string EmailAddress { get; set; }
        public string PreferredContactTypeCode { get; set; }
        public string SelfAssessedDisabilityCode { get; set; }
        public string IndigenousStatusCode { get; set; }
        public string CitizenshipCode { get; set; }
        public string HighestSchoolLevelCode { get; set; }
        public DateTime? LeftSchoolDate { get; set; }
        public string ProfileTypeCode { get; set; }
        public string VisaNumber { get; set; }
        public string CountryOfBirthCode { get; set; }
        public string LanguageCode { get; set; }
        public bool? InterpretorRequiredFlag { get; set; }
        public bool DeceasedFlag { get; set; }
        public DateTime? DeceasedDate { get; set; }
        public string NotProvidingUSIReasonCode { get; set; }
        public string CustomerReferenceNumber { get; set; }
        public bool ActiveFlag { get; set; }
        public DateTime? InactiveDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }
        public long AuditEventId { get; set; }

        public virtual ICollection<Phone> Phones { get; }
        public virtual ICollection<Address> Addresses { get; }
        public virtual ICollection<ApprenticeTFN> TFNs { get; }
        public virtual ICollection<PriorQualification> PriorQualifications { get; }
        public virtual ICollection<PriorApprenticeshipQualification> PriorApprenticeshipQualifications { get; }
        public virtual ICollection<ApprenticeUSI> USIs { get; }
        public virtual Guardian Guardian { get; set; }

        public Profile()
        {
            ActiveFlag = true;
            DeceasedFlag = false;
            Phones = new Collection<Phone>();
            Addresses = new Collection<Address>();
            TFNs = new Collection<ApprenticeTFN>();
            PriorQualifications = new Collection<PriorQualification>();
            PriorApprenticeshipQualifications = new Collection<PriorApprenticeshipQualification>();
            USIs = new Collection<ApprenticeUSI>();
        }
    }
}