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

        protected override void Given()
        {
            profile = new Profile();
            qualification = new Qualification()
            {
                QualificationCode = "QCode",
                QualificationDescription = "QDescription",
                StartMonth = "JAN",
                StartYear = "2000",
                EndMonth = "DEC",
                EndYear = "2004",
            };
            profile.Qualifications.Add(qualification);
            validationException = new ValidationException(null, (ValidationError) null);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidQualification))
                .Returns(validationException);
        }

        
        protected override async void When()
        {
            profile.Qualifications = await ClassUnderTest.ValidateAsync(profile.Qualifications.ToList());
        }

        [TestMethod]
        public void UpdateStartAndEndDateIfQualificationIsValid()
        {
            profile.Qualifications.ToList().ForEach(x =>
            {
                x.StartDate.Should().Be(new DateTime(int.Parse(x.StartYear), DateTime.ParseExact(x.StartMonth, "MMM", CultureInfo.CurrentCulture).Month, 1));
                x.EndDate.Should().Be(new DateTime(int.Parse(x.EndYear), DateTime.ParseExact(x.EndMonth, "MMM", CultureInfo.CurrentCulture).Month, 1));
            });
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfStartOrEndDateIsNull()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartMonth = null;
            qualification.StartYear = null;
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(c => c.ValidateAsync(profile.Qualifications.ToList()))
                .Should().Throw<ValidationException>();
        }
        
         [TestMethod]
        public void ThrowsValidationExceptionIfStartOrEndMonthNotValid()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartMonth = "MMM";
            qualification.EndMonth = "OCT";
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(c => c.ValidateAsync(profile.Qualifications.ToList()))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfStartYearIsNotValid()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartYear = "1800";            
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(c => c.ValidateAsync(profile.Qualifications.ToList()))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfEndYearIsNotValid()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.EndYear = "1800";
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(c => c.ValidateAsync(profile.Qualifications.ToList()))
                .Should().Throw<ValidationException>();
        }
        [TestMethod]
        public void ThrowsExceptionIfEndYearIsLessThanStartYear()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartYear = "2005";
            qualification.EndYear = "2000";
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(c => c.ValidateAsync(profile.Qualifications.ToList()))
                .Should().Throw<ValidationException>();
        }
        [TestMethod]
        public void ThrowsExceptionIfEndDateIsLessThanStartDate()
        {
            profile.Qualifications = new List<Qualification>();
            qualification.StartYear = "2000";
            qualification.EndYear = "2000";
            qualification.StartMonth = "DEC";
            qualification.EndMonth = "JAN";
            profile.Qualifications.Add(qualification);
            ClassUnderTest.Invoking(c => c.ValidateAsync(profile.Qualifications.ToList()))
                .Should().Throw<ValidationException>();
        }
    }

    #endregion
}