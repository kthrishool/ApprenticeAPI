using System;
using System.Collections.Generic;
using System.Text;

namespace ADMS.Apprentice.Core.Helpers
{

    public class CryptographyHelper : ICryptographyHelper
    {
        private const string mstrModuleName = "mdlClientTFN";
        private const int giTFNFieldSize = 20;  // Size of the varchar TaxFileNumber field on the CLIENTTFN table
        private const int giTFNMaxSize = 9;     // Maximum size of a Tax File Number string (assuming no spaces)

        public CryptographyHelper()
        {
        }


        public string MergeKeyStrings(int iMerge, bool blnEven)
        {
            int iEnd;
            int iIncrement;
            int iStart;
            string strMergedString;
            string strMKey1 = "";
            string strMKey2 = "";
            string strKey1;
            string strKey2;
            string strKey3;
            string strKey4;
            string strTempKey;

            var key1 = new[] {ChrW(172), ChrW(35), ChrW(115), ChrW(23) , ChrW(97) , ChrW(12) ,
                   ChrW(219) , ChrW(138) , ChrW(242) , ChrW(161) , ChrW(51) , ChrW(163) ,
                   ChrW(212) , ChrW(231) , ChrW(45) , ChrW(148) , ChrW(168) , ChrW(187)};

            var key2 = new[] {ChrW(6) , ChrW(76) , ChrW(246) , ChrW(171) , ChrW(226) , ChrW(237) ,
                   ChrW(170) , ChrW(5) , ChrW(207) , ChrW(46) , ChrW(209) , ChrW(221) ,
                   ChrW(71) , ChrW(33) , ChrW(69) , ChrW(220) , ChrW(123) , ChrW(37) };

            var key3 = new[] {ChrW(136) , ChrW(107) , ChrW(10) , ChrW(3) , ChrW(139) , ChrW(195) ,
                   ChrW(29) , ChrW(215) , ChrW(75) , ChrW(243) , ChrW(32) , ChrW(197) ,
                   ChrW(180) , ChrW(119) , ChrW(157) , ChrW(175) , ChrW(147) , ChrW(201) };

            var key4 = new[] {ChrW(7) , ChrW(53) , ChrW(186) , ChrW(41) , ChrW(81) , ChrW(42) ,
                   ChrW(156) , ChrW(128) , ChrW(150) , ChrW(137) , ChrW(190) , ChrW(24) ,
                   ChrW(218) , ChrW(77) , ChrW(227) , ChrW(30) , ChrW(200) , ChrW(173) };

            strKey1 = new string(key1);
            strKey2 = new string(key2);
            strKey3 = new string(key3);
            strKey4 = new string(key4);

            strMergedString = "";

            // Set key string 1
            switch (iMerge)
            {

                case 1:
                case 2:
                case 3:
                    strMKey1 = strKey1;
                    break;
                case 4:
                case 5:
                    strMKey1 = strKey2;
                    break;

                case 6:
                    strMKey1 = strKey3;
                    break;
            }

            // Set key string 2
            switch (iMerge)
            {
                case 1:
                    strMKey2 = strKey2;
                    break;
                case 2:
                case 4:
                    strMKey2 = strKey3;
                    break;

                case 3:
                case 5:
                case 6:
                    strMKey2 = strKey4;
                    break;
            }

            // Reverse the order if total is even
            if (blnEven)
            {
                strTempKey = strMKey1;
                strMKey1 = strMKey2;
                strMKey2 = strTempKey;
                iStart = 18;
                iEnd = 0;
                iIncrement = -1;
            }
            else
            {
                iStart = 1;
                iEnd = 19;
                iIncrement = 1;
            }

            // Merge the two key strings into one 18 character string
            for (int iLoop = iStart; iLoop != iEnd; iLoop += iIncrement)
            {
                if (iLoop % 2 == 0)
                {
                    strMergedString += strMKey1.Substring(iLoop-1, 1);
                }
                else
                {
                    strMergedString += strMKey2.Substring(iLoop-1, 1);
                }
            }

            return strMergedString;
        }

        ////**************************************************************************
        //// GetKeySum
        //// ---------
        //// Calculate the sum of all the digits making up the Clients ClientId
        ////**************************************************************************
        public int GetKeySum(string strClientId)
        {
            int iLength;
            int iTotal;
            int iOut;

            iLength = strClientId.Length;
            iTotal = 0;

            for (int iLoop = 0; iLoop < iLength; iLoop++)
            {
                Int32.TryParse(strClientId.Substring(iLoop, 1), out iOut);
                iTotal += iOut;
            }

            return iTotal;
        }


        public int MergeAddCount(string strClientId, int iTotal)
        {
            int part1 = (int)Math.Round(((strClientId.Length * iTotal) / 13.0));
            int part2 = (giTFNFieldSize - giTFNMaxSize - 3);
            return part1 % part2;
        }

