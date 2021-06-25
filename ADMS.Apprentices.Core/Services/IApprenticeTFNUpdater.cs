using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages.TFN;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
	public interface IApprenticeTFNUpdater
	{
		Task<ApprenticeTFN> SetRevalidate(int apprenticeId);
		Task<ApprenticeTFN> Update(ApprenticeTFNV1 message);
	}
}