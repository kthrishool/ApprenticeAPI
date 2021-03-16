using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Helpers
{
    [RegisterWithIocContainer]
    public interface ICryptographyHelper
    {
        string MergeKeyStrings(int iMerge, bool blnEven);
        int GetKeySum(string strClientId);
        int MergeAddCount(string strClientId, int iTotal);
        string MergeAdd(string strClientId, int iTotal, string strMergedString, string strEncryptedTFN);
        string GetKey(string strClientId, int iTotal, int iDecryptMultiplier);
        char Chr(int CharCode);
        int Asc(string String);

    }
}
