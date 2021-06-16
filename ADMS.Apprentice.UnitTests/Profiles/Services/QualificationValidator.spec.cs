using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using ADMS.Apprentice.Core.Services.Validators;
using Moq;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenValidatingAQualification

    [TestClass]
    public class WhenValidatingAQualification : GivenWhenThen<QualificationValidator>
    {
        private Qualification qualification;
        //private Address invalidAddress;
        private Profile profile;
        private ValidationException validationException;
        
        private IValidatorExceptionBuilder exceptionBuilder;

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
            Container.GetMock<IValidatorExceptionBuilderFactory>()
                .Setup(ebf => ebf.CreateExceptionBuilder())
                .Returns(() => new ValidatorExceptionBuilder(exceptionFactory));
            
            Container.GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidateAsync(It.IsAny<Qualification>()))
                .ReturnsAsync(() => new ValidatorExceptionBuilder(exceptionFactory));

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidQualification))
                .Returns(validationException);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.DuplicateQualification))
                .Returns(validationException);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(It.IsAny<ValidationExceptionType[]>()))
                .Returns(validationException);
        }


        protected override async void When()
        {
             exceptionBuilder = await ClassUnderTest.ValidateAsync(profile.Qualifications.ToList());
        }

        [TestMethod]
        public void UpdateStartAndEndDateIfQualificationIsValid()
        {
            profile.Qualifications.ToList().ForEach(x =>
            {
                x.StartDate.Should().Be(new DateTime(2010, 1, 1));
                x.EndDate.Should().Be(new DateTime(2020, 1, 1));
            });
        }

        [TestMethod]
        public void NoExceptionIfStartDateIsNull()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartDate = null;
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void NoExceptionIfEndDateIsNull()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.EndDate = null;
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfFoundDuplicate()
        {           
            profile.Qualifications = new List<Qualification>();            
            profile.Qualifications.Add(qualification);            
            profile.Qualifications.Add(qualification);

            ClassUnderTest.Invoking(c => c.CheckForDuplicates(profile.Qualifications.ToList()).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void DoesNotThrowValidationExceptionIfNoDuplicate()
        {
            profile.Qualifications = new List<Qualification>();
            profile.Qualifications.Add(qualification);

            ClassUnderTest.Invoking(c => c.CheckForDuplicates(profile.Qualifications.ToList()).ThrowAnyExceptions())
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfQCodeIsNull()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.QualificationCode = null;
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfEndDateIsLessThanStartDate()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartDate = new DateTime(2020, 1, 2);
            qualification.EndDate = new DateTime(2020, 1, 1);
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

       
        [TestMethod]
        public void ThrowsExceptionIfStartDateIsGreaterThanTodaysDate()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartDate = DateTime.Now.AddDays(+1);
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }       

        [TestMethod]
        public void ThrowsExceptionIfEndDateIsGraterThanTodaysDate()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.EndDate = DateTime.Now.AddDays(+1);
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfStartDateEndDateIsGraterThanTodaysDate()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartDate = DateTime.Now.AddDays(+1);
            qualification.EndDate = DateTime.Now.AddDays(+1);
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }        

        [TestMethod]
        public void ThrowsExceptionIfStartDateIsLessThanDateofBirthPlus12Years()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartDate = ProfileConstants.Birthdate.AddYears(+11);
            qualification.Profile = new Profile()
            {
                BirthDate = ProfileConstants.Birthdate
            };
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfEndDateIsLessThanDateofBirthPlus12Years()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartDate = null;
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(+12);
            qualification.Profile = new Profile()
            {
                BirthDate = ProfileConstants.Birthdate
            };
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfStartDateEndDateIsLessThanDateofBirthPlus12Years()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartDate = ProfileConstants.Birthdate.AddYears(+10);
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(+11);
            qualification.Profile = new Profile()
            {
                BirthDate = ProfileConstants.Birthdate
            };
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void NotThrowExceptionIfStartDateEndDateIsGreaterThanDateofBirthPlus12Years()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartDate = ProfileConstants.Birthdate.AddYears(+13);
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(+14);
            qualification.Profile = new Profile()
            {
                BirthDate = ProfileConstants.Birthdate
            };
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        [TestMethod]
        public void NotThrowExceptionIfEndDateIsGreaterThanDateofBirthPlus12Years()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(+14);
            qualification.Profile = new Profile()
            {
                BirthDate = ProfileConstants.Birthdate
            };
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().NotThrow();
        }


        [TestMethod]
        public void NotThrowExceptionIfEndDateIsNotNullAndProfileisNUll()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.EndDate = null;
            qualification.Profile = null;
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile.Qualifications.ToList())).ThrowAnyExceptions())
                .Should().NotThrow();
        }       
    }
    #endregion
}