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
    #region WhenCreatingAPriorQualification

    [TestClass]
    public class WhenCreatingAPriorQualification : GivenWhenThen<PriorQualificationCreator>
    {
        private PriorQualification qualification;
        private PriorQualificationMessage message;
        private int apprenticeId;
        private Profile profile;
        private Registration registration;
        private int apprenticeshipId;

        protected override void Given()
        {
            profile = new Profile();
            apprenticeId = 1;
            apprenticeshipId = 11;
            profile.Id = apprenticeId;
            profile.PriorQualifications.Clear();
            var q = ProfileConstants.QualificationMessage;
            message = new PriorQualificationMessage()
            {
                QualificationCode = q.QualificationCode, QualificationDescription = q.QualificationDescription,
                StartDate = q.StartDate, EndDate = q.EndDate
            };

            registration = new Registration()
            {
                CurrentEndReasonCode = "CMPS",
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2020, 1, 1),
                RegistrationId = apprenticeshipId,
                QualificationCode = "QCode",
                TrainingContractId = 100,
            };

            Container.GetMock<ITYIMSRepository>()
                .Setup(s => s.GetCompletedRegistrationsByApprenticeIdAsync(apprenticeId))
                .ReturnsAsync(registration);
            Container.GetMock<IRepository>()
                .Setup(s => s.GetAsync<Profile>(apprenticeId, true))
                .ReturnsAsync(profile);
            Container.GetMock<IQualificationValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<IQualificationAttributes>(), It.IsAny<Profile>()))
                .ReturnsAsync(new ValidationExceptionBuilder());
        }

        protected override void When()
        {
        }

        [TestMethod]
        public async Task ShouldReturnQualification()
        {
            qualification = await ClassUnderTest.CreateAsync(apprenticeId, message);
            qualification.Should().NotBeNull();
        }


        [TestMethod]
        public async Task ShouldValidatesTheQualificationRequest()
        {
            qualification = await ClassUnderTest.CreateAsync(apprenticeId, message);
            Container.GetMock<IQualificationValidator>().Verify(r => r.ValidateAsync(qualification, profile));
        }

        [TestMethod]
        public async Task ShouldSetTheDetails()
        {
            qualification = await ClassUnderTest.CreateAsync(apprenticeId, message);
            qualification.QualificationCode.Should().Be(message.QualificationCode);
            qualification.QualificationDescription.Should().Be(message.QualificationDescription);
            qualification.QualificationANZSCOCode.Should().Be(message.QualificationANZSCOCode);
            qualification.QualificationLevel.Should().Be(message.QualificationLevel);
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