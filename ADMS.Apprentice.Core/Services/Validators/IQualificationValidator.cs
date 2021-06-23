using System.Collections.Generic;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.TYIMS.Entities;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IQualificationValidator
    {
        Task<ValidationExceptionBuilder> ValidateAsync(List<Qualification> qualifications);
        Task<ValidationExceptionBuilder> ValidateAsync(Qualification qualification);

        ValidationExceptionBuilder CheckForDuplicates(List<Qualification> qualifications);
        ValidationExceptionBuilder ValidateAgainstApprenticeshipQualification(Qualification qualification, Registration registration);
    }
}