using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record UpdateProfileMessage : ProfileMessage
    {
        public new List<UpdatePhoneNumberMessage> PhoneNumbers { get; init; }

        [Display(Name = "USIChangeReason")]
        [MaxLength(300, ErrorMessage = "USI Change Reason Exceeds 300 Characters")]
        public string USIChangeReason { get; init; }
    };
}