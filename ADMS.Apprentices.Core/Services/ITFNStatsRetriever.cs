using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages.TFN;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public interface ITFNStatsRetriever
    {
        IQueryable<ApprenticeTFN> RetrieveTfnStats(TFNStatsCriteria criteria);
    }
}
