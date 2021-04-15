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

namespace ADMS.Apprentice.Core.Services
{
    public class QualificationValidator : IQualificationValidator
    {
        private readonly IExceptionFactory exceptionFactory;        

        public QualificationValidator(IExceptionFactory exceptionFactory)     
        {            
            this.exceptionFactory = exceptionFactory;            
        }

        public async Task<List<Qualification>> ValidateAsync(List<Qualification> qualifications)
        {
            var validQualifications = new List<Qualification>();

            foreach (Qualification qualification in qualifications)
            {
                //check if mandatory fields presents
                if (qualification.QualificationCode.IsNullOrEmpty() ||
                    qualification.StartMonth.IsNullOrEmpty() || qualification.StartYear.IsNullOrEmpty() ||
                    qualification.EndMonth.IsNullOrEmpty() || qualification.EndYear.IsNullOrEmpty())
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                //check if the month is valid
                if (!(Enum.IsDefined(typeof(MonthCode), qualification.StartMonth) && Enum.IsDefined(typeof(MonthCode), qualification.EndMonth)))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                //Check if the year is valid
                if (!(int.TryParse(qualification.StartYear, out int startYear) && startYear >= 1900 && startYear <= DateTime.Now.Year))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);
                
                if (!(int.TryParse(qualification.EndYear, out int endYear) && endYear >= 1900 && endYear <= DateTime.Now.Year))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                //check if startyear <= endyear
                if (!(int.TryParse(qualification.StartYear, out startYear) && int.TryParse(qualification.EndYear, out endYear) && startYear <= endYear))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);
                
                //At this point we know we have a valid start and end month fields because month code has been validated from reference data and year validations happened as well,
                //so create the date out of it.                
                qualification.StartDate = new DateTime(int.Parse(qualification.StartYear), DateTime.ParseExact(qualification.StartMonth, "MMM", CultureInfo.CurrentCulture).Month, 1);
                qualification.EndDate = new DateTime(int.Parse(qualification.EndYear), DateTime.ParseExact(qualification.EndMonth, "MMM", CultureInfo.CurrentCulture).Month, 1);
                
                //check if StartDate < endDate
                if (qualification.StartDate > qualification.EndDate)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

                validQualifications.Add(qualification);                
            }

            return validQualifications;
        }

    }
}