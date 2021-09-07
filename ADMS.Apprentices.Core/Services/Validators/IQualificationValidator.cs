using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Services.Validators
{
    /* NOTE: Do not use [RegisterWithIocContainer] as this interface has two implementations */
    public interface IQualificationValidator
    {
        ValidationExceptionBuilder CheckForDuplicates(List<PriorQualification> qualifications);
        ValidationExceptionBuilder ValidatePriorQualification(PriorQualification qualification, Profile profile);
    }
}