using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
	public interface ITfnDetailCreator
	{
		Task<TfnDetail> CreateTfnDetailAsync(TFNV1 message);

	}
}