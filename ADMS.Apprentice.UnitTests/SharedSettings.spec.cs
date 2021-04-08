using ADMS.Apprentice.Core;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests
{
    #region WhenUsingSharedSettings

    [TestClass]
    public class WhenUsingSharedSettings : GivenWhenThen<SharedSettings>
    {
        protected override void Given()
        {
            Container
                .GetMock<IOptions<OurEnvironmentSettings>>()
                .Setup(r => r.Value)
                .Returns(new OurEnvironmentSettings {SortableListRowLimit = 69, WebRootUrl = "root"});
            Container
                .GetMock<IOptions<OurTestingSettings>>()
                .Setup(r => r.Value)
                .Returns(new OurTestingSettings {EnableTestingTools = true});
            Container
                .GetMock<IOptions<OurDatabaseSettings>>()
                .Setup(r => r.Value)
                .Returns(new OurDatabaseSettings {DatabaseConnectionString = "db-connection"});
        }

        [TestMethod]
        public void SortableListRowLimitComesFromOurEnvironmentSettings()
        {
            ClassUnderTest.SortableListRowLimit.Should().Be(69);
        }

        [TestMethod]
        public void WebRootUrlComesFromOurEnvironmentSettings()
        {
            ClassUnderTest.WebRootUrl.Should().Be("root");
        }

        [TestMethod]
        public void EnableTestingToolsComesFromOurTestingSettings()
        {
            ClassUnderTest.EnableTestingTools.Should().BeTrue();
        }

        [TestMethod]
        public void DatabaseConnectionStringComesFromOurDatabaseSettings()
        {
            ClassUnderTest.DatabaseConnectionString.Should().Be("db-connection");
        }
    }

    #endregion
}