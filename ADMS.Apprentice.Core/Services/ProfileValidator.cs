using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared.Exceptions;
using Castle.Core.Internal;

namespace ADMS.Apprentice.Core.Services
{
    public class ProfileValidator : IProfileValidator
    {
        private readonly IExceptionFactory exceptionFactory;

        public ProfileValidator(IExceptionFactory exceptionFactory)
        {
            this.exceptionFactory = exceptionFactory;
        }

        public Task ValidateAsync(Profile profile)
        {
            if (!ValidateAge(profile.BirthDate))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidApprenticeAge);
            // making this async because I think we will be wanting to look in the database for duplicates
            if (!(profile.ProfileTypeCode != null && Enum.IsDefined(typeof(ProfileType), profile.ProfileTypeCode)))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidApprenticeprofileType);
            if (!EmailValidation(profile.EmailAddress))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidEmailAddress);
            foreach (var PhoneNumbers in profile.Phones)
            {
                var ErrorMessage = ValidationExceptionType.InvalidPhoneNumber;
                if (!PhoneValidator.ValidatePhone(PhoneNumbers.PhoneNumber, ErrorMessage))
                    throw exceptionFactory.CreateValidationException(ErrorMessage);
            }
            return Task.CompletedTask;
        }

        // All email Validations are done in this function
        private bool EmailValidation(string? emailAddress)
        {
            if (emailAddress.IsNullOrEmpty())
                return true;
            if (!(new EmailAddressAttribute().IsValid(emailAddress)))
                return false;
            if (emailAddress.IndexOf('@') < 0)
                return false;

            // check domain name in Email
            var domainName = emailAddress.Substring(emailAddress.LastIndexOf('@') + 1);
            if (domainName.IndexOf('.') < 1)
                return false;
            return true;
        }

        private bool ValidateAge(DateTime birthDate)
        {
            //identify the age from DOB and check atleast 12 years old.
            var age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear) age--;
            return age >= 12;
        }
    }
}