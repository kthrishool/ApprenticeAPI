using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Attributes;
using System.Collections.Generic;
using ADMS.Apprentices.Core.Exceptions;

namespace ADMS.Apprentices.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IAddressValidator
    {
        Task<ValidationExceptionBuilder> ValidateAsync(IAddressAttributes addresses);
    }
}