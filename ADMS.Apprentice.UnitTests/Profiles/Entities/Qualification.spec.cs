using Adms.Shared.Testing;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.UnitTests.Constants;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ADMS.Apprentice.UnitTests.Profiles.Models
{
    #region WhenInstantiatingAQualification

    [TestClass]
    public class WhenInstantiatingAQualification : GivenWhenThen
    {              
        private Qualification qualification;
        private Profile profile;
        private DateTime dt = DateTime.Now;
        private Guid g = new Guid();
        private byte[] b = new byte[] { 1 };

        protected override void Given()
        {
            qualification = ProfileConstants.Qualification;
            qualification.Id = 1;
            qualification.ApprenticeId = 1;
            qualification.CreatedOn = dt;
            qualification.CreatedBy = "1";
            qualification.UpdatedOn = dt;
            qualification.UpdatedBy = "1";
            qualification.Version = b;
            qualification.AuditEventId = 1;
            profile = new Profile { Id = 1 };
            qualification.Profile = profile;
        }
        /// <summary>
        /// Sets all the properties for the models.
        /// </summary>
        [TestMethod]
        public void SetsPropertiesOnModel()
        {
            qualification.Id.Should().Be(1);
            qualification.ApprenticeId.Should().Be(1);
            qualification.QualificationCode.Should().Be(ProfileConstants.Qualification.QualificationCode);
            qualification.QualificationDescription.Should().Be(ProfileConstants.Qualification.QualificationDescription);
            qualification.QualificationLevel.Should().Be(ProfileConstants.Qualification.QualificationLevel);
            qualification.QualificationANZSCOCode.Should().Be(ProfileConstants.Qualification.QualificationANZSCOCode);
            qualification.StartDate.Should().Be(ProfileConstants.Qualification.StartDate);
            qualification.EndDate.Should().Be(ProfileConstants.Qualification.EndDate);
            qualification.CreatedOn.Should().Be(dt);
            qualification.CreatedBy.Should().Be("1");
            qualification.UpdatedOn.Should().Be(dt);
            qualification.UpdatedBy.Should().Be("1");
            qualification.Version.Should().NotBeEmpty();
            qualification.AuditEventId.Should().Be(1);
            qualification.Profile.Should().Be(profile);
        }
    }

    #endregion
}