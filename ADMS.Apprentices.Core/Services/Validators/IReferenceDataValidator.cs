using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IReferenceDataValidator
    {
        Task<ValidationExceptionBuilder> ValidateAsync(Profile profile);
        Task<ValidationExceptionBuilder> ValidatePriorQualificationsAsync(IQualificationAttributes qualification);
        Task<ValidationExceptionBuilder> ValidatePriorApprenticeshipQualificationsAsync(PriorApprenticeshipQualification priorApprenticeship);
    }
}