using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.Profiles.Models
{
    #region WhenInstantiatingAPriorQualificationModel

    [TestClass]
    public class WhenInstantiatingAPriorQualificationModel : GivenWhenThen
    {
        private PriorQualificationModel qualificationModel;
        private PriorQualification qualification;

        protected override void Given()
        {
            qualification = ProfileConstants.Qualification;
        }

        protected override void When()
        {
            qualificationModel = new PriorQualificationModel(qualification);
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
            qualificationModel.QualificationLevel.Should().Be(ProfileConstants.Qualification.QualificationLevel);
            qualificationModel.QualificationANZSCOCode.Should().Be(ProfileConstants.Qualification.QualificationANZSCOCode);
            qualificationModel.QualificationManualReasonCode.Should().Be(ProfileConstants.Qualification.QualificationManualReasonCode);
            qualificationModel.StartDate.Should().Be(ProfileConstants.Qualification.StartDate);
            qualificationModel.EndDate.Should().Be(ProfileConstants.Qualification.EndDate);
        }

        [TestMethod]
        public void StartAndEndDateIsNull()
        {
            qualification.StartDate = null;
            qualification.EndDate = null;
            qualificationModel = new PriorQualificationModel(qualification);
            qualificationModel.StartDate.Should().BeNull();
            qualificationModel.EndDate.Should().BeNull();
        }
    }

    #endregion
}