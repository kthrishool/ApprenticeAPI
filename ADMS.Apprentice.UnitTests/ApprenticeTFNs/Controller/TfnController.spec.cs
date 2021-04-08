using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages.TFN;
using ADMS.Apprentice.Core.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ADMS.Apprentice.Api.Controllers.Tfn;
using Adms.Shared.Testing;

namespace ADMS.Apprentice.UnitTests.ApprenticeTFNs.Services
{
    #region WhenCreatingApprenticeTfn
    [TestClass]
    public class WhenCreatingApprenticeTfnUsingTheApi: GivenWhenThen<ApprenticeTFNController>
    {
        private ApprenticeTFN profile;
        private ActionResult<ApprenticeTFNV1> result;
        private ApprenticeTFNV1 message;       
        protected override void Given()
        {
            message = new ApprenticeTFNV1
            {
                ApprenticeId =1,
                TaxFileNumber = 123456789
            };

            profile = new ApprenticeTFN
            {
                Id = 1,
                ApprenticeId = 1,
                TaxFileNumber = "123456789"
            };

            Container
               .GetMock<IApprenticeTFNCreator>()
               .Setup(r => r.CreateAsync(message))
               .Returns(Task.FromResult(profile));

        }

        protected override async void When()
        {             
            result = await ClassUnderTest.Post(1, message);
        }

        [TestMethod]
        public void ShouldReturnResult()
        {
            result.Should().NotBeNull();
        }
    }

    #endregion
}