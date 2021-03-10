using System;
using System.Xml.Linq;
using Adms.Shared;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

// ReSharper disable UnusedMember.Local

namespace Adms.Shared.UnitTests.Services
{
    public class CryptographyHelperTestsBase
    {
        protected Mock<IDateTimeHelper> mockDateTimeHelper;
        protected CryptographyHelper cryptographyHelper;

        protected const string clientId1 = "3431";
        protected const string clientId2 = "98765";
        protected const int keySum1 = 11;
        protected const int keySum2 = 35;

        public CryptographyHelperTestsBase()
        {
            mockDateTimeHelper = new Mock<IDateTimeHelper>();
            cryptographyHelper = new CryptographyHelper(mockDateTimeHelper.Object);

            var fakeDate = new DateTime(2018, 05, 15, 12, 53, 24);
            mockDateTimeHelper.Setup(o => o.GetDateTimeNow()).Returns(fakeDate);
        }
    }

    [TestClass]
    public class CryptographyHelperTests : CryptographyHelperTestsBase
    {
        [TestMethod]
        public void Asc()
        {
            var s = "�";

            var result = cryptographyHelper.Asc(s);

            Assert.AreEqual(65533, result);
        }
        [TestMethod]
        public void AscWithExtraCharacters()
        {
            var s = "�X";

            var result = cryptographyHelper.Asc(s);

            Assert.AreEqual(65533, result);
        }
        [TestMethod]
        public void AscEmpty()
        {
            var s = "";

            var result = Assert.ThrowsException<ArgumentException>(() => cryptographyHelper.Asc(s));

            Assert.AreEqual("Argument_LengthGTZero1", result.Message);
        }

        [TestMethod]
        public void Chr()
        {
            var result = cryptographyHelper.Chr(65533);

            Assert.AreEqual('�', result);
        }
        [TestMethod]
        public void AscB()
        {
            var result = cryptographyHelper.Asc("B");

            Assert.AreEqual(66, result);
        }
        [TestMethod]
        public void ChrB()
        {
            var result = cryptographyHelper.Chr(66);

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
        public void GetKeyClient1()
        {
            var result = cryptographyHelper.GetKey(clientId1, keySum1, 2);

            Assert.AreEqual("22686991018", result);
        }
        [TestMethod]
        public void GetKeyClient2()
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
        public void GetKeyClient2ForEncryption()
        {
            var result = cryptographyHelper.GetKey(clientId2, keySum2, -1);

            Assert.AreEqual("81942779305", result);
        }

        [TestMethod]
        public void GetKeyClient2ForEncryptionDifferentTime()
        {
            var fakeDate = new DateTime(2018, 05, 15, 12, 53, 33);
            mockDateTimeHelper.Setup(o => o.GetDateTimeNow()).Returns(fakeDate);

            var result = cryptographyHelper.GetKey(clientId2, keySum2, -1);

            Assert.AreNotEqual("81942779305", result);
        }


    }

}