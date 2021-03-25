using System;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;

namespace ADMS.Apprentice.Core.Services
{
    public class ProfileCreator : IProfileCreator
    {
        private readonly IRepository repository;

        private readonly IProfileValidator profileValidator;
        private readonly IReferenceDataClient referenceDataClient;

        public ProfileCreator(IRepository repository,
            IProfileValidator profileValidator,
            IReferenceDataClient referenceDataClient)
        {
            this.repository = repository;
            this.profileValidator = profileValidator;
            this.referenceDataClient = referenceDataClient;
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
                ProfileTypeCode =
                    Enum.IsDefined(typeof(ProfileType), message?.ProfileType) ? message.ProfileType : null,
                Phones = message?.PhoneNumbers?.Select(c => new Phone()
                    {PhoneNumber = c, PhoneTypeCode = PhoneType.LandLine.ToString()}).ToList(),
            };
            //List<CodeLocalityPostcodesState> postcodeValidations = new List<CodeLocalityPostcodesState>();
            if (message.ResidentialAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    StateCode = message.ResidentialAddress.StateCode,
                    Postcode = message.ResidentialAddress.Postcode,
                    AddressTypeCode = AddressType.RESD.ToString(),
                    SingleLineAddress = message.ResidentialAddress.SingleLineAddress,
                    Locality = message.ResidentialAddress.Locality,
                    StreetAddress1 = message.ResidentialAddress.StreetAddress1,
                    StreetAddress2 = message.ResidentialAddress.StreetAddress2,
                    StreetAddress3 = message.ResidentialAddress.StreetAddress3
                });
            }
            if (message.PostalAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    StateCode = message.PostalAddress.StateCode,
                    Postcode = message.PostalAddress.Postcode,
                    AddressTypeCode = AddressType.POST.ToString(),
                    SingleLineAddress = message.PostalAddress.SingleLineAddress,
                    Locality = message.PostalAddress.Locality,
                    StreetAddress1 = message.PostalAddress.StreetAddress1,
                    StreetAddress2 = message.PostalAddress.StreetAddress2,
                    StreetAddress3 = message.PostalAddress.StreetAddress3
                });
            }

            //var b = await referenceDataClient.AutocompleteAddress(message.ResidentialAddress.SingleLineAddress);
            var c = await referenceDataClient.AutocompleteAddress(message.ResidentialAddress.SingleLineAddress);
            await profileValidator.ValidateAsync(profile);
            repository.Insert(profile);
           
            return profile;
        }
    }
}