using ADMS.Apprentice.Core.Helpers;
using System;

namespace ADMS.Apprentice.Core.Services
{
    public class Cryptography : ICryptography
    {
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly IDateTimeHelper dateTimeHelper;

        public Cryptography(
            ICryptographyHelper cryptographyHelper,
            IDateTimeHelper dateTimeHelper
            )
        {
            this.cryptographyHelper = cryptographyHelper;
            this.dateTimeHelper = dateTimeHelper;
        }

        ////************************************************************************************
        //// EncryptTFN
        //// ----------
        //// This function takes the Clients ClientId and their TFN, uses the ClientId to
        //// calculate an encryption key and encrypts the TFN using this key against a merge of
        //// two of the four encryption strings. The final encrypted string stores the multiplier,
        //// the merge key string option and the merge key string reverse flag as well as the
        //// encrypted TFN. if the length of the ClientId is even, these three components are
        //// stored in the middle of the encrypted string, otherwise they are stored at the
        //// beginning of the string. The string also has a number of additional meaningless
        //// characters inserted in every second position at the start of the string. The
        //// number of these imbedded characters is determined by the length of the client id
        //// and the characters remaining in the TaxFileNumber field. This was included to give
        //// the TaxFileNumber encryption strings varying lengths.
        ////************************************************************************************

        public string EncryptTFN(string strClientId, string strTFN)
        {
            bool blnEven;
            int iEncryptionMerge;
            int iKeyLength;
            int iKeyPosition;
            int iLoop;
            int iMergeStringPosition;
            int iMultiplier;
            int iReverseFlag;
            int iTFNLength;
            int iTotal;
            string strEncryptedTFN;
            string strEncryptionKey;
            string strMergedString;

            blnEven = (strClientId.Length % 2) == 0;
            iTotal = cryptographyHelper.GetKeySum(strClientId);

            //// Determine which encryption strings to use
            iEncryptionMerge = dateTimeHelper.GetDateTimeNow().Second % 6;
            if (iEncryptionMerge == 0)
            {
                iEncryptionMerge = 6;
            }

            // Get the merged encryption strings
            iReverseFlag = (dateTimeHelper.GetDateTimeNow().Second * iTotal) % 2;
            strMergedString = cryptographyHelper.MergeKeyStrings(iEncryptionMerge, (iReverseFlag == 0));

            // Calculate encryption key and get the multiplier from that key
            strEncryptionKey = cryptographyHelper.GetKey(strClientId, iTotal, -1);

            Int32.TryParse(strEncryptionKey.Substring(0, 1), out int iOut);
            iMultiplier = iOut + iTotal;

            // Store the multiplier, merge option and merge reverse flag in the
            // middle of the encrypted TFN string for an even lengthed ClientID
            // and at the start for an odd one
            if (blnEven)
            {
                strEncryptedTFN = "";
            }
            else
            {
                strEncryptedTFN = new string(new[] { cryptographyHelper.Chr(iMultiplier), cryptographyHelper.Chr(iEncryptionMerge + iTotal), cryptographyHelper.Chr(iReverseFlag + iTotal) });
            }

            iTFNLength = strTFN.Length;
            iKeyLength = strEncryptionKey.Length;
            iKeyPosition = 1;

            // Encrypt each character of the TFN
            for (iLoop = strTFN.Length; iLoop > 0; iLoop--)
            {
                // Store key components in the middle of the string for even lengthed ClientIds
                if (blnEven)
                {
                    if (iLoop == (iTFNLength / 2))
                    {
                        strEncryptedTFN += new string(new[] { cryptographyHelper.Chr(iMultiplier), cryptographyHelper.Chr(iEncryptionMerge + iTotal), cryptographyHelper.Chr(iReverseFlag + iTotal) });
                    }
                }

                int.TryParse(strEncryptionKey.Substring(iKeyPosition, 1), out iOut);
                iMergeStringPosition = (iOut * iMultiplier) % 18;
                if (iMergeStringPosition == 0)
                {
                    iMergeStringPosition = 18;
                }

                int.TryParse(strTFN.Substring(iLoop - 1, 1), out iOut);
                var c = strMergedString.Substring(iMergeStringPosition - 1, 1);
                var d = cryptographyHelper.Asc(c);
                var e = cryptographyHelper.Chr(d + iOut);

                strEncryptedTFN += e;

                if (iKeyPosition == iKeyLength)
                {
                    iKeyPosition = 1;
                }
                else
                {
                    iKeyPosition += 1;
                }
            }

            // Embed additional characters to create varying lengthed encrypted TFNs
            strEncryptedTFN = cryptographyHelper.MergeAdd(strClientId, iTotal, strMergedString, strEncryptedTFN);

            return strEncryptedTFN;
        }

