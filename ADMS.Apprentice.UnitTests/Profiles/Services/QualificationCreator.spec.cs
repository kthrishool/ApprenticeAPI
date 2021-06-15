using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Apprentice.UnitTests.Constants;
using Adms.Shared;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenCreatingAQualification

    [TestClass]
    public class WhenCreatingAQualification : GivenWhenThen<QualificationCreator>
    {
        private Qualification qualification;
        private ProfileQualificationMessage message;
        private int apprenticeId;

        protected override void Given()
        {
            apprenticeId = 1;
            message = ProfileConstants.QualificationMessage;            
        }

        protected override async void When()
        {
            qualification = await ClassUnderTest.CreateAsync(apprenticeId, message);
        }

        [TestMethod]
        public void ShouldReturnQualification()
        {
            qualification.Should().NotBeNull();
        }


        [TestMethod]
        public void ShouldValidatesTheQualificationRequest()
        {
            Container.GetMock<IQualificationValidator>().Verify(r => r.ValidateAsync(qualification));
        }

        [TestMethod]
        public void ShouldSetTheDetails()
        {
            qualification.QualificationCode.Should().Be(message.QualificationCode);
            qualification.QualificationDescription.Should().Be(message.QualificationDescription);
            qualification.QualificationANZSCOCode.Should().Be(message.QualificationANZSCOCode);
            qualification.QualificationLevel.Should().Be(message.QualificationLevel);
        }
    }
    #endregion
}