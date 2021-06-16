using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;
using System.Collections.Generic;
using ADMS.Apprentice.Core.Exceptions;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IAddressValidator
    {
        Task<IValidatorExceptionBuilder> ValidateAsync(IAddressAttributes addresses);
    }
}