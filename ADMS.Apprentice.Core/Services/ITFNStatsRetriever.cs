using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages.TFN;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface ITFNStatsRetriever
    {
        IQueryable<ApprenticeTFN> RetrieveTfnStats(TFNStatsCriteria criteria);
    }
}
