using System;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record UpdatePhoneNumberMessage : PhoneNumberMessage
    {
        /// <summary>
        /// Only for Update
        /// </summary>
        public int Id { get; set; }
    }
}