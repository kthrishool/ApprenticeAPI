using System;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.Core.Messages
{
    public record PhoneNumberMessage
    {
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string PhoneTypeCode { get; set; }

        [Display(Name = "Preferred Phone Number")]
        public Boolean PreferredPhoneFlag { get; set; }
    }
}