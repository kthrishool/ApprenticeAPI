using System.Collections.Generic;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using ADMS.Apprentice.Core.HttpClients.USI;
using Moq;
using System;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    
    [TestClass]
    public class WhenVerifyingUSI : GivenWhenThen<USIVerify>
    {
        private Profile profile;
        private ApprenticeUSI apprenticeUSI;
        private List<VerifyUsiModel> verifymodel = new List<VerifyUsiModel>();        

        protected override void Given()
        {
            profile = new Profile()
            {
                FirstName = ProfileConstants.Firstname,
                Surname = ProfileConstants.Surname,
                BirthDate = ProfileConstants.Birthdate,
            };
            apprenticeUSI = new ApprenticeUSI()
            {
                USI = "test",
                ActiveFlag = true
            };
            profile.USIs.Add(apprenticeUSI);

            verifymodel = new List<VerifyUsiModel>()
            {
                new VerifyUsiModel() { DateOfBirthMatched = true, FamilyNameMatched = true, FirstNameMatched = true, USIStatus = "Valid" }
            };
            Container
                .GetMock<IUSIClient>()
                .Setup(r => r.VerifyUsi(It.IsAny<List<VerifyUsiMessage>>()))
                .ReturnsAsync(verifymodel);           
        }

        protected override void When()
        {
            apprenticeUSI = ClassUnderTest.Verify(profile);
        }


        [TestMethod]
        public void ReturnsResultAfterVerify()
        {
            apprenticeUSI.Should().NotBeNull();
        }

        [TestMethod]
        public void ReturnsNullIfNoUSIForApprentice()
        {
            profile.USIs.Clear();
            apprenticeUSI = ClassUnderTest.Verify(profile);
            apprenticeUSI.Should().BeNull();
        }
       

        [TestMethod]
        public void ReturnsNullIfApprenticeUSIisNull()
        {
            profile.USIs = null;
            apprenticeUSI = ClassUnderTest.Verify(profile);
            apprenticeUSI.Should().BeNull();
        }

        [TestMethod]
        public void ReturnsResultIfExceptionHappensInTheUSIVerifyService()
        {
            Container
                .GetMock<IUSIClient>()
                .Setup(r => r.VerifyUsi(It.IsAny<List<VerifyUsiMessage>>()))
                .Throws(new Exception());
            apprenticeUSI = ClassUnderTest.Verify(profile);
            apprenticeUSI.Should().NotBeNull();
        }

        [TestMethod]
        public void USIVerifyFlagShouldBeFalseIfNameDoesntMatch()
        {
            verifymodel = new List<VerifyUsiModel>()
            {
                new VerifyUsiModel() { DateOfBirthMatched = true, FamilyNameMatched = true, FirstNameMatched = false, USIStatus = "Valid" }
            };
            Container
                .GetMock<IUSIClient>()
                .Setup(r => r.VerifyUsi(It.IsAny<List<VerifyUsiMessage>>()))
                .ReturnsAsync(verifymodel);
            apprenticeUSI = ClassUnderTest.Verify(profile);
            apprenticeUSI.USIVerifyFlag.Should().Be(false);
        }

        [TestMethod]
        public void USIVerifyFlagShouldBeFalseIfDOBDoesntMatch()
        {
            verifymodel = new List<VerifyUsiModel>()
            {
                new VerifyUsiModel() { DateOfBirthMatched = false, FamilyNameMatched = true, FirstNameMatched = true, USIStatus = "Valid" }
            };
            Container
                .GetMock<IUSIClient>()
                .Setup(r => r.VerifyUsi(It.IsAny<List<VerifyUsiMessage>>()))
                .ReturnsAsync(verifymodel);
            apprenticeUSI = ClassUnderTest.Verify(profile);
            apprenticeUSI.USIVerifyFlag.Should().Be(false);
        }

        [TestMethod]
        public void USIVerifyFlagShouldBeFalseIfNoResult()
        {
            verifymodel = new List<VerifyUsiModel>()
            {
                new VerifyUsiModel() { DateOfBirthMatched = null, FamilyNameMatched = null, FirstNameMatched = null, USIStatus = null }
            };
            Container
                .GetMock<IUSIClient>()
                .Setup(r => r.VerifyUsi(It.IsAny<List<VerifyUsiMessage>>()))
                .ReturnsAsync(verifymodel);
            apprenticeUSI = ClassUnderTest.Verify(profile);
            apprenticeUSI.USIVerifyFlag.Should().Be(false);
        }

        [TestMethod]
        public void USIVerifyFlagShouldBeFalseIfUsiStatusIsInvalid()
        {
            verifymodel = new List<VerifyUsiModel>()
            {
                new VerifyUsiModel() { DateOfBirthMatched = true, FamilyNameMatched = true, FirstNameMatched = true, USIStatus = "Invalid" }
            };
            Container
                .GetMock<IUSIClient>()
                .Setup(r => r.VerifyUsi(It.IsAny<List<VerifyUsiMessage>>()))
                .ReturnsAsync(verifymodel);
            apprenticeUSI = ClassUnderTest.Verify(profile);
            apprenticeUSI.USIVerifyFlag.Should().Be(false);
        }

    }
}