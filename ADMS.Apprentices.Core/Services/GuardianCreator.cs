using System.Threading.Tasks;
using Adms.Shared;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;

namespace ADMS.Apprentices.Core.Services
{
    public class GuardianCreator : IGuardianCreator
    {
        private readonly IGuardianValidator guardianValidator;
        private readonly IRepository repository;

        public GuardianCreator(
            IRepository repository,
            IGuardianValidator profileValidator)
        {
            this.repository = repository;
            this.guardianValidator = profileValidator;
        }

        public async Task<Guardian> CreateAsync(int apprenticeId, ProfileGuardianMessage message)
        {
            await CheckGuardianExists(apprenticeId);
            var guardian = new Guardian()
            {
                ApprenticeId = apprenticeId,
                Surname = message.Surname,
                FirstName = message.FirstName,
                EmailAddress = message.EmailAddress.Sanitise(),
                HomePhoneNumber = message.HomePhoneNumber.SanitiseForPhoneNumber(),
                Mobile = message.Mobile.SanitiseForPhoneNumber(),
                WorkPhoneNumber = message.WorkPhoneNumber.SanitiseForPhoneNumber(),
            };
            if (message.Address != null)
            {
                guardian.SingleLineAddress = message.Address.SingleLineAddress.Sanitise();
                guardian.StreetAddress1 = message.Address.StreetAddress1.Sanitise();
                guardian.StreetAddress2 = message.Address.StreetAddress2.Sanitise();
                guardian.StreetAddress3 = message.Address.StreetAddress3.Sanitise();
                guardian.Locality = message.Address.Locality.Sanitise();
                guardian.StateCode = message.Address.StateCode.SanitiseUpper();
                guardian.Postcode = message.Address.Postcode.Sanitise();
            }

            var exceptionBuilder = await guardianValidator.ValidateAsync(guardian);
            exceptionBuilder.ThrowAnyExceptions();
            return guardian;
        }

        private async Task CheckGuardianExists(int apprenticeId)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            if (profile.Guardian != null)
                throw AdmsValidationException.Create(ValidationExceptionType.GuardianExists);
        }
    }
}