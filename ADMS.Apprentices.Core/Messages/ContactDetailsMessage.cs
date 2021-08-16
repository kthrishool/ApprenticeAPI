using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record ContactDetailsMessage
    {
        [Display(Name = "Email address")]
        [MaxLength(256, ErrorMessage = "Email Address Exceeds 256 Characters")]
        public string EmailAddress { get; init; }

        [Display(Name = "Phone number")]
        public List<PhoneNumberMessage> PhoneNumbers { get; init; }

        [Display(Name = "Residential Address")]
        public ProfileAddressMessage ResidentialAddress { get; init; }

        [Display(Name = "Postal Address")]
        public ProfileAddressMessage PostalAddress { get; init; }

        [Display(Name = "PreferredContactTypeCode")]
        [MaxLength(10, ErrorMessage = "Preferred Contact Exceeds 10 Characters")]
        public string PreferredContactTypeCode { get; init; }
    }
}