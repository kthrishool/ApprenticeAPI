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
                OtherNames = string.IsNullOrEmpty(message.OtherNames) ? null : message.OtherNames,
                PreferredName = string.IsNullOrEmpty(message.PreferredName) ? null : message.PreferredName,
                BirthDate = message.BirthDate,
                EmailAddress = string.IsNullOrEmpty(message.EmailAddress) ? null : message.EmailAddress,
                IndigenousStatusCode = string.IsNullOrEmpty(message.IndigenousStatusCode) ? null : message.IndigenousStatusCode,
                SelfAssessedDisabilityCode = string.IsNullOrEmpty(message.SelfAssessedDisabilityCode) ? null : message.SelfAssessedDisabilityCode.ToUpper(),
                InterpretorRequiredFlag = message.InterpretorRequiredFlag,
                CitizenshipCode = string.IsNullOrEmpty(message.CitizenshipCode) ? null : message.CitizenshipCode.ToUpper(),
                ProfileTypeCode =
                    Enum.IsDefined(typeof(ProfileType), message?.ProfileType) ? message.ProfileType : null,
                Phones = message?.PhoneNumbers?.Select(c => new Phone()
                    {PhoneNumber = c, PhoneTypeCode = PhoneType.LandLine.ToString()}).ToList(),
                CountryOfBirthCode = string.IsNullOrEmpty(message.CountryOfBirthCode) ? null : message.CountryOfBirthCode.ToUpper(),
            };
            if (message?.GenderCode != null)
            {
                profile.GenderCode = Enum.IsDefined(typeof(GenderType), message?.GenderCode.ToUpper()) ? message.GenderCode.ToUpper() : null;
            }
            //List<CodeLocalityPostcodesState> postcodeValidations = new List<CodeLocalityPostcodesState>();
            if (message.ResidentialAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = String.IsNullOrEmpty(message.ResidentialAddress.SingleLineAddress) ? null : message.ResidentialAddress.SingleLineAddress,
                    StreetAddress1 = String.IsNullOrEmpty(message.ResidentialAddress.StreetAddress1) ? null : message.ResidentialAddress.StreetAddress1,
                    StreetAddress2 = String.IsNullOrEmpty(message.ResidentialAddress.StreetAddress2) ? null : message.ResidentialAddress.StreetAddress2,
                    StreetAddress3 = String.IsNullOrEmpty(message.ResidentialAddress.StreetAddress3) ? null : message.ResidentialAddress.StreetAddress3,
                    Locality = String.IsNullOrEmpty(message.ResidentialAddress.Locality) ? null : message.ResidentialAddress.Locality,
                    StateCode = String.IsNullOrEmpty(message.ResidentialAddress.StateCode) ? null : message.ResidentialAddress.StateCode,
                    Postcode = String.IsNullOrEmpty(message.ResidentialAddress.Postcode) ? null : message.ResidentialAddress.Postcode,
                    AddressTypeCode = AddressType.RESD.ToString(),
                });
            }
            if (message.PostalAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = String.IsNullOrEmpty(message.PostalAddress.SingleLineAddress) ? null : message.PostalAddress.SingleLineAddress,
                    StreetAddress1 = String.IsNullOrEmpty(message.PostalAddress.StreetAddress1) ? null : message.PostalAddress.StreetAddress1,
                    StreetAddress2 = String.IsNullOrEmpty(message.PostalAddress.StreetAddress2) ? null : message.PostalAddress.StreetAddress2,
                    StreetAddress3 = String.IsNullOrEmpty(message.PostalAddress.StreetAddress3) ? null : message.PostalAddress.StreetAddress3,
                    Locality = String.IsNullOrEmpty(message.PostalAddress.Locality) ? null : message.PostalAddress.Locality,
                    StateCode = String.IsNullOrEmpty(message.PostalAddress.StateCode) ? null : message.PostalAddress.StateCode,
                    Postcode = String.IsNullOrEmpty(message.PostalAddress.Postcode) ? null : message.PostalAddress.Postcode,
                    AddressTypeCode = AddressType.POST.ToString(),
                });
            }

            await profileValidator.ValidateAsync(profile);
            repository.Insert(profile);

            return profile;
        }
    }
}