
using System.Collections.Generic;

namespace ADMS.Apprentices.Core.Messages
{
    public record UpdateProfileMessage : ProfileMessage
    {
        public new List<UpdatePhoneNumberMessage> PhoneNumbers { get; init; }
    };
}