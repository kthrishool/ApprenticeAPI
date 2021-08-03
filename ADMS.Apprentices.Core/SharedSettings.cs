using Adms.Shared;
using Microsoft.Extensions.Options;

namespace ADMS.Apprentices.Core
{
    public class SharedSettings : ISharedSettings
    {
        private readonly IOptions<OurTestingSettings> ourTestingSettings;
        private readonly IOptions<OurEnvironmentSettings> ourEnvironmentSettings;
        private readonly IOptions<OurDatabaseSettings> ourDatabaseSettings;        
        private readonly IOptions<AuthorisationSettings> authorisationSettings;
        private readonly IOptions<ClaimTypeSettings> claimTypeSettings;

        public int SortableListRowLimit => ourEnvironmentSettings.Value.SortableListRowLimit;
        public bool EnableTestingTools => ourTestingSettings.Value.EnableTestingTools;
        public string WebRootUrl => ourEnvironmentSettings.Value.WebRootUrl;
        public string DatabaseConnectionString => ourDatabaseSettings.Value.DatabaseConnectionString;
        public AuthorisationSettings Authorisation => authorisationSettings.Value;
        public ClaimTypeSettings ClaimTypes => claimTypeSettings.Value;
        public SharedSettings(
            IOptions<OurTestingSettings> ourTestingSettings,
            IOptions<OurEnvironmentSettings> ourEnvironmentSettings,
            IOptions<OurDatabaseSettings> ourDatabaseSettings,
            IOptions<AuthorisationSettings> authorisationSettings,
            IOptions<ClaimTypeSettings> claimTypeSettings)
        {
            this.ourTestingSettings = ourTestingSettings;
            this.ourEnvironmentSettings = ourEnvironmentSettings;
            this.ourDatabaseSettings = ourDatabaseSettings;
            this.authorisationSettings = authorisationSettings;
            this.claimTypeSettings = claimTypeSettings;
        }
    }
}