using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IGuardianValidator
    {
        Task ValidateAsync(Guardian guardian);
    }
}