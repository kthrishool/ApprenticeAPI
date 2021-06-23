using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IProfileValidator
    {
        Task<ValidationExceptionBuilder> ValidateAsync(Profile profile);

       
    }
}