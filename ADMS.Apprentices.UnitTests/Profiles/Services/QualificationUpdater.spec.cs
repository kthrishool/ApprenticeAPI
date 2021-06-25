using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentices.Core.Services.Validators;
using Moq;
using Adms.Shared.Exceptions;
using ADMS.Apprentices.Core.TYIMS.Entities;
using System.Threading.Tasks;
using ADMS.Services.Infrastructure.Core.Exceptions;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenUpdatingAQualification

    [TestClass]
    public class WhenUpdatingAQualification : GivenWhenThen<QualificationUpdater>
    {
        private Qualification qualification;
        private ProfileQualificationMessage message;
        private int qualificationId;
        private int apprenticeshipId;
        private Profile profile;
        private Registration registration;

        protected override void Given()
        {
            qualificationId = 20;
            apprenticeshipId = 40;
            qualification = new Qualification() {
                Id = qualificationId,
                QualificationCode = "something",
             };
            var q = ProfileConstants.QualificationMessage;            
            message = new ProfileQualificationMessage()
                            {QualificationCode = q.QualificationCode, QualificationDescription = q.QualificationDescription,
                                StartDate = q.StartDate, EndDate = q.EndDate, ApprenticeshipId = apprenticeshipId };

            profile = new Profile();
            profile.Qualifications.Add(qualification);
            registration = new Registration();
            
            Container.GetMock<IExceptionFactory>()
                .Setup(s => s.CreateNotFoundException("Apprentice Qualification ", It.IsAny<string>()))
                .Returns(new NotFoundException(null, "test", "test"));
            Container.GetMock<IRepository>()
                .Setup(s => s.GetAsync<Profile>(It.IsAny<int>(), true))
                .ReturnsAsync(profile);
            Container.GetMock<ITYIMSRepository>()
                .Setup(s => s.GetRegistrationAsync(apprenticeshipId))
                .ReturnsAsync(registration);
            Container.GetMock<IQualificationValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<Qualification>()))
                .ReturnsAsync(new ValidationExceptionBuilder(Container.GetMock<IExceptionFactory>().Object));
            Container.GetMock<IQualificationValidator>()
                .Setup(s => s.ValidateAgainstApprenticeshipQualification(qualification, registration))
                .Returns(new ValidationExceptionBuilder(Container.GetMock<IExceptionFactory>().Object));
        }

        protected override void When()
        {
        }

        [TestMethod]
        public async Task SetsQualificationDetails()
        {
            qualification = await ClassUnderTest.Update(10, qualificationId, message);
            qualification.QualificationCode.Should().Be(message.QualificationCode);
            qualification.QualificationDescription.Should().Be(message.QualificationDescription);
            qualification.QualificationANZSCOCode.Should().Be(message.QualificationANZSCOCode);
            qualification.QualificationLevel.Should().Be(message.QualificationLevel);
        }

        [TestMethod]
        public async Task ShouldValidatesTheRequest()
        {
            qualification = await ClassUnderTest.Update(10, qualificationId, message);
            Container.GetMock<IQualificationValidator>().Verify(r => r.ValidateAsync(qualification));
        }

        [TestMethod]
        public void WhenQualificationIdIsDifferent_ThenAnExceptionShouldOccur()
        {
            ClassUnderTest.Invoking(c => c.Update(10, qualificationId+1, message))
                .Should().Throw<NotFoundException>();
        }
    }
    #endregion
}