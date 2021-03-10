using System;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using Adms.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.TfnDetails.Services
{
    #region WhenCreatingATfnDetail
    [TestClass]
    public class WhenCreatingATfnDetail : GivenWhenThen<TfnDetailCreator>
    {        
        private TfnDetail tfnDetail;
        private TfnCreateMessage message;

        protected override void Given()
        {
            message = new TfnCreateMessage
            {
                ApprenticeId = 1,
                TFN = "123456789"
            };
        }

        protected override void When()
        {
            tfnDetail = ClassUnderTest.CreateTfnDetailAsync(message).Result;
        }

        [TestMethod]
        public void ShouldReturnTfnDetail()
        {
            tfnDetail.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldAddTheTfnDetailToTheDatabase()
        {
            Container.GetMock<IRepository>().Verify(r => r.Insert(tfnDetail));
        }

        [TestMethod]
        public void ShouldSetTheTfn()
        {
            tfnDetail.TFN.Should().Be(message.TFN);
        }

        [TestMethod]
        public void ShouldSetTheApprenticeId()
        {
            tfnDetail.ApprenticeId.Should().Be(message.ApprenticeId);
        }

        [TestMethod]
        public void ShouldSetDefaultValues()
        {
            tfnDetail.Status.Should().Be(TfnStatus.New);
        }

        [TestMethod]
        public void ShouldCreateOneStatusHistoryRecord()
        {
            tfnDetail.TfnStatusHistories.Count.Should().Be(1);
        }



    }

    #endregion
}