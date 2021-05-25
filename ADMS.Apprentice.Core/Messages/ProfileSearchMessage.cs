using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.Core.Messages
{
    public record ProfileSearchMessage
    {
        public string Surname { get; init; }

        public string FirstName { get; init; }

        public string OtherNames { get; init; }

        public DateTime? BirthDate { get; init; }

        public string EmailAddress { get; init; }

        public string USI { get; init; }

        public string Phonenumber { get; init; }

        public string Address { get; init; }
    }
}