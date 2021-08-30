using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IProfileValidator
    {
        Task<ValidationExceptionBuilder> ValidateAsync(Profile profile);       
    }
}