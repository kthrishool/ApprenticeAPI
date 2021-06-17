using ADMS.Apprentice.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADMS.Apprentice.Api.Configuration
{
    /// <summary>
    /// Configuration for settings
    /// </summary>
    public static class SettingsConfiguration
    {
        /// <summary>
        /// Maps our configuration classes to the configuration held in app.config
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OurEnvironmentSettings>(configuration.GetSection(nameof(OurEnvironmentSettings)));
            services.Configure<OurDatabaseSettings>(configuration.GetSection(nameof(OurDatabaseSettings)));
            services.Configure<OurTestingSettings>(configuration.GetSection(nameof(OurTestingSettings)));
            services.Configure<OurUsiSettings>(configuration.GetSection(nameof(OurUsiSettings)));
            services.Configure<OurHttpClientSettings>(configuration.GetSection(nameof(OurHttpClientSettings)));
        }
    }
}