using Adms.Shared.Attributes;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface IUSIVerify
    {
        Task VerifyAsync(int apprenticeId, string usi);
    }
}