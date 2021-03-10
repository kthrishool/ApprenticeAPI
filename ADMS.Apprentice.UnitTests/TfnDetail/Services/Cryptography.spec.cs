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

namespace ADMS.Apprentice.UnitTests.Services
{
    #region TFN Encryptor tests

    [TestClass]
    public class WhenEncryptingTfn : CryptographyBase
    {

        protected const string tfn1 = "123123123";
        protected const string tfn2 = "874926891";

        [TestMethod]
        public void WhenEncryptingValidTfnForClient1()
        {
            cryptography.EncryptTFN(clientId1, tfn1).Should().Be("·­  +");
        }

        [TestMethod]
        public void WhenEncryptingValidTfnForClient2()
        {
            cryptography.EncryptTFN(clientId2, tfn1).Should().Be("+­)#ML,+ ");
        }

        [TestMethod]
        public void WhenEncryptingBlankClientId()
        {
            cryptography.EncryptTFN("", tfn1).Should().Be("\0");
        }

        [TestMethod]
        public void WhenEncryptingBlankTfn()
        {
            cryptography.EncryptTFN("", tfn1).Should().Be("\0");
        }

    }

    # endregion

    #region TFN Decryptor tests

    public class TFNDecryptorTests : CryptographyBase
    {
        protected const string clientId1 = "3431";
        protected const string clientId2 = "98765";
        protected const int keySum1 = 11;
        protected const int keySum2 = 35;

        [TestMethod]
        public void WhenDecryptingTheTFNClient1()
        {

            cryptography.DecryptTFN(clientId1, "¬Ã#ªºÁ¨§¦¤").Should().Be("123456789");
        }

        [TestMethod]
        public void WhenDecryptingTheTFNClient2()
        {
            cryptography.DecryptTFN(clientId2, "*&#$ºÄÃQÂ¾¼").Should().Be("123456789");
        }

        [TestMethod]
        public void WhenDecryptingTheTFNClient3()
        {
             cryptography.DecryptTFN("4406684", "#­% ÷>20ÿú1þ").Should().Be("874926891");
        }

        [TestMethod]
        public void WhenDecryptingTheTFNForClient4FromADifferentEncryptedTfn()
        {
            cryptography.DecryptTFN("4406684", "!­& µ&½¸1"). Should().Be("874926891");
        }

        [TestMethod]
        public void WhenDecryptingTheTFNForClient4WithTheWrongClientId()
        {
            cryptography.DecryptTFN("4406685", "!­& µ&½¸1").Should().Be("874926891");
        }
    }
    # endregion

    #region Base

    public class CryptographyBase : GivenWhenThen<Cryptography>
    {
        protected Mock<IDateTimeHelper> mockDateTimeHelper;
        protected CryptographyHelper cryptographyHelper;
        protected Cryptography cryptography;

        protected const string clientId1 = "3431";
        protected const string clientId2 = "98765";
        protected const string clientId3 = "4406684";

        public CryptographyBase()
        {
            var fakeDate = new DateTime(2021, 02, 25, 12, 53, 24);

            mockDateTimeHelper = new Mock<IDateTimeHelper>();
            mockDateTimeHelper.Setup(o => o.GetDateTimeNow()).Returns(fakeDate);
            cryptographyHelper = new CryptographyHelper(mockDateTimeHelper.Object);
            cryptography = new Cryptography(cryptographyHelper, mockDateTimeHelper.Object);
        }
    }


    # endregion

}