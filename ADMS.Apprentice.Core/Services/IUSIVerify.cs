using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Entities;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface IUSIVerify
    {     
        ApprenticeUSI Verify(Profile profile);
    }
}