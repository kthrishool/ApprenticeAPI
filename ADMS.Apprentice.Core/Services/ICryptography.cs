namespace ADMS.Apprentice.Core.Services
{
    public interface ICryptography
    {
        string EncryptTFN(string strClientId, string strEncryptedTFN);
        string DecryptTFN(string strClientId, string strEncryptedTFN);
    }
}