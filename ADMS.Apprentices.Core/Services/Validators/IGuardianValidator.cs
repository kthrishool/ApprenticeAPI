using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Attributes;
using System.Threading.Tasks;

namespace ADMS.Apprentices.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IGuardianValidator
    {
        Task<ValidationExceptionBuilder> ValidateAsync(Guardian guardian);
    }
}