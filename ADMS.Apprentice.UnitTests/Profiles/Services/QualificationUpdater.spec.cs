using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using Adms.Shared;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentice.Core.Services.Validators;
using Moq;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenUpdatingAQualification

    [TestClass]
    public class WhenUpdatingAQualification : GivenWhenThen<QualificationUpdater>
    {
        private Qualification qualification;
        private ProfileQualificationMessage message;

        protected override void Given()
        {
            qualification = new Qualification();
            qualification.QualificationCode = "something";
            message = ProfileConstants.QualificationMessage;
            Container.GetMock<IQualificationValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<Qualification>()))
                .ReturnsAsync(new ValidatorExceptionBuilder(Container.GetMock<IExceptionFactory>().Object));
        }

        protected override async void When()
        {
            qualification = await ClassUnderTest.Update(qualification, message);
        }

        [TestMethod]
        public void SetsQualificationDetails()
        {
            qualification.QualificationCode.Should().Be(message.QualificationCode);
            qualification.QualificationDescription.Should().Be(message.QualificationDescription);
            qualification.QualificationANZSCOCode.Should().Be(message.QualificationANZSCOCode);
            qualification.QualificationLevel.Should().Be(message.QualificationLevel);
        }

        [TestMethod]
        public void ShouldValidatesTheRequest()
        {
            Container.GetMock<IQualificationValidator>().Verify(r => r.ValidateAsync(qualification));
        }
    }
    #endregion
}