using System;
using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.Core.Interface;
using ADMS.Apprentice.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IRepository = Adms.Shared.IRepository;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared.Exceptions;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace ADMS.Apprentice.UnitTests.Core
{
    #region OurDatabaseSettings
    [TestClass]
    public class WhenSettingOurDatabaseSettings
    {
        private OurDatabaseSettings ourDatabaseSettings;

        [TestMethod]
        public void NewOurDatabaseSettings()
        {
            ourDatabaseSettings = new OurDatabaseSettings
            {
                DatabaseConnectionString = "DatabaseConnectionString",
                TyimsDatabaseConnectionString = "TyimsDatabaseConnectionString",
                MigrateDatabaseOnStartup = true,
                SeedSampleData = true
            };

            ourDatabaseSettings.DatabaseConnectionString.Should().Be("DatabaseConnectionString");
            ourDatabaseSettings.TyimsDatabaseConnectionString.Should().Be("TyimsDatabaseConnectionString");
            ourDatabaseSettings.MigrateDatabaseOnStartup.Should().BeTrue();
            ourDatabaseSettings.SeedSampleData.Should().BeTrue();
        }


    }

    #endregion

    #region OurEnvironmentSettings
    [TestClass]
    public class WhenSettingOurEnvironmentSettings
    {
        private OurEnvironmentSettings ourEnvironmentSettings;

        [TestMethod]
        public void NewOurEnvironmentSettings()
        {
            ourEnvironmentSettings = new OurEnvironmentSettings
            {
                WebRootUrl = "WebRootUrl",
                ClientBuild = "ClientBuild",
                DisclaimerUrl = "DisclaimerUrl",
                CopyrightUrl = "CopyrightUrl",
                PrivacyUrl = "PrivacyUrl",
                SortableListRowLimit = 1
            };

            ourEnvironmentSettings.WebRootUrl.Should().Be("WebRootUrl");
            ourEnvironmentSettings.ClientBuild.Should().Be("ClientBuild");
            ourEnvironmentSettings.DisclaimerUrl.Should().Be("DisclaimerUrl");
            ourEnvironmentSettings.CopyrightUrl.Should().Be("CopyrightUrl");
            ourEnvironmentSettings.PrivacyUrl.Should().Be("PrivacyUrl");
            ourEnvironmentSettings.SortableListRowLimit.Should().Be(1);
        }
    }

    #endregion

    #region OurTestingSettings
    [TestClass]
    public class WhenSettingOurTestingSettings
    {
        private OurTestingSettings ourTestingSettings;

        [TestMethod]
        public void NewOurTestingSettings()
        {
            ourTestingSettings = new OurTestingSettings
            {
                EnableTestingTools = true,
                EnableEfProfiler = true
            };

            ourTestingSettings.EnableTestingTools.Should().BeTrue();
            ourTestingSettings.EnableEfProfiler.Should().BeTrue();
        }
    }

    #endregion


    #region OurSharedSettings
    [TestClass]
    public class WhenSettingOurSharedSettings
    {
        private SharedSettings sharedSettings;

        [TestMethod]
        public void NewSharedSettings()
        {
            OurTestingSettings ourTestingSettings = new();
            OurEnvironmentSettings ourEnvironmentSettings = new();
            OurDatabaseSettings ourDatabaseSettings = new();

            ourEnvironmentSettings.SortableListRowLimit = 4;
            ourEnvironmentSettings.WebRootUrl = "WebRootUrl";
            ourDatabaseSettings.DatabaseConnectionString = "DatabaseConnectionString";
            ourTestingSettings.EnableTestingTools = true;

            sharedSettings = new SharedSettings(
                Options.Create(ourTestingSettings),
                Options.Create(ourEnvironmentSettings),
                Options.Create(ourDatabaseSettings)
                );

            sharedSettings.SortableListRowLimit.Should().Be(4);
            sharedSettings.WebRootUrl.Should().Be("WebRootUrl");
            sharedSettings.DatabaseConnectionString.Should().Be("DatabaseConnectionString");
            sharedSettings.EnableTestingTools.Should().BeTrue();
        }


    }

    #endregion

}