using System;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using Adms.Shared;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.ApprenticeTFNs.Services
{
    #region WhenCreatingAApprenticeTFN
    [TestClass]
    public class WhenCreatingAApprenticeTFN : GivenWhenThen<ApprenticeTFNCreator>
    {        
        private ApprenticeTFN tfnDetail;
        private ApprenticeTFNV1 message;

        protected override void Given()
        {
            message = new ApprenticeTFNV1
            {
                ApprenticeId = 1,
                TaxFileNumber = "123456789"
            };
        }

        protected override void When()
        {
            tfnDetail = ClassUnderTest.CreateAsync(message).Result;
        }

        [TestMethod]
        public void ShouldReturnApprenticeTFN()
        {
            tfnDetail.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldAddTheApprenticeTFNToTheDatabase()
        {
            Container.GetMock<IRepository>().Verify(r => r.Insert(tfnDetail));
        }

        [TestMethod]
        public void ShouldEncryptTheTFNn()
        {
            Container.GetMock<ICryptography>().Verify(r => r.EncryptTFN(message.ApprenticeId.ToString(), message.TaxFileNumber));
        }

        [TestMethod]
        public void ShouldSetTheApprenticeId()
        {
            tfnDetail.ApprenticeId.Should().Be(message.ApprenticeId);
        }

        [TestMethod]
        public void ShouldSetDefaultValues()
        {
            tfnDetail.StatusCode.Should().Be(TFNStatus.New);
        }

    }

    #endregion
}