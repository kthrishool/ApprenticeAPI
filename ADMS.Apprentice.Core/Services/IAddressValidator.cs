using System.Collections.Generic;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface IAddressValidator
    {
        Task<Task> Validate(Profile message);
    }
}