using System;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Models
{
    public record ProfileListModel
    {
        public int Id { get; }
        public string Surname { get; }
        public string FirstName { get; }
        public string OtherNames { get; }
        public string PreferredName { get; }
        public DateTime BirthDate { get; }
        public string EmailAddress { get; }
        public string ProfileType { get; }

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
            EmailAddress = apprentice.EmailAddress;
            ProfileType = apprentice.ProfileTypeCode;

            CreatedOn = apprentice.CreatedOn;
            CreatedBy = apprentice.CreatedBy;
            UpdatedOn = apprentice.UpdatedOn;
            UpdatedBy = apprentice.UpdatedBy;
        }
    }
}