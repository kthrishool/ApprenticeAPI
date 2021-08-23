using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.Core.TYIMS.Entities;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenUpdatingAPriorApprenticeshipQualification

    [TestClass]
    public class WhenUpdatingAPriorApprenticeshipQualification : GivenWhenThen<PriorApprenticeshipQualificationUpdater>
    {
        private PriorApprenticeshipQualification qualification;
        private PriorApprenticeshipQualificationMessage message;
        private int qualificationId;

        private Profile profile;
        private Registration registration;

        protected override void Given()
        {
            qualificationId = 20;
            qualification = new PriorApprenticeshipQualification
            {
                Id = qualificationId,
                QualificationCode = "something",
            };
            var q = ProfileConstants.QualificationMessage;
            message = new PriorApprenticeshipQualificationMessage
            {
                QualificationCode = q.QualificationCode,
                QualificationDescription = q.QualificationDescription,
                StartDate = q.StartDate,
                NotOnTrainingGovAu = true
            };

            profile = new Profile();
            profile.PriorApprenticeshipQualifications.Add(qualification);
            registration = new Registration();

            Container.GetMock<IRepository>()
                .Setup(s => s.GetAsync<Profile>(It.IsAny<int>(), true))
                .ReturnsAsync(profile);
            ChangeRegistrationDetails(ProfileConstants.Id);
            Container.GetMock<IPriorApprenticeshipQualificationValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<PriorApprenticeshipQualification>(), It.IsAny<Profile>()))
                .ReturnsAsync(new ValidationExceptionBuilder());
        }

        private void ChangeRegistrationDetails(int id)
        {
            Container.GetMock<ITYIMSRepository>()
                .Setup(s => s.GetCompletedRegistrationsByApprenticeIdAsync(id))
                .ReturnsAsync(registration);
        }

        [TestMethod]
        public async Task SetsQualificationDetails()
        {
            qualification = await ClassUnderTest.Update(10, qualificationId, message, profile);
            qualification.QualificationCode.Should().Be(message.QualificationCode);
            qualification.QualificationDescription.Should().Be(message.QualificationDescription);
            qualification.QualificationANZSCOCode.Should().Be(message.QualificationANZSCOCode);
            qualification.QualificationLevel.Should().Be(message.QualificationLevel);
            qualification.NotOnTrainingGovAu.Should().BeTrue();
        }

        [TestMethod]
        public void WhenQualificationIdIsDifferent_ThenAnExceptionShouldOccur()
        {
            ClassUnderTest.Invoking(c => c.Update(10, qualificationId + 1, message, profile))
                .Should().Throw<AdmsNotFoundException>();
        }
    }

    #endregion
}