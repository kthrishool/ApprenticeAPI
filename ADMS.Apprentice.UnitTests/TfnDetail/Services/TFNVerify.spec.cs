using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests;
using System.Collections.Generic;
using System.Xml.Linq;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable UnusedMember.Local

namespace Adms.Shared.UnitTests.Services
{
    #region WhenVerifyingATfn

    [TestClass]
    public class WhenMatchesChecksumForTfn : GivenWhenThen<TFNVerify>
    {
        [TestMethod]
        public void WhenCheckingValidTfn()
        {
            ClassUnderTest.MatchesChecksum("343656027").Should().BeTrue();
        }

        [TestMethod]
        public void WhenCheckingInvalidTfn()
        {
            ClassUnderTest.MatchesChecksum("123123123123").Should().BeFalse();
            ClassUnderTest.MatchesChecksum("123123123").Should().BeFalse();
            ClassUnderTest.MatchesChecksum("999999999").Should().BeFalse();
            ClassUnderTest.MatchesChecksum("012345678").Should().BeFalse();
            ClassUnderTest.MatchesChecksum("999").Should().BeFalse();
            ClassUnderTest.MatchesChecksum("").Should().BeFalse();
        }

        [TestMethod]
        public void WhenCheckingNullTfn()
        {
            ClassUnderTest.MatchesChecksum(null).Should().BeFalse();
        }
    }

    #endregion

}