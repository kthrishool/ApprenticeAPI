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
        private readonly IValidatorExceptionBuilderFactory exceptionBuilderFactory;
        private readonly IAddressValidator addressValidator;
        private readonly IReferenceDataValidator referenceDataValidator;
        private readonly IUSIValidator usiValidator;
        private readonly IPhoneValidator phoneValidator;

        public ProfileValidator(IValidatorExceptionBuilderFactory exceptionBuilderFactory,
            IAddressValidator addressValidator,
            IReferenceDataValidator referenceDataValidator,
            IUSIValidator usiValidator,
            IPhoneValidator phoneValidator)
        {
            this.exceptionBuilderFactory = exceptionBuilderFactory;
            this.addressValidator = addressValidator;
            this.referenceDataValidator = referenceDataValidator;
            this.usiValidator = usiValidator;
            this.phoneValidator = phoneValidator;
        }

        public async Task<IValidatorExceptionBuilder> ValidateAsync(Profile profile)
        {
            var exceptionBuilder = exceptionBuilderFactory.CreateExceptionBuilder();
            var tasks = new List<Task<IValidatorExceptionBuilder>>();
            if (profile.EmailAddress == null && (profile.Phones == null || profile.Phones.Count == 0))
                exceptionBuilder.Add(ValidationExceptionType.MandatoryContact);

            if (profile.BirthDate.Year == 0001)
                exceptionBuilder.Add(ValidationExceptionType.InvalidDOB);

            if (!ValidateAge(profile.BirthDate))
                exceptionBuilder.Add(ValidationExceptionType.InvalidApprenticeAge);

            if (profile.ProfileTypeCode.IsNullOrEmpty() ||
                !(Enum.IsDefined(typeof(ProfileType), profile.ProfileTypeCode)))
                exceptionBuilder.Add(ValidationExceptionType.InvalidApprenticeprofileType);


            if (!EmailValidation(profile.EmailAddress))
                exceptionBuilder.Add(ValidationExceptionType.InvalidEmailAddress);

            if (profile.LeftSchoolDate != null)
            {
                if (Convert.ToDateTime(profile.LeftSchoolDate) < profile.BirthDate || Convert.ToDateTime(profile.LeftSchoolDate) > DateTime.Now)
                    exceptionBuilder.Add(ValidationExceptionType.InvalidLeftSchoolDetails);
            }            


            // Phone validation
            exceptionBuilder.AddExceptions(PhoneValidation(profile));

            tasks.Add(ValidateAddressesAsync(profile));

            // USI Validator
            exceptionBuilder.AddExceptions(USIValidation(profile));

            // Codes validation
            // Country of Birth
            // language
            // Completed School level
            // Preferred Contact            
            tasks.Add(referenceDataValidator.ValidateAsync(profile));
            exceptionBuilder.AddExceptions(await tasks.WaitAndReturnExceptionBuilder());

            return exceptionBuilder;
        }
        
        protected async Task<IValidatorExceptionBuilder> ValidateAddressesAsync(Profile profile)
        {
            var exceptionBuilder = exceptionBuilderFactory.CreateExceptionBuilder();
            var tasks = new List<Task<IValidatorExceptionBuilder>>();
            // Address validation
            if (profile.Addresses != null)
            {
                // validation needs to happen
                foreach (Address profileAddress in profile.Addresses)
                {
                    IAddressAttributes address = profileAddress;
                    tasks.Add(addressValidator.ValidateAsync(address));
                }
                if(tasks.Any())
                    exceptionBuilder.AddExceptions(await tasks.WaitAndReturnExceptionBuilder());
            }
            return exceptionBuilder;
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

        private IValidatorExceptionBuilder PhoneValidation(Profile profile)
        {
            var exceptionBuilder = exceptionBuilderFactory.CreateExceptionBuilder();
            if (profile.Phones != null)
            {
                var newPhones = new List<Phone>();
                var preferredPhoneSet = false;

                foreach (Phone phone in profile.Phones)
                {
                    if (phone == null || phone.PhoneNumber.IsNullOrEmpty()) continue;
                    Phone newPhone = phone;

                    phoneValidator.ValidatePhonewithType(exceptionBuilder, newPhone);
                    if (preferredPhoneSet && Convert.ToBoolean(newPhone.PreferredPhoneFlag))
                    {
                        newPhone.PreferredPhoneFlag = false;
                    }
                    else if (Convert.ToBoolean(newPhone.PreferredPhoneFlag))
                        preferredPhoneSet = Convert.ToBoolean(newPhone.PreferredPhoneFlag);
                    if (!newPhones.Any(c => newPhone.PhoneNumber.Contains(c.PhoneNumber)))
                    {
                        newPhones.Add(newPhone);
                    }
                }
                profile.Phones = newPhones;
            }
            return exceptionBuilder;
        }

        #endregion


        #region USI validation

        private IValidatorExceptionBuilder USIValidation(Profile profile)
        {
            return usiValidator.Validate(profile);
        }

        #endregion
    }
}