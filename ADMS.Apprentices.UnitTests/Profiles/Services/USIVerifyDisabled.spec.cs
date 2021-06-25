using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.UnitTests.Constants;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{

    [TestClass]
    public class WhenUSIVerifyDisabled : GivenWhenThen<USIVerifyDisabled>
    {
        private Profile profile;
        private ApprenticeUSI apprenticeUSI;

        protected override void Given()
        {
            profile = new Profile()
            {
                FirstName = ProfileConstants.Firstname,
                Surname = ProfileConstants.Surname,
                BirthDate = ProfileConstants.Birthdate,
            };
        }

        protected override void When()
        {
            apprenticeUSI = ClassUnderTest.Verify(profile);
        }


        [TestMethod]
        public void ReturnsNull()
        {
            apprenticeUSI.Should().BeNull();
        }
    }
}