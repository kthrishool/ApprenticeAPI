using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Models;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
	public interface IApprenticeTFNRetreiver
	{
		ApprenticeTFNModel Get(int id);
	}
}