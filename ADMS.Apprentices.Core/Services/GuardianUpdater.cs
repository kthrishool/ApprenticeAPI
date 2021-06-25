using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.Core.Helpers;
using System;
using System.Linq;
using Adms.Shared.Extensions;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public class GuardianUpdater : IGuardianUpdater
    {
        private readonly IGuardianValidator guardianValidator;
        public GuardianUpdater(IGuardianValidator guardianValidator)
        {
            this.guardianValidator = guardianValidator;
        }

        public async Task<Guardian> Update(Guardian guardian, ProfileGuardianMessage message)
        {
            guardian.Surname = message.Surname;
            guardian.FirstName = message.FirstName;
            guardian.EmailAddress = message.EmailAddress.Sanitise();
            guardian.HomePhoneNumber = message.HomePhoneNumber.Sanitise();
            guardian.Mobile = message.Mobile.Sanitise();
            guardian.WorkPhoneNumber = message.WorkPhoneNumber.Sanitise();
            
            guardian.SingleLineAddress = message.Address?.SingleLineAddress.Sanitise();
            guardian.StreetAddress1 = message.Address?.StreetAddress1.Sanitise();
            guardian.StreetAddress2 = message.Address?.StreetAddress2.Sanitise();
            guardian.StreetAddress3 = message.Address?.StreetAddress3.Sanitise();
            guardian.Locality = message.Address?.Locality.Sanitise();
            guardian.StateCode = message.Address?.StateCode.Sanitise();
            guardian.Postcode = message.Address?.Postcode.Sanitise();
            
            var exceptionBuilder = await guardianValidator.ValidateAsync(guardian);
            exceptionBuilder.ThrowAnyExceptions();
            return guardian;
        }

    }
}