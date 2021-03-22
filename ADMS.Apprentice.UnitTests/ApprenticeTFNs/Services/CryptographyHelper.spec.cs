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
    }

}