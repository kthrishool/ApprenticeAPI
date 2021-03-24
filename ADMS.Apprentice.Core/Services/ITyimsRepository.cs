using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Services
{
    public interface ITyimsRepository
    {
        Task<List<CodeLocalityPostcodesState>> GetPostCodeAsync(string PostCodeId);
    }
}