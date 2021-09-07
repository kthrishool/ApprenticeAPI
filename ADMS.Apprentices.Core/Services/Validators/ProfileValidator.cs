#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using Adms.Shared.Exceptions;
using Castle.Core.Internal;

namespace ADMS.Apprentices.Core.Services.Validators
{
    public class ProfileValidator : IProfileValidator
    {
        private readonly IAddressValidator addressValidator;        
        private readonly IUSIValidator usiValidator;
        private readonly IPhoneValidator phoneValidator;

        public ProfileValidator(
            IAddressValidator addressValidator,            
            IUSIValidator usiValidator,
            IPhoneValidator phoneValidator)
        {
            this.addressValidator = addressValidator;            
            this.usiValidator = usiValidator;
            this.phoneValidator = phoneValidator;
        }

        public async Task<ValidationExceptionBuilder> ValidateAsync(Profile profile)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();
            var tasks = new List<Task<ValidationExceptionBuilder>>();
            if (profile.EmailAddress == null && profile.Phones.Count == 0)
                exceptionBuilder.AddException(ValidationExceptionType.MandatoryContact);

            if (profile.BirthDate.Year == 0001)
                exceptionBuilder.AddException(ValidationExceptionType.InvalidDOB);

            if (!ValidateAge(profile.BirthDate))
                exceptionBuilder.AddException(ValidationExceptionType.InvalidApprenticeAge);

            if (profile.ProfileTypeCode.IsNullOrEmpty() ||
                !(Enum.IsDefined(typeof(ProfileType), profile.ProfileTypeCode)))
                exceptionBuilder.AddException(ValidationExceptionType.InvalidApprenticeprofileType);


            if (!EmailValidation(profile.EmailAddress))
                exceptionBuilder.AddException(ValidationExceptionType.InvalidEmailAddress);

            if (profile.LeftSchoolDate != null)
            {
                if (Convert.ToDateTime(profile.LeftSchoolDate) < profile.BirthDate || Convert.ToDateTime(profile.LeftSchoolDate) > DateTime.Now)
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidLeftSchoolDetails);
            }

            if (!profile.PreferredContactTypeCode.IsNullOrEmpty())
            {
                ValidatePreferredContactType(exceptionBuilder, profile);
            }

            // Phone validation
            //exceptionBuilder.AddExceptions(PhoneValidation(profile));

            tasks.Add(ValidateAddressesAsync(profile));

            // USI Validator
            exceptionBuilder.AddExceptions(USIValidation(profile));
           
            exceptionBuilder.AddExceptions(await tasks.WaitAndReturnExceptionBuilder());

            return exceptionBuilder;
        }
        
        protected async Task<ValidationExceptionBuilder> ValidateAddressesAsync(Profile profile)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();
            var tasks = new List<Task<ValidationExceptionBuilder>>();
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
            if (!new EmailAddressAttribute().IsValid(emailAddress))
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

        private void ValidatePreferredContactType(ValidationExceptionBuilder exceptionBuilder, Profile profile)
        {            
            switch (profile.PreferredContactTypeCode)
            {
                case nameof(PreferredContactType.MOBILE) or nameof(PreferredContactType.SMS):
                    // if (profile.Phones.Any(c => !c.PhoneNumber.IsNullOrEmpty() && c.PhoneNumber.StartsWith("04")) == false)
                    if (profile.Phones.Any() == false)
                    {
                        exceptionBuilder.AddException(ValidationExceptionType.MobilePreferredContactIsInvalid);
                    }
                    break;
                case nameof(PreferredContactType.PHONE):
                    if (profile.Phones.Any() == false)
                    {
                        exceptionBuilder.AddException(ValidationExceptionType.PhonePreferredContactIsInvalid);
                    }
                    break;
                case nameof(PreferredContactType.EMAIL):
                    if (string.IsNullOrEmpty(profile.EmailAddress))
                    {
                        exceptionBuilder.AddException(ValidationExceptionType.EmailPreferredContactIsInvalid);
                    }
                    break;
                case nameof(PreferredContactType.MAIL):
                    if (profile.Addresses.Any() == false)
                    {
                        exceptionBuilder.AddException(ValidationExceptionType.MailPreferredContactIsInvalid);
                    }
                    break;
                default:
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidPreferredContactCode);
                    break;
            }
            // validate rules based on the type of contact
        }

        #region Phone validation
        //**** NOT IN USE ***********
        //private ValidationExceptionBuilder PhoneValidation(Profile profile)
        //{
        //    var exceptionBuilder = new ValidationExceptionBuilder();
        //    if (profile.Phones != null)
        //    {               
        //        var preferredPhoneSet = false;

        //        foreach (Phone phone in profile.Phones)
        //        {
        //            if (phone == null) continue;

        //            phoneValidator.ValidatePhonewithType(exceptionBuilder, phone);
        //            if (preferredPhoneSet && Convert.ToBoolean(phone.PreferredPhoneFlag))
        //            {
        //                phone.PreferredPhoneFlag = false;
        //            }
        //            else if (Convert.ToBoolean(phone.PreferredPhoneFlag))
        //                preferredPhoneSet = Convert.ToBoolean(phone.PreferredPhoneFlag);                    
        //        }
        //    }
        //    return exceptionBuilder;
        //}

        #endregion


        #region USI validation

        private ValidationExceptionBuilder USIValidation(Profile profile)
        {
            return usiValidator.Validate(profile);
        }

        #endregion
    }
}