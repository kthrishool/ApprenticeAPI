using System.Reflection;
using ADMS.Apprentice.Core;
using ADMS.Apprentice.Database;
using Adms.Shared;
using Adms.Shared.Helpers;
using Adms.Shared.Paging;
using Microsoft.Extensions.DependencyInjection;

namespace ADMS.Apprentice.Api.Configuration
{
    /// <summary>
    /// Configuration for dependency injection
    /// </summary>
    public static class DependencyInjectionConfiguration
    {
        /// <summary>
        /// Configures dependency injection
        /// </summary>
        /// <param name="services">Services collection</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            // interfaces which live in the same assembly as their implementation(s) can be registered using our IocRegistrationHelper
            Assembly core = typeof(OurDatabaseSettings).Assembly;
            Assembly database = typeof(Repository).Assembly;
            Assembly web = typeof(DependencyInjectionConfiguration).Assembly;
            Assembly shared = typeof(PagingHelper).Assembly;

            IocRegistrationHelper.SetupAutoRegistrations(services, core);
            IocRegistrationHelper.SetupAutoRegistrations(services, database);
            IocRegistrationHelper.SetupAutoRegistrations(services, web);
            IocRegistrationHelper.SetupAutoRegistrations(services, shared);

            // interfaces which live in a different assembly to their implementation get registered manually here
            services.AddTransient<ISharedSettings, SharedSettings>();
            services.AddScoped<IRepository, Repository>();            
        }
    }
}