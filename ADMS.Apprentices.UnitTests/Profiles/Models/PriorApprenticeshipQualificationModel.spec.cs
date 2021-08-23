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
    public class WhenInstantiatingAPriorPriorApprenticeshipModel : GivenWhenThen
    {
        private PriorApprenticeshipQualificationModel model;
        private PriorApprenticeshipQualification priorApprenticeship;

        protected override void Given()
        {
            priorApprenticeship = ProfileConstants.PriorApprenticeshipQualification;
        }

        protected override void When()
        {
            model = new PriorApprenticeshipQualificationModel(priorApprenticeship);
        }

        [TestMethod]
        public void ReturnsAModel()
        {
            model.Should().NotBeNull();
        }

        /// <summary>
        /// Sets all the properties for the models.
        /// </summary>
        [TestMethod]
        public void SetsPropertiesOnModel()
        {
            model.EmployerName.Should().Be(ProfileConstants.PriorApprenticeshipQualification.EmployerName);
            model.QualificationCode.Should().Be(ProfileConstants.PriorApprenticeshipQualification.QualificationCode);
            model.QualificationDescription.Should().Be(ProfileConstants.PriorApprenticeshipQualification.QualificationDescription);
            model.QualificationLevel.Should().Be(ProfileConstants.PriorApprenticeshipQualification.QualificationLevel);
            model.QualificationANZSCOCode.Should().Be(ProfileConstants.PriorApprenticeshipQualification.QualificationANZSCOCode);
            model.NotOnTrainingGovAu.Should().Be(ProfileConstants.PriorApprenticeshipQualification.NotOnTrainingGovAu);
            model.StartDate.Should().Be(ProfileConstants.PriorApprenticeshipQualification.StartDate);
            model.ApprenticeshipReference.Should().Be(ProfileConstants.PriorApprenticeshipQualification.ApprenticeshipReference);
        }
    }

    #endregion
}