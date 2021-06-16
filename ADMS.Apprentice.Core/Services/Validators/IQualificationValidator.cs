using System.Collections.Generic;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IQualificationValidator
    {
        Task<IValidatorExceptionBuilder> ValidateAsync(List<Qualification> qualifications);
        Task<IValidatorExceptionBuilder> ValidateAsync(Qualification qualifications);

        IValidatorExceptionBuilder CheckForDuplicates(List<Qualification> qualifications);
    }
}