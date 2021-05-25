using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services.Validators;

namespace ADMS.Apprentice.Core.Services
{
    public class GuardianCreator : IGuardianCreator
    {
        private readonly IGuardianValidator guardianValidator;

        public GuardianCreator(
            IGuardianValidator profileValidator)
        {
            this.guardianValidator = profileValidator;
        }

        public async Task<Guardian> CreateAsync(ProfileGuardianMessage message)
        {
            var guardian = new Guardian()
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                EmailAddress = message.EmailAddress.Sanitise(),
                HomePhoneNumber = message.HomePhoneNumber.Sanitise(),
                Mobile = message.Mobile.Sanitise(),
                WorkPhoneNumber = message.WorkPhoneNumber.Sanitise()
            };
            if (message.Address != null)
            {
                guardian.SingleLineAddress = message.Address.SingleLineAddress.Sanitise();
                guardian.StreetAddress1 = message.Address.StreetAddress1.Sanitise();
                guardian.StreetAddress2 = message.Address.StreetAddress2.Sanitise();
                guardian.StreetAddress3 = message.Address.StreetAddress3.Sanitise();
                guardian.Locality = message.Address.Locality.Sanitise();
                guardian.StateCode = message.Address.StateCode.Sanitise();
                guardian.Postcode = message.Address.Postcode.Sanitise();
            }

            await guardianValidator.ValidateAsync(guardian);
            return guardian;
        }
    }
}