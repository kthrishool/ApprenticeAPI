using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Models;

namespace ADMS.Apprentices.Core.Services
{
	[RegisterWithIocContainer]
	public interface IGuardianRetriever
	{
		Task<Guardian> GetAsync(int apprenticeId);
	}
}