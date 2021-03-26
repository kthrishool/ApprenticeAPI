using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface IProfileValidator
    {
        Task<Profile> ValidateAsync(Profile profile);
    }
}