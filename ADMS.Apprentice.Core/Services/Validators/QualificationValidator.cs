using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using Adms.Shared.Extensions;
using Adms.Shared.Exceptions;
using ADMS.Apprentice.Core.Helpers;
using System;
using System.Globalization;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public class QualificationValidator : IQualificationValidator
    {
        private readonly IExceptionFactory exceptionFactory;
        private readonly IReferenceDataValidator referenceDataValidator;

        public QualificationValidator(IExceptionFactory exceptionFactory, IReferenceDataValidator referenceDataValidator)     
        {            
            this.exceptionFactory = exceptionFactory;
            this.referenceDataValidator = referenceDataValidator;
        }

        public async Task<List<Qualification>> ValidateAsync(List<Qualification> qualifications)
        {
            var validQualifications = new List<Qualification>();

            foreach (Qualification qualification in qualifications)
            {
                //check if mandatory fields presents
                if (qualification.QualificationCode.IsNullOrEmpty() )
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                //check if the month is valid
                if (!qualification.StartMonth.IsNullOrEmpty() && !(Enum.IsDefined(typeof(MonthCode), qualification.StartMonth)))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);
                if (!qualification.EndMonth.IsNullOrEmpty() && !(Enum.IsDefined(typeof(MonthCode), qualification.EndMonth)))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                //Check if the year is valid
                if (qualification.StartYear.HasValue && (qualification.StartYear < 1900 || qualification.StartYear > DateTime.Now.Year))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                if (qualification.EndYear.HasValue && (qualification.EndYear < 1900 || qualification.EndYear > DateTime.Now.Year))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                //check if startyear <= endyear
                if (qualification.StartYear.HasValue && qualification.EndYear.HasValue && qualification.StartYear > qualification.EndYear)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                //if month exist, need year and vice versa
                if ((qualification.StartYear.HasValue && qualification.StartMonth.IsNullOrEmpty()) || (!qualification.StartMonth.IsNullOrEmpty() && !qualification.StartYear.HasValue))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                if ((qualification.EndYear.HasValue && qualification.EndMonth.IsNullOrEmpty()) || (!qualification.EndMonth.IsNullOrEmpty() && !qualification.EndYear.HasValue))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                //At this point we know we have a valid start and end month fields because month code has been validated against enum and year validations happened as well,
                //so create the date out of it if Months and Years exist.

                qualification.StartDate = qualification.StartYear.HasValue ? new DateTime(qualification.StartYear.Value, DateTime.ParseExact(qualification.StartMonth, "MMM", CultureInfo.CurrentCulture).Month, 1) : null;
                qualification.EndDate = qualification.EndYear.HasValue ? new DateTime(qualification.EndYear.Value, DateTime.ParseExact(qualification.EndMonth, "MMM", CultureInfo.CurrentCulture).Month, 1) : null;

                //check if StartDate < endDate if startDate and EndDate not null
                if (qualification.StartDate?.Date > qualification.EndDate?.Date)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                await referenceDataValidator.ValidateAsync(qualification);

                validQualifications.Add(qualification);                
            }

            return validQualifications;
        }

    }
}