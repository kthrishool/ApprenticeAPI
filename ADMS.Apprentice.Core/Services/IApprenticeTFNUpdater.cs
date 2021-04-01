using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages.TFN;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
	public interface IApprenticeTFNUpdater
	{
		Task<ApprenticeTFN> SetRevalidate(int apprenticeId);
		Task<ApprenticeTFN> Update(ApprenticeTFNV1 message);
	}
}