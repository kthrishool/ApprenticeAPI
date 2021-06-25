using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Entities;
using System.Threading.Tasks;

namespace ADMS.Apprentices.Core.Services
{
    public interface IUSIVerify
    {     
        ApprenticeUSI Verify(Profile profile);
    }
}