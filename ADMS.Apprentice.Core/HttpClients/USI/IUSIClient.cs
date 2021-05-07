using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace ADMS.Apprentice.Core.HttpClients.USI
{    
    public interface IUSIClient
    {
        [Post("/usiverification")]
        [Headers("Authorization: Bearer")]
        public Task<List<VerifyUsiModel>> VerifyUsi(List<VerifyUsiMessage> verifyUsiMessages);
    }
}