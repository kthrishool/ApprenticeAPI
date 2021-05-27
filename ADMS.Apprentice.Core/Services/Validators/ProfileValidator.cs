#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared.Exceptions;
using Castle.Core.Internal;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public class ProfileValidator : IProfileValidator
    {
        private readonly IExceptionFactory exceptionFactory;
        private readonly IAddressValidator addressValidator;
        private readonly IReferenceDataValidator referenceDataValidator;
        private readonly IUSIValidator usiValidator;
        private readonly IPhoneValidator phoneValidator;

        public ProfileValidator(IExceptionFactory exceptionFactory,
            IAddressValidator addressValidator,
            IReferenceDataValidator referenceDataValidator,
            IUSIValidator usiValidator,
            IPhoneValidator phoneValidator)
        {
            this.exceptionFactory = exceptionFactory;
            this.addressValidator = addressValidator;
            this.referenceDataValidator = referenceDataValidator;
            this.usiValidator = usiValidator;
            this.phoneValidator = phoneValidator;
        }

        public async Task<Profile> ValidateAsync(Profile profile)
        {
            if (profile.BirthDate.Year == 0001)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidDOB);
            if (!ValidateAge(profile.BirthDate))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidApprenticeAge);
            if (profile.ProfileTypeCode.IsNullOrEmpty())
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidApprenticeprofileType);
            if (!(Enum.IsDefined(typeof(ProfileType), profile.ProfileTypeCode)))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidApprenticeprofileType);

            if (!EmailValidation(profile.EmailAddress))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidEmailAddress);

            if (profile.LeftSchoolDate != null)
            {
                if (Convert.ToDateTime(profile.LeftSchoolDate) < profile.BirthDate)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidLeftSchoolDetails);
            }


            // Phone validation
            PhoneValidation(profile);

            // Address validation
            if (profile.Addresses != null)
            {
                // validation needs to happen
                var updatedAddress = new List<Address>();
                foreach (Address profileAddress in profile.Addresses)
                {
                    IAddressAttributes address = profileAddress;
                    await addressValidator.ValidateAsync(address);
                    updatedAddress.Add((Address) address);
                }
                profile.Addresses = updatedAddress;
            }

            // USI Validator
            USIValidation(profile);

            // Codes validation
            // Country of Birth
            // language
            // Completed School level
            // Preferred Contact            
            await referenceDataValidator.ValidateAsync(profile);

            return profile;
        }


        // All email Validations are done in this function
        private bool EmailValidation(string? emailAddress)
        {
            if (emailAddress.IsNullOrEmpty())
                return true;
            if (!(new EmailAddressAttribute().IsValid(emailAddress)))
                return false;

            // check domain name in Email
            if (emailAddress != null)
            {
                var domainName = emailAddress.Substring(emailAddress.LastIndexOf('@') + 1);
                if (domainName.IndexOf('.') < 1)
                    return false;
                else if (emailAddress.IndexOf("..", StringComparison.Ordinal) >= 0)
                    return false;
            }
            return true;
        }

        private bool ValidateAge(DateTime birthDate)
        {
            //identify the age from DOB and check at least 12 years old.
            var age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear) age--;
            return age >= 12;
        }


        #region Phone validation

        private void PhoneValidation(Profile profile)
        {
            if (profile.Phones != null)
            {
                var newPhone = new List<Phone>();
                var preferredPhoneSet = false;

                foreach (Phone phone in profile.Phones)
                {
                    if (phone == null || phone.PhoneNumber.IsNullOrEmpty()) continue;
                    Phone newPhoneNumber = phone;

                    phoneValidator.ValidatePhonewithType(newPhoneNumber);
                    if (preferredPhoneSet && Convert.ToBoolean(newPhoneNumber.PreferredPhoneFlag))
                    {
                        newPhoneNumber.PreferredPhoneFlag = false;
                    }
                    else if (Convert.ToBoolean(newPhoneNumber.PreferredPhoneFlag))
                        preferredPhoneSet = Convert.ToBoolean(newPhoneNumber.PreferredPhoneFlag);
                    if (!newPhone.Any(c => newPhoneNumber.PhoneNumber.Contains(c.PhoneNumber)))
                    {
                        newPhone.Add(newPhoneNumber);
                    }
                }
                profile.Phones = newPhone;
            }
        }

        #endregion


        #region USI validation

        private void USIValidation(Profile profile)
        {
            usiValidator.Validate(profile);
        }

        #endregion
    }
}