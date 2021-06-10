using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Models;

namespace ADMS.Apprentice.Core.Services
{
	[RegisterWithIocContainer]
	public interface IGuardianRetreiver
	{
		Task<Guardian> GetAsync(int apprenticeId);
	}
}