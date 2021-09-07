using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record ProfileSearchMessage
    {
        public int? ApprenticeID { get; init; }

        public string Name { get; init; }        

        public DateTime? BirthDate { get; init; }

        public string EmailAddress { get; init; }

        public string USI { get; init; }
        
        [Adms.Shared.Attributes.PhoneNumber]
        public string Phonenumber { get; init; }

        public string Address { get; init; }
    }
}