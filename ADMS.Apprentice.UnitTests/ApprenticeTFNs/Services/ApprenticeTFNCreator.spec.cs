using System;
using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.Core.Interface;
using ADMS.Services.Infrastructure.Data;
using ADMS.Services.Infrastructure.Testing.Identity;
using ADMS.Services.Infrastructure.UnitTesting;
using Adms.Shared.Database;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IRepository = Adms.Shared.IRepository;

namespace ADMS.Apprentice.UnitTests.ApprenticeTFNs.Services
{
    #region WhenCreatingAApprenticeTFN
    [TestClass]
    public class WhenCreatingAApprenticeTFN : GivenWhenThen<ApprenticeTFNCreator>
    {        
        private ApprenticeTFN tfnDetail;
        private ApprenticeTFNV1 message;
        private DateTime currentDate;

        protected override void Given()
        {
            currentDate = DateTime.Now;
            message = new ApprenticeTFNV1
            {
                ApprenticeId = 1,
                TaxFileNumber = 123456789
            };

            var mockIContext = Container.GetMock<IContext>();
            mockIContext.Setup(x => x.DateTimeContext).Returns(currentDate);

            var repo = Container.GetMock<IRepository>();
            repo.Setup(x => x.Get<Profile>(message.ApprenticeId)).Returns(new Profile());
            //repo.Setup(x => x.Retrieve<ApprenticeTFN>()).Returns(new EnumerableQuery<ApprenticeTFN>(default(ApprenticeTFN)));

            //var mockIUser = Container.GetMock<IUser>();
            //mockIUser.Setup(x => x.EffectiveDateTime).Returns(currentDate);

            Container.GetMock<IContextRetriever>().Setup(x => x.GetContext()).Returns(mockIContext.Object);
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
            Container.GetMock<ICryptography>().Verify(r => r.EncryptTFN(message.ApprenticeId.ToString(), message.TaxFileNumber.ToString()));
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
            tfnDetail.StatusDate.Should().BeOnOrBefore(currentDate);
        }

    }

    #endregion
}