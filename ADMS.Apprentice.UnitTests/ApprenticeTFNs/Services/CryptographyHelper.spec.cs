using System;
using ADMS.Apprentice.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

// ReSharper disable UnusedMember.Local

namespace ADMS.Apprentice.UnitTests.ApprenticeTFNs.Services
{
    public class CryptographyHelperTestsBase
    {
        protected CryptographyHelper cryptographyHelper;

        protected const string clientId1 = "1";
        protected const string clientId2 = "98765";
        protected const int keySum1 = 1;
        protected const int keySum2 = 35;

        public CryptographyHelperTestsBase()
        {
            cryptographyHelper = new CryptographyHelper();
        }
    }

    [TestClass]
    public class CryptographyHelperTests : CryptographyHelperTestsBase
    {
        [TestMethod]
        public void Asc()
        {
            var s = "�";

            var result = cryptographyHelper.AscW(s);

            Assert.AreEqual(65533, result);
        }
        [TestMethod]
        public void AscWithExtraCharacters()
        {
            var s = "�X";

            var result = cryptographyHelper.AscW(s);

            Assert.AreEqual(65533, result);
        }
        [TestMethod]
        public void AscEmpty()
        {
            var s = "";

            var result = Assert.ThrowsException<ArgumentException>(() => cryptographyHelper.AscW(s));

            Assert.AreEqual("Argument_LengthGTZero1", result.Message);
        }

        [TestMethod]
        public void AscNull()
        {
            string s = null;

            var result = Assert.ThrowsException<ArgumentException>(() => cryptographyHelper.AscW(s));

            Assert.AreEqual("Argument_LengthGTZero1", result.Message);
        }

        [TestMethod]
        public void Chr()
        {
            var result = cryptographyHelper.ChrW(65533);

            Assert.AreEqual('�', result);
        }
        [TestMethod]
        public void AscB()
        {
            var result = cryptographyHelper.AscW("B");

            Assert.AreEqual(66, result);
        }
        [TestMethod]
        public void ChrB()
        {
            var result = cryptographyHelper.ChrW(66);

            Assert.AreEqual('B', result);
        }

        [TestMethod]
        public void GetKeySumClient1()
        {
            var s = clientId1;

            var result = cryptographyHelper.GetKeySum(s);

            Assert.AreEqual(keySum1, result);
        }
        [TestMethod]
        public void GetKeySumClient2()
        {
            var s = clientId2;

            var result = cryptographyHelper.GetKeySum(s);

            Assert.AreEqual(keySum2, result);
        }

        [TestMethod]
        public void GetKeyClient1ForDecryption()
        {
            var result = cryptographyHelper.GetKey(clientId1, keySum1, 2);

            Assert.AreEqual("22222222222", result);
        }
        [TestMethod]
        public void GetKeyClient2ForDecryption()
        {
            var result = cryptographyHelper.GetKey(clientId2, keySum2, 11);

            Assert.AreEqual("113120899195", result);
        }
        [TestMethod]
        public void GetKeyClient2WithTheWrongKeySum()
        {
            var result = cryptographyHelper.GetKey(clientId2, keySum1, 11);

            Assert.AreNotEqual("113120899195", result);
        }

        [TestMethod]
        public void GetKeyClient2WithTheWrongMultiplier()
        {
            var result = cryptographyHelper.GetKey(clientId2, keySum2, 9);

            Assert.AreNotEqual("113120899195", result);
        }
        [TestMethod]
        public void GetKeyClient1iTotal13()
        {
            var result = cryptographyHelper.GetKey(clientId1, 13, 2);

            Assert.AreEqual("22995544113", result);
        }

        [TestMethod]
        public void GetKeyClient1ForEncryption()
        {
            var result = cryptographyHelper.GetKey(clientId1, keySum1, -1);

            Assert.AreEqual("11111111111", result);
        }

        [TestMethod]
        public void GetKeyClient2ForEncryption()
        {
            var result = cryptographyHelper.GetKey(clientId2, keySum2, -1);

            Assert.AreEqual("81942779305", result);
        }

        [TestMethod]
        public void MergeKeyStrings1Even()
        {
            var key = "%¨Ü-!ÔÝ3.òÛía«sL¬";

            var result = cryptographyHelper.MergeKeyStrings(1, true);

            Assert.AreEqual(key, result);
        }
        [TestMethod]
        public void MergeKeyStrings4False()
        {
            var key= "\u0088L\n«\u008bí\u001d\u0005K. Ý´!\u009dÜ\u0093%";

            var result = cryptographyHelper.MergeKeyStrings(4, false);

            Assert.AreEqual(key, result);
        }
        [TestMethod]
        public void MergeKeyStrings6Even()
        {
            var key= "­\u0093\u001e\u009dM´\u0018 \u0089K\u0080\u001d*\u008b)\n5\u0088";
            var result = cryptographyHelper.MergeKeyStrings(6, true);

            Assert.AreEqual(key, result);
        }
    }

}