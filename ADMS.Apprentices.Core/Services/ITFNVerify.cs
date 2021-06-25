using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public interface ITFNVerify
    {
        bool MatchesChecksum(string tfn);
    }
}