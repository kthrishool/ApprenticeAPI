using System;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared;

namespace ADMS.Apprentice.Core.Services
{
    public class ProfileCreator : IProfileCreator
    {
        private readonly IRepository repository;
        private readonly IProfileValidator profileValidator;

        public ProfileCreator(IRepository repository,
            IProfileValidator profileValidator)
        {
            this.repository = repository;
            this.profileValidator = profileValidator;
        }

        public async Task<Profile> CreateAsync(ProfileMessage message)
        {
            var profile = new Profile
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                OtherNames = message.OtherNames,
                PreferredName = message.PreferredName,
                BirthDate = message.BirthDate,
                EmailAddress = message.EmailAddress,
                IndigenousStatusCode = message.IndigenousStatusCode,
                SelfAssessedDisabilityCode =  message.SelfAssessedDisabilityCode.ToUpper(),
                CitizenshipCode = message.CitizenshipCode.ToUpper(),
                ProfileTypeCode =
                    Enum.IsDefined(typeof(ProfileType), message?.ProfileType) ? message.ProfileType : null,
                Phones = message?.PhoneNumbers?.Select(c => new Phone()
                    {PhoneNumber = c, PhoneTypeCode = PhoneType.LandLine.ToString()}).ToList()
               
            };
            if(message?.GenderCode !=null)
            {
                profile.GenderCode = Enum.IsDefined(typeof(GenderType), message?.GenderCode.ToUpper()) ? message.GenderCode.ToUpper() : null;
            }
            //List<CodeLocalityPostcodesState> postcodeValidations = new List<CodeLocalityPostcodesState>();
            if (message.ResidentialAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = message.ResidentialAddress.SingleLineAddress,
                    StreetAddress1 = message.ResidentialAddress.StreetAddress1,
                    StreetAddress2 = message.ResidentialAddress.StreetAddress2,
                    StreetAddress3 = message.ResidentialAddress.StreetAddress3,
                    Locality = message.ResidentialAddress.Locality,
                    StateCode = message.ResidentialAddress.StateCode,
                    Postcode = message.ResidentialAddress.Postcode,
                    AddressTypeCode = AddressType.RESD.ToString(),
                });
            }
            if (message.PostalAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = message.PostalAddress.SingleLineAddress,
                    StreetAddress1 = message.PostalAddress.StreetAddress1,
                    StreetAddress2 = message.PostalAddress.StreetAddress2,
                    StreetAddress3 = message.PostalAddress.StreetAddress3,
                    Locality = message.PostalAddress.Locality,
                    StateCode = message.PostalAddress.StateCode,
                    Postcode = message.PostalAddress.Postcode,
                    AddressTypeCode = AddressType.POST.ToString(),
                });
            }

            await profileValidator.ValidateAsync(profile);
            repository.Insert(profile);

            return profile;
        }
    }
}