using ADMS.Apprentice.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADMS.Apprentice.Api.Configuration
{
    public static class SettingsConfiguration
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OurEnvironmentSettings>(configuration.GetSection(nameof(OurEnvironmentSettings)));
            services.Configure<OurDatabaseSettings>(configuration.GetSection(nameof(OurDatabaseSettings)));
            services.Configure<OurTestingSettings>(configuration.GetSection(nameof(OurTestingSettings)));
        }
    }
}