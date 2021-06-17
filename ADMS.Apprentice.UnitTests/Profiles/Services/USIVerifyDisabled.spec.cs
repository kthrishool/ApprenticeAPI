using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
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