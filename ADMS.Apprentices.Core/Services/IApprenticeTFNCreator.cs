using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages.TFN;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
	public interface IApprenticeTFNCreator
	{
		Task<ApprenticeTFN> CreateAsync(ApprenticeTFNV1 message);

	}
}