        //************************************************************************************
        // DecryptTFN
        // ----------
        // This function takes the Clients ClientId and their encrypted TFN, reconstructs the
        // original encryption key and merged encryption strings, removes the additional embedded
        // characters and decrypts the TFN. The multiplier, the merge key string option and the
        // merge key string reverse flag are extracted from the TFN encryption string.
        //************************************************************************************
        public string DecryptTFN(string strClientId, string strEncryptedTFN)
        {
            bool blnEven;
            int iEncryptionMerge;
            int iExtractPosition;
            int iKeyLength;
            int iKeyPosition;
            int iLoop;
            int iMergeAddCount;
            int iMergeStringPosition;
            int iMultiplier;
            int iReverseFlag;
            int iTFNLength;
            int iTotal;
            string strEncryptionKey;
            string strMergedString;
            string strNewEncryptedTFN;
            string strTemp;
            string strTFN;

            try
            {
                blnEven = (strClientId.Length % 2) == 0;
                iTotal = cryptographyHelper.GetKeySum(strClientId);

                // Determine how many additional characters embedded
                iMergeAddCount = cryptographyHelper.MergeAddCount(strClientId, iTotal);

                // Remove additional characters
                if (iMergeAddCount > 0)
                {
                    strTemp = "";

                    for (iLoop = 1; iLoop <= iMergeAddCount; iLoop++)
                    {
                        strTemp += strEncryptedTFN.Substring(iLoop * 2 - 2, 1);
                    }

                    strTemp += strEncryptedTFN.Substring(iMergeAddCount * 2);
                }
                else
                {
                    strTemp = strEncryptedTFN;
                }

                iTFNLength = strTemp.Length;

                // Extract multiplier, merge key string option and merge reverse flag
                if (blnEven)
                {
                    iExtractPosition = (iTFNLength - 3) - ((iTFNLength - 3) / 2) + 1;
                    strNewEncryptedTFN = strTemp.Substring(0, iExtractPosition - 1);
                }
                else
                {
                    iExtractPosition = 1;
                    strNewEncryptedTFN = "";
                }
                iMultiplier = cryptographyHelper.Asc(strTemp.Substring(iExtractPosition - 1, 1));
                iEncryptionMerge = cryptographyHelper.Asc(strTemp.Substring(iExtractPosition, 1)) - iTotal;
                iReverseFlag = cryptographyHelper.Asc(strTemp.Substring(iExtractPosition + 1, 1)) - iTotal;
                strNewEncryptedTFN += strTemp.Substring(iExtractPosition + 2);
                iTFNLength = strNewEncryptedTFN.Length;

                // Reconstruct merged keys string
                strMergedString = cryptographyHelper.MergeKeyStrings(iEncryptionMerge, (iReverseFlag == 0));

                // Re-calculate the encryption key
                strEncryptionKey = cryptographyHelper.GetKey(strClientId, iTotal, iMultiplier - iTotal);
                iKeyLength = strEncryptionKey.Length;
                iKeyPosition = 1;

                // Decrypt remaining encryption string
                strTFN = "";
                int iOut;
                for (iLoop = 0; iLoop < iTFNLength; iLoop++)
                {
                    Int32.TryParse(strEncryptionKey.Substring(iKeyPosition, 1), out iOut);
                    iMergeStringPosition = (iOut * iMultiplier) % 18;
                    if (iMergeStringPosition == 0)
                    {
                        iMergeStringPosition = 18;
                    }

                    var mid1 = strNewEncryptedTFN.Substring(iLoop, 1);
                    var asc1 = cryptographyHelper.Asc(mid1);
                    var mid2 = strMergedString.Substring(iMergeStringPosition - 1, 1);
                    var asc2 = cryptographyHelper.Asc(mid2);

                    strTFN = (asc1 - asc2).ToString() + strTFN;

                    if (iKeyPosition == iKeyLength - 1)
                    {
                        iKeyPosition = 1;
                    }
                    else
                    {
                        iKeyPosition = iKeyPosition + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errors encrypting TFN = " + ex.Message + Environment.NewLine + strEncryptedTFN);
                //Throw ex
            }

            return strTFN;
        }
    }
}