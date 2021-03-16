using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
	public interface IApprenticeTFNCreator
	{
		Task<ApprenticeTFN> CreateAsync(ApprenticeTFNV1 message);

	}
}