        ////**************************************************************************
        //// MergeAdd
        //// --------
        //// This routine takes the already encrypted TFN and imbeds extra characters
        //// into the string. The number of extra characters imbedded is determined
        //// by multiplying the ClientId by the sum of all the digits in the ClientId,
        //// dividing by 13 and then using the remainder of that value divided by
        //// the number of characters left to be filled in the TaxFileNumber field
        ////**************************************************************************
        public string MergeAdd(string strClientId, int iTotal, string strMergedString, string strEncryptedTFN)
        {
            int iMergeAddCount;
            string strNewEncryptedTFN;

            //// Subtract 3 from the difference between the field size and maximum size of a TFN to
            //// take into account the storage of the encryption multiplier, the encryption merge string
            //// option and the encryption merge string order flag
            iMergeAddCount = MergeAddCount(strClientId, iTotal);
            strNewEncryptedTFN = "";

            if (iMergeAddCount > 0)
            {
                for (int iLoop = 0; iLoop < iMergeAddCount; iLoop++)
                {
                    strNewEncryptedTFN += strEncryptedTFN.Substring(iLoop, 1);
                    strNewEncryptedTFN += strMergedString.Substring(iLoop, 1);
                }

                strEncryptedTFN = strNewEncryptedTFN + strEncryptedTFN.Substring(iMergeAddCount);
            }

            return strEncryptedTFN;
        }

        ////**************************************************************************************************
        //// GetKey
        //// ------
        //// This function takes the Clients ClientId and calculates an encryption key to be used against
        //// the merged encryption strings when building the encrypted TFN. The function is called both when
        //// encrypting the TFN and when decrypting the TFN. When decrypting the TFN, the decryption multiplier
        //// is extracted from the encrypted TFN string, otherwise the multiplier is created using the seconds
        //// portion of the current time.
        ////**************************************************************************************************
        public string GetKey(string strClientId, int iTotal, int iDecryptMultiplier)
        {
            int iLength;
            int iLoop;
            int iModTest;
            int iMultiplier;
            string strKey;

            iLength = strClientId.Length;

            //// Use % 13 to determine a multiplier unless the total is 13.
            //// Where the total is 13 use % 17 to prevent an all 0 encryption key
            if (iTotal == 13)
            {
                iModTest = 17;
            }
            else
            {
                iModTest = 13;
            }

            var seconds = 24;

            if (iDecryptMultiplier == -1)
            {
                iMultiplier = ((seconds * iTotal) % iModTest) % 10;

                if (iMultiplier == 0)
                {
                    iMultiplier = 1;
                }
            }
            else
            {
                iMultiplier = iDecryptMultiplier;
            }

            strKey = iMultiplier.ToString();

            int iOut = 0;
            for (iLoop = iLength; iLoop > 0; iLoop--)
            {
                Int32.TryParse(strClientId.Substring(iLoop-1, 1), out iOut);
                strKey += (((iOut * iMultiplier) % iModTest) % 10).ToString();
            }

            if (iLength < 10)
            {
                for (iLoop = 0; iLoop < 10 - iLength; iLoop++)
                {
                    Int32.TryParse(strKey.Substring(iLoop, 1), out iOut);

                    strKey += (((iOut * iTotal) % iModTest) % 10).ToString();
                }
            }

            return strKey;
        }

        /// <summary>
        /// Returns the character associated with the specified character code.
        /// </summary>
        /// 
        /// <returns>
        /// Returns the character associated with the specified character code.
        /// </returns>
        /// <param name="CharCode">Required. An Integer expression representing the <paramref name="code point"/>, or character code, for the character.</param><exception cref="T:System.ArgumentException"><paramref name="CharCode"/> &lt; 0 or &gt; 255 for Chr.</exception><filterpriority>1</filterpriority>
        public char ChrW(int CharCode)
        {
            return Microsoft.VisualBasic.Strings.ChrW(CharCode);
        }

        /// <summary>
        /// Returns an Integer value representing the character code corresponding to a character.
        /// </summary>
        /// 
        /// <returns>
        /// Returns an Integer value representing the character code corresponding to a character.
        /// </returns>
        /// <param name="String">Required. Any valid Char or String expression. If <paramref name="String"/> is a String expression, only the first character of the string is used for input. 
        /// If <paramref name="String"/> is Nothing or contains no characters, an <see cref="T:System.ArgumentException"/> error occurs.</param><filterpriority>1</filterpriority>
        /// 
        public int AscW(string String)
        {

            if (String == null || String.Length == 0)
                throw new ArgumentException("Argument_LengthGTZero1");

            return Microsoft.VisualBasic.Strings.AscW(String[0]);

        }


    }
}
