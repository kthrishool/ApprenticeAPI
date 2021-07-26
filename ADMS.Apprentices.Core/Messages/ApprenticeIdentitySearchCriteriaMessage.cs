using System;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Messages
{
    public record ApprenticeIdentitySearchCriteriaMessage
    {
        public string FirstName { get; init; }
        public string Surname { get; init; }
        public DateTime? BirthDate { get; init; }
        public string USI { get; init; }

        [Email]
        public string EmailAddress { get; init; }

        [PhoneNumber]
        public string PhoneNumber { get; init; }
    }
}