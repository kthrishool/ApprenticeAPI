using System;
using ADMS.Apprentice.Core.ClaimSubmissions.Entities;
using ADMS.Apprentice.Core.ClaimSubmissions.Messages;
using ADMS.Apprentice.Core.ClaimSubmissions.Services;
using Adms.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.ClaimSubmissions.Services
{
    #region WhenCreatingAClaimSubmission

    [TestClass]
    public class WhenCreatingAClaimSubmission : GivenWhenThen<ClaimSubmissionCreator>
    {
        private const int registrationId = 123;
        private ClaimSubmission claimSubmission;
        private ClaimSubmissionMessage message;

        protected override void Given()
        {
            message = new ClaimSubmissionMessage
            {
                Type = ClaimType.Commencement,
                Category = ClaimCategory.Com3,
                RegistrationId = registrationId
            };
        }

        protected override async void When()
        {
            claimSubmission = await ClassUnderTest.CreateAsync(message);
        }

        [TestMethod]
        public void ShouldReturnClaimSubmission()
        {
            claimSubmission.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldAddTheClaimSubmissionToTheDatabase()
        {
            Container.GetMock<IRepository>().Verify(r => r.Insert(claimSubmission));
        }

        [TestMethod]
        public void ShouldSetTheClaimType()
        {
            claimSubmission.Type.Should().Be(ClaimType.Commencement);
        }

        [TestMethod]
        public void ShouldSetTheClaimCategory()
        {
            claimSubmission.Category.Should().Be(ClaimCategory.Com3);
        }

        [TestMethod]
        public void ShouldSetSomeMockValuesOnOtherFields()
        {
            claimSubmission.ApprenticeName.Should().NotBeNull();
            claimSubmission.ApprenticeId.Should().BeGreaterThan(0);
            // ..etc
        }

        [TestMethod]
        public void ShouldSetStatus()
        {
            claimSubmission.SubmissionStatus.Should().Be(ClaimSubmissionStatus.AwaitingEmployerApproval);
        }

        [TestMethod]
        public void ShouldSetCreatedAndLastModifiedDates()
        {
            claimSubmission.CreatedDate.Should().BeCloseTo(DateTime.Now);
            claimSubmission.LastModifiedDate.Should().BeCloseTo(DateTime.Now);
        }
    }

    #endregion
}