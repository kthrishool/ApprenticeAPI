﻿using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Adms.Shared.Exceptions;
using Moq;
using System.Collections.Generic;
using ADMS.Apprentices.Core.TYIMS.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenCreatingAQualification

    [TestClass]
    public class WhenCreatingAQualification : GivenWhenThen<QualificationCreator>
    {
        private Qualification qualification;
        private ProfileQualificationMessage message;
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
            profile.Qualifications.Clear();
            var q = ProfileConstants.QualificationMessage;            
            message = new ProfileQualificationMessage()
                            {QualificationCode = q.QualificationCode, QualificationDescription = q.QualificationDescription,
                                StartDate = q.StartDate, EndDate = q.EndDate, ApprenticeshipId = apprenticeshipId };

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
                .Setup(s => s.GetRegistrationAsync(apprenticeshipId))
                .ReturnsAsync(registration);
            Container.GetMock<IRepository>()
                .Setup(s => s.GetAsync<Profile>(apprenticeId, true))
                .ReturnsAsync(profile);
            Container.GetMock<IQualificationValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<Qualification>(), It.IsAny<Profile>()))
                .ReturnsAsync(new ValidationExceptionBuilder());
            Container.GetMock<IQualificationValidator>()
                .Setup(s => s.ValidateAgainstApprenticeshipQualification(It.IsAny<Qualification>(), registration,It.IsAny<Profile>()))
                .Returns(new ValidationExceptionBuilder());

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
            message.ApprenticeshipId = apprenticeshipId;
            ClassUnderTest.Invoking(c => c.CreateAsync(apprenticeId, message)).Invoke();
            Container.GetMock<ITYIMSRepository>().Verify();
        }

        [TestMethod]
        public void WhenApprenticeshipIdIsPopulatedAndIsNotValid_ThenTheRegistrationShouldBeRetrieved()
        {
            message.ApprenticeshipId = apprenticeshipId;

            var exceptionBuilder = new ValidationExceptionBuilder();
            exceptionBuilder.AddException(ValidationExceptionType.QualificationApprenticeshipIsNotComplete);
            Container.GetMock<IQualificationValidator>()
                .Setup(s => s.ValidateAgainstApprenticeshipQualification(It.IsAny<Qualification>(), registration , It.IsAny<Profile>()))
                .Returns(exceptionBuilder);

            ClassUnderTest.Invoking(c => c.CreateAsync(apprenticeId, message))
                .Should().Throw<AdmsValidationException>();
        }
        
        [TestMethod]
        public void WhenAnExceptionIsThrown_ThenItIsPassedOn()
        {
            Container.GetMock<ITYIMSRepository>()
                .Setup(s => s.GetRegistrationAsync(apprenticeshipId))
                .ThrowsAsync(new ArgumentOutOfRangeException());
            ClassUnderTest.Invoking(async c => await c.CreateAsync(apprenticeId, message))
                .Should().Throw<ArgumentOutOfRangeException>();
        }
    }
    #endregion
}