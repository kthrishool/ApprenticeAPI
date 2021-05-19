using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Services.Validators;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    [TestClass]
    public class WhenValidatingGuardian : GivenWhenThen<GuardianValidator>
    {
        private Profile newProfile;
        private Guardian guardian;

        protected override void Given()
        {
            newProfile = new Profile();
            guardian = new Guardian();
        }

        protected override void When()
        {
            ClassUnderTest.ValidateAsync(guardian);
        }

        [TestMethod]
        public void DoNothingWhenNothingisPassed()
        {
            ClassUnderTest.Invoking(c => c.ValidateAsync(guardian)).Should().NotThrow();
        }
    }
}