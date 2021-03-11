using System;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Models
{
    public record ProfileListModel
    {
        public int Id { get; }
        public string Surname { get; }
        public string FirstName { get; }
        public string OtherNames { get; }
        public string PreferredName { get; }
        public string GenderCode { get; }
        public DateTime BirthDate { get; }
        public string EmailAddress { get; }
        public string SelfAssessedDisabilityCode { get; }
        public string IndigenousStatusCode { get; }
        public string CitizenshipCode { get; }
        public string EducationLevelCode { get; }
        public string LeftSchoolMonthCode { get; }
        public string LeftSchoolYearCode { get; }
        public string ProfileTypeCode { get; }
        public bool DeceasedFlag { get; }
        public bool ActiveFlag { get; }

        public DateTime? CreatedOn { get; }
        public string CreatedBy { get; }
        public DateTime? UpdatedOn { get; }
        public string UpdatedBy { get; }

        public ProfileListModel(Profile apprentice)
        {
            Id = apprentice.Id;
            Surname = apprentice.Surname;
            FirstName = apprentice.FirstName;
            OtherNames = apprentice.OtherNames;
            PreferredName = apprentice.PreferredName;            
            BirthDate = apprentice.BirthDate;
            GenderCode = apprentice.GenderCode;
            EmailAddress = apprentice.EmailAddress;
            SelfAssessedDisabilityCode = apprentice.SelfAssessedDisabilityCode;
            IndigenousStatusCode = apprentice.IndigenousStatusCode;
            CitizenshipCode = apprentice.CitizenshipCode;
            EducationLevelCode = apprentice.EducationLevelCode;
            LeftSchoolMonthCode = apprentice.LeftSchoolMonthCode;
            LeftSchoolYearCode = apprentice.LeftSchoolYearCode;
            ProfileTypeCode = apprentice?.ProfileTypeCode?.ToString();
            DeceasedFlag = apprentice.DeceasedFlag;
            ActiveFlag = apprentice.ActiveFlag;
            CreatedOn = apprentice.CreatedOn;
            CreatedBy = apprentice.CreatedBy;
            UpdatedOn = apprentice.UpdatedOn;
            UpdatedBy = apprentice.UpdatedBy;
        }
    }
}