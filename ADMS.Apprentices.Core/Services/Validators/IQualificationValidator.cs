using System.Collections.Generic;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Attributes;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.TYIMS.Entities;

namespace ADMS.Apprentices.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IQualificationValidator
    {
        //Task<ValidationExceptionBuilder> ValidateAsync(List<Qualification> qualifications);
        Task<ValidationExceptionBuilder> ValidateAsync(Qualification qualification, Profile profile);
        ValidationExceptionBuilder CheckForDuplicates(List<Qualification> qualifications);
        ValidationExceptionBuilder ValidateAgainstApprenticeshipQualification(Qualification qualification, Registration registration);
    }
}