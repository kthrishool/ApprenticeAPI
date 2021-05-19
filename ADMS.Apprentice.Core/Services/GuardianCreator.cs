using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services.Validators;
using Adms.Shared;

namespace ADMS.Apprentice.Core.Services
{
    public class GuardianCreator : IGuardianCreator
    {
        private readonly IGuardianValidator guardianValidator;
        private readonly IRepository repository;

        public GuardianCreator(IRepository repository,
            IGuardianValidator profileValidator)
        {
            this.repository = repository;
            this.guardianValidator = profileValidator;
        }

        public Guardian CreateAsync(int apprenticeId, ProfileGuardianMessage message)
        {
            var guardian = new Guardian()
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                EmailAddress = message.EmailAddress.Sanitise(),
                SingleLineAddress = message.Address.SingleLineAddress.Sanitise(),
                StreetAddress1 = message.Address.StreetAddress1.Sanitise(),
                StreetAddress2 = message.Address.StreetAddress2.Sanitise(),
                StreetAddress3 = message.Address.StreetAddress3.Sanitise(),
                Locality = message.Address.Locality.Sanitise(),
                StateCode = message.Address.StateCode.Sanitise(),
                Postcode = message.Address.Postcode.Sanitise(),
                LandLine = message.HomePhoneNumber.Sanitise(),
                Mobile = message.Mobile.Sanitise(),
                WorkPhoneNumber = message.WorkPhoneNumber.Sanitise()
            };

              guardianValidator.ValidateAsync(guardian);
            return guardian;
        
        }
    }
}