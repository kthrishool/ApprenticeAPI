using System;

namespace ADMS.Apprentices.Core.Models
{
    public record ApprenticeIdentitySearchResultModel
    {
        public int ApprenticeId { get; init; }
        public string FirstName { get; init; }
        public string OtherNames { get; init; }
        public string Surname { get; init; }
        public DateTime BirthDate { get; init; }
        public string EmailAddress { get; init; }
        public string USI { get; init; }
        public string MobilePhoneNumber { get; init; }
        public string OtherPhoneNumber { get; init; }
        public string ResidentialAddress { get; init; }
        public int ScoreValue { get; init; }
        public bool USIMatch { get; init; }
        public bool PhoneNumberMatch { get; init; }
        public bool EmailMatch { get; init; }
        public bool BirthDateMatch { get; init; }
        public bool FirstNameMatch { get; init; }
        public bool SurnameMatch { get; init; }
    }
}