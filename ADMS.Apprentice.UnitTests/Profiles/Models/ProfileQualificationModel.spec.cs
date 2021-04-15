using Adms.Shared.Testing;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.UnitTests.Constants;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Models
{
    #region WhenInstantiatingAProfileQualificationModel

    [TestClass]
    public class WhenInstantiatingAProfileQualificationModel : GivenWhenThen
    {        
        private ProfileQualificationModel qualificationModel;
        private Qualification qualification;

        protected override void Given()
        {
            qualification = ProfileConstants.Qualification;
        }

        protected override void When()
        {
            qualificationModel = new ProfileQualificationModel(qualification);
        }

        [TestMethod]
        public void ReturnsAModel()
        {
            qualificationModel.Should().NotBeNull();
        }

        /// <summary>
        /// Sets all the properties for the models.
        /// </summary>
        [TestMethod]
        public void SetsPropertiesOnModel()
        {
            qualificationModel.QualificationCode.Should().Be(ProfileConstants.Qualification.QualificationCode);
            qualificationModel.QualificationDescription.Should().Be(ProfileConstants.Qualification.QualificationDescription);
            qualificationModel.StartMonth.Should().Be(ProfileConstants.Qualification.StartDate.ToString("MMM").ToUpper());
            qualificationModel.StartYear.Should().Be(ProfileConstants.Qualification.StartDate.Year.ToString());
            qualificationModel.EndMonth.Should().Be(ProfileConstants.Qualification.EndDate.ToString("MMM").ToUpper());
            qualificationModel.EndYear.Should().Be(ProfileConstants.Qualification.EndDate.Year.ToString());
        }
    }

    #endregion
}