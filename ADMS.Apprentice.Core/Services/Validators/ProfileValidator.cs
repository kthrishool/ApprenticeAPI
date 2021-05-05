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
using System.Globalization;

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

            if (ValidateLeftSchoolDetails(profile.LeftSchoolMonthCode, profile.LeftSchoolYear))
            {
                //At this point we know we have a valid month and year..
                //so create the date out of it if Months and Years exist.
                profile.LeftSchoolDate = profile.LeftSchoolYear.HasValue ? new DateTime(profile.LeftSchoolYear.Value, DateTime.ParseExact(profile.LeftSchoolMonthCode, "MMM", CultureInfo.CurrentCulture).Month, 1) : null;               
            }
            
            if (profile.Phones != null)
            {
                var newPhone = new List<Phone>();
                var preferredPhoneSet = false;
                
                foreach (Phone phone in profile.Phones)
                {
                    if (phone == null || phone.PhoneNumber.IsNullOrEmpty()) continue;
                    var ErrorMessage = ValidationExceptionType.InvalidPhoneNumber;
                    string formattedPhone = phone.PhoneNumber;
                    PhoneType phoneType = PhoneType.MOBILE;
                    if (!PhoneValidator.ValidatePhone(ref formattedPhone, ref phoneType, ErrorMessage))
                        throw exceptionFactory.CreateValidationException(ErrorMessage);
                    if (preferredPhoneSet && Convert.ToBoolean(phone.PreferredPhoneFlag))
                    {
                        phone.PreferredPhoneFlag = false;
                    }
                    else if (Convert.ToBoolean(phone.PreferredPhoneFlag))
                        preferredPhoneSet = Convert.ToBoolean(phone.PreferredPhoneFlag);
                    if (!newPhone.Any(c => phone.PhoneNumber.Contains(c.PhoneNumber)))
                    {
                        newPhone.Add(new Phone()
                        {
                            PhoneNumber = formattedPhone,
                            PhoneTypeCode = phoneType.ToString(),
                            PreferredPhoneFlag = phone.PreferredPhoneFlag
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
            await referenceDataValidator.ValidateAsync(profile);
            
            if (profile.Qualifications != null) 
            {
                profile.Qualifications = await qualificationValidator.ValidateAsync(profile.Qualifications.ToList());
                qualificationValidator.CheckForDuplicates(profile.Qualifications.ToList());
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

        private bool ValidateLeftSchoolDetails(string monthCode, int? year)
        {
            //check month code
            if (!monthCode.IsNullOrEmpty() && !Enum.IsDefined(typeof(MonthCode), monthCode))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidMonthCode);

            //check valid year
            if (year.HasValue && (year < 1900 || year > DateTime.Now.Year))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidLeftSchoolYear);

            //if month exist, need year and vice versa
            if ((year.HasValue && monthCode.IsNullOrEmpty()) || (!monthCode.IsNullOrEmpty() && !year.HasValue))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidLeftSchoolDetails);

            return true;            
        }
    }
}