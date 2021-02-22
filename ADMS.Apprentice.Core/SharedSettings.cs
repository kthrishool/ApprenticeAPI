using Adms.Shared;
using Microsoft.Extensions.Options;

namespace ADMS.Apprentice.Core
{
    public class SharedSettings : ISharedSettings
    {
        private readonly IOptions<OurTestingSettings> ourTestingSettings;
        private readonly IOptions<OurEnvironmentSettings> ourEnvironmentSettings;

        public int SortableListRowLimit => ourEnvironmentSettings.Value.SortableListRowLimit;
        public bool EnableTestingTools => ourTestingSettings.Value.EnableTestingTools;
        public string WebRootUrl => ourEnvironmentSettings.Value.WebRootUrl;

        public SharedSettings(IOptions<OurTestingSettings> ourTestingSettings, IOptions<OurEnvironmentSettings> ourEnvironmentSettings)
        {
            this.ourTestingSettings = ourTestingSettings;
            this.ourEnvironmentSettings = ourEnvironmentSettings;
        }
    }
}