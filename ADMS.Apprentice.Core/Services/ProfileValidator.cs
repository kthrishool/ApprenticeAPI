using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;

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
            if(!(profile.ProfileTypeCode != null && Enum.IsDefined(typeof(ProfileType),  profile.ProfileTypeCode)))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidApprenticeprofileType);
            return Task.CompletedTask;
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