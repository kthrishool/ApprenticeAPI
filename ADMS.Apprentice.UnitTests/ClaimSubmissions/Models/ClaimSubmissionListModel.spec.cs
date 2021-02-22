using System;
using ADMS.Apprentice.Core.ClaimSubmissions.Entities;
using ADMS.Apprentice.Core.ClaimSubmissions.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.ClaimSubmissions.Models
{
    #region WhenInstantiatingAClaimSubmissionListModel

    [TestClass]
    public class WhenInstantiatingAClaimSubmissionListModel : GivenWhenThen
    {
        private ClaimSubmissionListModel model;
        private ClaimSubmission claimSubmission;

        protected override void Given()
        {
            claimSubmission = new ClaimSubmission
            {
                ApprenticeId = 111,
                ApprenticeName = "Bob",
                Category = ClaimCategory.Com3,
                CreatedDate = DateTime.Now.AddMinutes(-3),
                EmployerId = 222,
                EmployerName = "Acme",
                Id = 123,
                LastModifiedDate = DateTime.Now,
                NetworkProviderId = 333,
                NetworkProviderName = "Jobs R Us",
                SubmissionStatus = ClaimSubmissionStatus.AwaitingApprenticeApproval,
                Type = ClaimType.Commencement
            };
        }

        protected override void When()
        {
            model = new ClaimSubmissionListModel(claimSubmission);
        }

        [TestMethod]
        public void ReturnsAModel()
        {
            model.Should().NotBeNull();
        }

        [TestMethod]
        public void SetsPropertiesOnModel()
        {
            model.ApprenticeId.Should().Be(111);
            model.ApprenticeName.Should().Be("Bob");
            model.Category.Should().Be(ClaimCategory.Com3);
            model.CreatedDate.Should().BeCloseTo(DateTime.Now.AddMinutes(-3));
            model.EmployerId.Should().Be(222);
            model.EmployerName.Should().Be("Acme");
            model.Id.Should().Be(123);
            model.LastModifiedDate.Should().BeCloseTo(DateTime.Now);
            model.NetworkProviderId.Should().Be(333);
            model.NetworkProviderName.Should().Be("Jobs R Us");
            model.SubmissionStatus.Should().Be(ClaimSubmissionStatus.AwaitingApprenticeApproval);
            model.Type.Should().Be(ClaimType.Commencement);
        }
    }

    #endregion

}
