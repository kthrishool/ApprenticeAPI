using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IReferenceDataValidator
    {
        Task<IValidatorExceptionBuilder> ValidateAsync(Profile profile);
        Task<IValidatorExceptionBuilder> ValidateAsync(Qualification qualification);
    }
}