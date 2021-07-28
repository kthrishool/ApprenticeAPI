using System;
using System.Linq;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Models
{
    public record ProfileListModel
    {
        public int Id { get; }
        public string Surname { get; }
        public string FirstName { get; }
        public string OtherNames { get; }       
        public DateTime BirthDate { get; }
        public string EmailAddress { get; }
        public string ProfileType { get; }
        public string USI { get; }        
        public int ScoreValue { get; }

        public ProfileListModel(Profile apprentice)
        {
            Id = apprentice.Id;
            Surname = apprentice.Surname;
            FirstName = apprentice.FirstName;
            OtherNames = apprentice.OtherNames;            
            BirthDate = apprentice.BirthDate;
            EmailAddress = apprentice.EmailAddress;
            ProfileType = apprentice.ProfileTypeCode;
            USI = apprentice.USIs.Any(c => c.ActiveFlag == true) ? apprentice.USIs.Where(c => c.ActiveFlag == true).LastOrDefault().USI : null;
        }

        public ProfileListModel(ProfileSearchResultModel apprentice)
        {
            Id = apprentice.ApprenticeId;
            Surname = apprentice.Surname;
            FirstName = apprentice.FirstName;
            OtherNames = apprentice.OtherNames;
            BirthDate = apprentice.BirthDate;
            EmailAddress = apprentice.EmailAddress;
            ProfileType = apprentice.ProfileTypeCode;
            USI = apprentice.USI;
            ScoreValue = apprentice.ScoreValue;
        }
    }
}


       