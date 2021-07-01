using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using ADMS.Apprentices.Core.Services.Validators;
using Moq;
using ADMS.Apprentices.Core.TYIMS.Entities;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenValidatingAQualification

    [TestClass]
    public class WhenValidatingAQualification : GivenWhenThen<QualificationValidator>
    {
        private Qualification qualification;       
        private Profile profile;
        private ValidationException validationException;
        //private ValidationException duplicateQualification;
        
        private ValidationExceptionBuilder exceptionBuilder;

        protected override void Given()
        {
            profile = new Profile();
            qualification = new Qualification()
            {
                QualificationCode = "QCode",
                QualificationDescription = "QDescription",
                QualificationLevel = "524",
                QualificationANZSCOCode = "ANZS",
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2020, 1, 1)
            };
            profile.Qualifications.Add(qualification);
            profile.BirthDate = ProfileConstants.Birthdate;
            validationException = new ValidationException(null, (ValidationError)null);

            var exceptionFactory = Container.GetMock<IExceptionFactory>().Object;

            Container.GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidateAsync(It.IsAny<Qualification>()))
                .ReturnsAsync(() => new ValidationExceptionBuilder(exceptionFactory));

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(It.IsAny<ValidationExceptionType[]>()))
                .Returns(validationException);
        }


        protected override async void When()
        {
             exceptionBuilder = await ClassUnderTest.ValidateAsync(qualification, profile);
        }
       
        [TestMethod]
        public void NoExceptionIfStartDateIsNull()
        {
            profile.Qualifications.Clear();
            qualification.StartDate = null;
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).HasExceptions().Should().BeFalse()); 
        }

        [TestMethod]
        public void NoExceptionIfEndDateIsNull()
        {
            profile.Qualifications.Clear();
            qualification.EndDate = null;
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).HasExceptions().Should().BeFalse());               
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfFoundDuplicate()
        {
            profile.Qualifications.Clear();
            profile.Qualifications.Add(qualification);
            profile.Qualifications.Add(qualification);

            ClassUnderTest.Invoking(c => c.CheckForDuplicates(profile.Qualifications.ToList()).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void DoesNotThrowValidationExceptionIfNoDuplicate()
        {
            profile.Qualifications.Clear();
            profile.Qualifications.Add(qualification);

            ClassUnderTest.Invoking(c => c.CheckForDuplicates(profile.Qualifications.ToList()).ThrowAnyExceptions())
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfQCodeIsNull()
        {
            profile.Qualifications.Clear();
            qualification.QualificationCode = null;
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
               .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void ThrowsExceptionIfEndDateIsLessThanStartDate()
        {
            profile.Qualifications.Clear();
            qualification.StartDate = new DateTime(2020, 1, 2);
            qualification.EndDate = new DateTime(2020, 1, 1);
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
              .Should().Throw<ValidationException>().Where(e => e == validationException);
        }


        [TestMethod]
        public void ThrowsExceptionIfStartDateIsGreaterThanTodaysDate()
        {
            profile.Qualifications.Clear();
            qualification.StartDate = DateTime.Now.AddDays(+1);
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
              .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void ThrowsExceptionIfEndDateIsGraterThanTodaysDate()
        {
            profile.Qualifications.Clear();
            qualification.EndDate = DateTime.Now.AddDays(+1);
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
              .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void ThrowsExceptionIfStartDateEndDateIsGraterThanTodaysDate()
        {
            profile.Qualifications.Clear();
            qualification.StartDate = DateTime.Now.AddDays(+1);
            qualification.EndDate = DateTime.Now.AddDays(+1);
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
              .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void ThrowsExceptionIfStartDateIsLessThanDateofBirthPlus12Years()
        {
            profile.Qualifications.Clear();
            qualification.StartDate = ProfileConstants.Birthdate.AddYears(11);            
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
              .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void ThrowsExceptionIfEndDateIsLessThanDateofBirthPlus12Years()
        {
            profile.Qualifications.Clear();
            qualification.StartDate = null;
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(11);            
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
              .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void ThrowsExceptionIfStartDateEndDateIsLessThanDateofBirthPlus12Years()
        {
            profile.Qualifications.Clear();
            qualification.StartDate = ProfileConstants.Birthdate.AddYears(10);
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(11);
           
            profile.Qualifications.Add(qualification);
            var b = ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)));
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
               .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void NotThrowExceptionIfStartDateEndDateIsGreaterThanDateofBirthPlus12Years()
        {
            profile.Qualifications.Clear();
            qualification.StartDate = ProfileConstants.Birthdate.AddYears(13);
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(14);            
            profile.Qualifications.Add(qualification);

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
                .Should().NotThrow();            
        }

        [TestMethod]
        public void NotThrowExceptionIfEndDateIsGreaterThanDateofBirthPlus12Years()
        {
            profile.Qualifications.Clear();
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(14);            
            profile.Qualifications.Add(qualification);

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
                            .Should().NotThrow();
        }

        [TestMethod]
        public void WhenQualificationBelongsToAnApprenticeshipAndHasNoEndDate_ThenAnExceptionOccurs()
        {
            profile.Qualifications.Clear();
            qualification.ApprenticeshipId = 10;
            qualification.EndDate = null;
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
              .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void WhenQualificationBelongsToAnApprenticeshipAndHasNoStartDate_ThenAnExceptionOccurs()
        {
            profile.Qualifications.Clear();
            qualification.ApprenticeshipId = 10;
            qualification.StartDate = null;
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification, profile)).ThrowAnyExceptions())
               .Should().Throw<ValidationException>().Where(e => e == validationException);
        }
    }
    #endregion
    
    #region "Qualification Validation with Apprenticeship"
    [TestClass]
    public class WhenValidatingAQualificationWithAnApprenticeship : GivenWhenThen<QualificationValidator>
    {
        private ValidationException validationException;
        private Qualification qualification;       
        private Profile profile;
        private Registration registration;
        protected override void Given()
        {
            profile = new Profile();
            qualification = new Qualification()
            {
                QualificationCode = "QCode",
                QualificationDescription = "QDescription",
                QualificationLevel = "524",
                QualificationANZSCOCode = "ANZS",
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2020, 1, 1),
                ApprenticeshipId = 20,
                Id = ProfileConstants.Id
            };
            profile.Qualifications.Add(qualification);
            profile.BirthDate = ProfileConstants.Birthdate;
            validationException = new ValidationException(null, (ValidationError)null);

            registration = new Registration()
            {
                CurrentEndReasonCode = "CMPS",
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2020, 1, 1),
                RegistrationId = qualification.ApprenticeshipId.Value,
                QualificationCode = "QCode",
                TrainingContractId = 100,
                ClientId = ProfileConstants.Id
            };

            var exceptionFactory = Container.GetMock<IExceptionFactory>().Object;

            base.Given();
            Container.GetMock<IExceptionFactory>()
                .Setup(ef => ef.CreateValidationException(It.IsAny<ValidationExceptionType[]>()))
                .Returns(validationException)
                ;
            
            Container.GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidateAsync(It.IsAny<Qualification>()))
                .ReturnsAsync(() => new ValidationExceptionBuilder(exceptionFactory));

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(It.IsAny<ValidationExceptionType[]>()))
                .Returns(validationException);
        }
        
        private ValidationExceptionBuilder callApprenticeshipQualification(Qualification qualification, Registration registration, Profile profile)
        {
            return ClassUnderTest.ValidateAgainstApprenticeshipQualification(qualification, registration, profile);
        }
        [TestMethod]
        public void WhenRegistrationIsValid_ThenNoErrorsOccur()
        {
            callApprenticeshipQualification(qualification, registration, profile).HasExceptions()
                .Should().Equals(false);

        } 

        [TestMethod]
        public void WhenRegistrationIsNull_ThenErrorsOccur()
        {
            callApprenticeshipQualification(qualification, null, profile).HasExceptions()
                .Should().Equals(true);
        } 

        [TestMethod]
        public void WhenRegistrationEndDateIsNull_ThenErrorsOccur()
        {
            registration.EndDate = null;
            callApprenticeshipQualification(qualification, registration,profile).HasExceptions()
                .Should().Equals(true);
        } 

        [TestMethod]
        public void WhenRegistrationEndReasonCodeIsNotCMPS_ThenErrorsOccur()
        {
            registration.CurrentEndReasonCode = "SUSP";
            callApprenticeshipQualification(qualification, registration,profile).HasExceptions()
                .Should().Equals(true);
        } 

        [TestMethod]
        public void WhenRegistrationQualificationCodeDoesNotEqualQualification_ThenErrorsOccur()
        {
            registration.QualificationCode = "OTH";
            callApprenticeshipQualification(qualification, registration, profile).HasExceptions()
                .Should().Equals(true);
        }
        [TestMethod]
        public void WhenRegistrationClientIdDoesNotEqualProfileId_ThenErrorsOccur()
        {
            registration.ClientId = ProfileConstants.Id + 10;
            callApprenticeshipQualification(qualification, registration, profile).HasExceptions()
                .Should().Equals(true);
        }
        
    }
    #endregion "Qualification Validation with Apprenticeship"
}