using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface IProfileValidator
    {
        Task ValidateAsync(Profile profile);
    }
}