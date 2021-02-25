using System;
using Adms.Shared;
using ADMS.Services.Infrastructure.Model.Interface;

namespace ADMS.Apprentice.Core.Entities
{
    public class Profile: IAmAnAggregateRoot<int>, IAuditableIdentifier, ITimestampEnabled
    {

        public int Id { get; set; }
        //public string TitleCode { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string PreferredName { get; set; }
        //public string GenderCode { get; set; }
        //public DateTime BirthDate { get; set; }
        //public string EmailAddress { get; set; }
        //public string SelfAssessedDisabilityCode { get; set; }
        //public string IndigenousStatusCode { get; set; }
        //public string CitizenshipCode { get; set; }
        //public string EducationLevelCode { get; set; }
        //public string LeftSchoolMonthCode { get; set; }
        //public string LeftSchoolYearCode { get; set; }
        //public ProfileType ProfileTypeCode { get; set; }


        //public bool DeceasedFlag { get; set; }
        //public bool ActiveFlag { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }
        public long AuditEventId { get; set; }

        //public Profile()
        //{
        //    TitleCode = "X";
        //    ActiveFlag = true;
        //    DeceasedFlag = false;
        //    IndigenousStatusCode = "X";
        //}
    }
}