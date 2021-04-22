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
        private readonly IQualificationValidator qualificationValidator;

        public ProfileValidator(IExceptionFactory exceptionFactory,
            IAddressValidator addressValidator,
            IReferenceDataValidator referenceDataValidator,
            IQualificationValidator qualificationValidator)
        {
            this.exceptionFactory = exceptionFactory;
            this.addressValidator = addressValidator;
            this.referenceDataValidator = referenceDataValidator;
            this.qualificationValidator = qualificationValidator;
        }

        public async Task<Profile> ValidateAsync(Profile profile)
        {
            if (!ValidateAge(profile.BirthDate))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidApprenticeAge);

            // making this async because I think we will be wanting to look in the database for duplicates
            if (!(profile.ProfileTypeCode != null && Enum.IsDefined(typeof(ProfileType), profile.ProfileTypeCode)))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidApprenticeprofileType);

            if (!EmailValidation(profile.EmailAddress))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidEmailAddress);

            if (!ValidateLeftSchoolYear(profile.LeftSchoolYearCode))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidLeftSchoolYear);

            if (!profile.LeftSchoolMonthCode.IsNullOrEmpty() && !Enum.IsDefined(typeof(MonthCode), profile.LeftSchoolMonthCode))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidMonthCode);

            if (profile.Phones != null)
            {
                var newPhone = new List<Phone>();
                var preferredPhoneSet = false;
                if (profile?.Phones != null)
                    foreach (Phone phoneNumbers in profile.Phones)
                    {
                        if (phoneNumbers == null || phoneNumbers?.PhoneNumber?.Length == 0) continue;
                        var ErrorMessage = ValidationExceptionType.InvalidPhoneNumber;
                        string? formattedPhone = phoneNumbers?.PhoneNumber;
                        PhoneType phoneType = PhoneType.MOBILE;
                        if (!PhoneValidator.ValidatePhone(ref formattedPhone, ref phoneType, ErrorMessage))
                            throw exceptionFactory.CreateValidationException(ErrorMessage);
                        if (preferredPhoneSet && Convert.ToBoolean(phoneNumbers.PreferredPhoneFlag))
                        {
                            phoneNumbers.PreferredPhoneFlag = false;
                        }
                        else if (Convert.ToBoolean(phoneNumbers.PreferredPhoneFlag))
                            preferredPhoneSet = Convert.ToBoolean(phoneNumbers.PreferredPhoneFlag);
                        if (!newPhone.Any(c => phoneNumbers.PhoneNumber.Contains(c.PhoneNumber)))
                        {
                            newPhone.Add(new Phone()
                            {
                                PhoneNumber = formattedPhone,
                                PhoneTypeCode = phoneType.ToString(),
                                PreferredPhoneFlag = phoneNumbers.PreferredPhoneFlag
                            });
                        }
                    }
                profile.Phones = newPhone;
            }
            // Address validation
            if (profile.Addresses != null)
            {
                // validation needs to happen
                profile.Addresses = await addressValidator.ValidateAsync(profile.Addresses.ToList());
            }

            // Codes validation
            // Country of Birth
            // language
            // Completed School level
            // Preferred Contact
            // Month code
            await referenceDataValidator.ValidateAsync(profile);

            //Qualification validator should be called after the reference data validator to make sure we are passing valid Month codes to create the start and end date
            if (profile.Qualifications != null)
            {
                profile.Qualifications = await qualificationValidator.ValidateAsync(profile.Qualifications.ToList());
            }
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

        private bool ValidateLeftSchoolYear(string stringYear)
        {
            if (stringYear.IsNullOrEmpty())
                return true;

            return (int.TryParse(stringYear, out int year) && (year >= 1900 && year <= DateTime.Now.Year));
        }
    }
}