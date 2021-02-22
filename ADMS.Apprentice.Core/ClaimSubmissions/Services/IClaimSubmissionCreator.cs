using System.Threading.Tasks;
using ADMS.Apprentice.Core.ClaimSubmissions.Entities;
using ADMS.Apprentice.Core.ClaimSubmissions.Messages;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.ClaimSubmissions.Services
{
    [RegisterWithIocContainer]
    public interface IClaimSubmissionCreator
    {
        Task<ClaimSubmission> CreateAsync(ClaimSubmissionMessage message);
    }
}