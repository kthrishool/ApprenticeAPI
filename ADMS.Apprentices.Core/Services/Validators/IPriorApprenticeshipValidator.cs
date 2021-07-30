using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IPriorApprenticeshipValidator
    {
        Task<ValidationExceptionBuilder> ValidateAsync(PriorApprenticeship priorApprenticeship, Profile profile);
    }
}