using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface ICryptography
    {
        string EncryptTFN(string strClientId, string strEncryptedTFN);
        string DecryptTFN(string strClientId, string strEncryptedTFN);
    }
}