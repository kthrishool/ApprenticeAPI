// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ADMS.Apprentices.Core
{
    public class OurEnvironmentSettings
    {
        public string WebRootUrl { get; set; }

        public int SortableListRowLimit { get; set; } = 5000;

        public string SwaggerPath { get; set; }
        public string SwaggerPrefix { get; set; }
    }
}
