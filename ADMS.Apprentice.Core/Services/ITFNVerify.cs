using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface ITFNVerify
    {
        bool MatchesChecksum(string tfn);
    }
}