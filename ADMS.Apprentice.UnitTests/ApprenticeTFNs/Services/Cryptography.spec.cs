using System;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.Services;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;

// ReSharper disable UnusedMember.Local

namespace ADMS.Apprentice.UnitTests.ApprenticeTFNs.Services
{
    #region TFN Encryptor tests

    [TestClass]
    public class TfnEncryptorTests : CryptographyBase
    {

        protected const string tfn1 = "123123123";
        protected const string tfn2 = "874926891";

        [TestMethod]
        public void WhenEncryptingValidTfnForClient1()
        {
            cryptography.EncryptTFN(clientId1, tfn1).Should().Be("");
        }

        [TestMethod]
        public void WhenEncryptingValidTfnForClient2()
        {
            cryptography.EncryptTFN(clientId2, tfn1).Should().Be("+­)#ML,+ ");
        }

        [TestMethod]
        public void WhenEncryptingDifferentValidTfnForClient2()
        {
            cryptography.EncryptTFN(clientId2, "987456369").Should().Be("+­)#!MN/.&");
        }

        [TestMethod]
        public void WhenEncryptingBlankClientId()
        {
            cryptography.EncryptTFN("", tfn1).Should().Be("\u0001\u0006\u0000");
        }

        [TestMethod]
        public void WhenEncryptingBlankTfn()
        {
            cryptography.EncryptTFN("", tfn1).Should().Be("\u0001\u0006\u0000");
        }
    }

    # endregion

    #region TFN Decryptor tests

    [TestClass]
    public class TFNDecryptorTests : CryptographyBase
    {
        protected const int keySum1 = 1;
        protected const int keySum2 = 35;

        [TestMethod]
        public void WhenDecryptingTheTFNClient1()
        {

            cryptography.DecryptTFN(clientId1, "").Should().Be("123123123");
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
            cryptography.DecryptTFN("4406685", "!­& µ&½¸1").Should().Be("121061471521011051071");
        }
    }
    #endregion

    #region TFN Encryptor Decryptor tests
    [TestClass]
    public class WhenEncryptingThenDecryptingTfn : CryptographyBase
    {

        protected const string tfn1 = "123123123";
        protected const string tfn2 = "874926891";

        [TestMethod]
        public void WhenEncryptingValidTfnForClient1()
        {
            var et = cryptography.EncryptTFN(clientId1, tfn1);
            var dt = cryptography.DecryptTFN(clientId1, et);

            dt.Should().Be(tfn1);
        }

        [TestMethod]
        public void WhenEncryptingValidTfnForClient9()
        {
            var tfn = "345345345";
            var et = cryptography.EncryptTFN("19", tfn);
            var dt = cryptography.DecryptTFN("19", et);

            byte[] bytes = Encoding.Default.GetBytes(et);
            var myString = Encoding.UTF8.GetString(bytes);

            et.Should().Be("\u0090­¸\u0093\u0096¢\u0097\u0010\u0010\n\u008e%\u008f ");

            dt.Should().Be(tfn);
        }
    }

    #endregion

    #region Base

    public class CryptographyBase : GivenWhenThen<Cryptography>
    {
        protected CryptographyHelper cryptographyHelper;
        protected Cryptography cryptography;

        protected const string clientId1 = "1";
        protected const string clientId2 = "98765";
        protected const string clientId3 = "4406684";

        public CryptographyBase()
        {
            cryptographyHelper = new CryptographyHelper();
            cryptography = new Cryptography(cryptographyHelper);
        }
    }


    # endregion

}