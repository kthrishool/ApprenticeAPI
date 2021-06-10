using System;
using System.Collections.Generic;
using ADMS.Services.Infrastructure.Model.Interface;
using Adms.Shared;

namespace ADMS.Apprentice.Core.Entities
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
        public string PreferredContactType { get; set; }
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

        public string CustomerReferenceNumber { get; set; }
        public bool ActiveFlag { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }
        public long AuditEventId { get; set; }


        public virtual ICollection<Phone> Phones { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<ApprenticeTFN> TFNs { get; set; }
        public virtual ICollection<Qualification> Qualifications { get; set; }

        public virtual Guardian Guardian { get; set; }
        public virtual ICollection<ApprenticeUSI> USIs { get; set; }

        public Profile()
        {
            ActiveFlag = true;
            DeceasedFlag = false;
            Phones = new List<Phone>();
            Addresses = new List<Address>();
            TFNs = new List<ApprenticeTFN>();
            Qualifications = new List<Qualification>();
            USIs = new List<ApprenticeUSI>();
        }
    }
}