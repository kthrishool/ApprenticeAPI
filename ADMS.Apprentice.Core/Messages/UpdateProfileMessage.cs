
using System.Collections.Generic;

namespace ADMS.Apprentice.Core.Messages
{
    public record UpdateProfileMessage : ProfileMessage
    {
        public new List<UpdatePhoneNumberMessage> PhoneNumbers { get; init; }
    };
}