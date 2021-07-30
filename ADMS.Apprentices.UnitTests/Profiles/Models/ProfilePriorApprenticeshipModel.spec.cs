using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.Profiles.Models
{
    #region WhenInstantiatingAProfileQualificationModel

    [TestClass]
    public class WhenInstantiatingAProfilePriorApprenticeshipModel : GivenWhenThen
    {
        private ProfilePriorApprenticeshipModel priorApprenticeshipModel;
        private PriorApprenticeship priorApprenticeship;

        protected override void Given()
        {
            priorApprenticeship = ProfileConstants.PriorApprenticeship;
        }

        protected override void When()
        {
            priorApprenticeshipModel = new ProfilePriorApprenticeshipModel(priorApprenticeship);
        }

        [TestMethod]
        public void ReturnsAModel()
        {
            priorApprenticeshipModel.Should().NotBeNull();
        }

        /// <summary>
        /// Sets all the properties for the models.
        /// </summary>
        [TestMethod]
        public void SetsPropertiesOnModel()
        {
            priorApprenticeshipModel.QualificationCode.Should().Be(ProfileConstants.Qualification.QualificationCode);
            priorApprenticeshipModel.QualificationDescription.Should().Be(ProfileConstants.Qualification.QualificationDescription);
            priorApprenticeshipModel.QualificationLevel.Should().Be(ProfileConstants.Qualification.QualificationLevel);
            priorApprenticeshipModel.QualificationANZSCOCode.Should().Be(ProfileConstants.Qualification.QualificationANZSCOCode);
            priorApprenticeshipModel.StartDate.Should().Be(ProfileConstants.Qualification.StartDate);
            priorApprenticeshipModel.EndDate.Should().Be(ProfileConstants.Qualification.EndDate);
        }

        [TestMethod]
        public void StartAndEndDateIsNull()
        {
            priorApprenticeship.StartDate = null;
            priorApprenticeship.EndDate = null;
            priorApprenticeshipModel = new ProfilePriorApprenticeshipModel(priorApprenticeship);
            priorApprenticeshipModel.StartDate.Should().BeNull();
            priorApprenticeshipModel.EndDate.Should().BeNull();
        }
    }

    #endregion
}