﻿using ADMS.Apprentice.Core;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests
{
    #region WhenUsingOurEnvironmentSettings

    [TestClass]
    public class WhenUsingOurEnvironmentSettings : GivenWhenThen
    {
        private OurEnvironmentSettings ourEnvironmentSettings;

        protected override void When()
        {
            ourEnvironmentSettings = new OurEnvironmentSettings();
        }

        [TestMethod]
        public void SetsDefaultSortableListRowLimit()
        {
            ourEnvironmentSettings.SortableListRowLimit.Should().Be(5000);
        }
    }

    #endregion
}