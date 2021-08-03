using System;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.Core.TYIMS.Entities;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenCreatingAQualification

    [TestClass]
    public class WhenCreatingAPriorApprenticeship : GivenWhenThen<PriorApprenticeshipCreator>
    {
        private PriorApprenticeship priorApprenticeship;
        private ProfilePriorApprenticeshipMessage message;
        private readonly int apprenticeId = ProfileConstants.Id;
        private Profile profile;
        private Registration registration;


        protected override void Given()
        {
            profile = new Profile
            {
                Id = ProfileConstants.Id
            };
            profile.Qualifications.Clear();
            var q = ProfileConstants.QualificationMessage;
            message = new ProfilePriorApprenticeshipMessage()
            {
                QualificationCode = q.QualificationCode, QualificationDescription = q.QualificationDescription,
                StartDate = q.StartDate, EndDate = q.EndDate
            };

            registration = new Registration()
            {
                CurrentEndReasonCode = "CMPS",
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2020, 1, 1),
                RegistrationId = ProfileConstants.Id,
                QualificationCode = "QCode",
                TrainingContractId = 100,
            };

            Container.GetMock<ITYIMSRepository>()
                .Setup(s => s.GetCompletedRegistrationsByApprenticeIdAsync(apprenticeId))
                .ReturnsAsync(registration);
            Container.GetMock<IRepository>()
                .Setup(s => s.GetAsync<Profile>(apprenticeId, true))
                .ReturnsAsync(profile);
            Container.GetMock<IPriorApprenticeshipValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<PriorApprenticeship>(), It.IsAny<Profile>()))
                .ReturnsAsync(new ValidationExceptionBuilder());
        }

        protected override void When()
        {
        }

        [TestMethod]
        public async Task ShouldReturnQualification()
        {
            priorApprenticeship = await ClassUnderTest.CreateAsync(apprenticeId, message);
            priorApprenticeship.Should().NotBeNull();
        }


        [TestMethod]
        public async Task ShouldSetTheDetails()
        {
            priorApprenticeship = await ClassUnderTest.CreateAsync(apprenticeId, message);
            priorApprenticeship.QualificationCode.Should().Be(message.QualificationCode);
            priorApprenticeship.QualificationDescription.Should().Be(message.QualificationDescription);
            priorApprenticeship.QualificationANZSCOCode.Should().Be(message.QualificationANZSCOCode);
            priorApprenticeship.QualificationLevel.Should().Be(message.QualificationLevel);
        }

        [TestMethod]
        public void WhenApprenticeshipIdIsPopulated_ThenTheRegistrationShouldBeRetrieved()
        {
            ClassUnderTest.Invoking(c => c.CreateAsync(apprenticeId, message)).Invoke();
            Container.GetMock<ITYIMSRepository>().Verify();
        }

        private void ChangeRegistrationDetails(int id)
        {
            Container.GetMock<ITYIMSRepository>()
                .Setup(s => s.GetCompletedRegistrationsByApprenticeIdAsync(id))
                .ThrowsAsync(new ArgumentOutOfRangeException());
        }
    }

    #endregion
}