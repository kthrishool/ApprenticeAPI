using System.Reflection;
using ADMS.Apprentices.Core;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.Database;
using Adms.Shared;
using Adms.Shared.Helpers;
using Adms.Shared.Paging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADMS.Apprentices.Api.Configuration
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
        /// <param name="configuration"></param>
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
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
            services.AddScoped<ITYIMSRepository, TYIMSRepository>();
            services.AddScoped<IApprenticeRepository, Repository>();
            /* NOTE: IQualificationValidator has two implementations, have to be registered manually here */
            services.AddTransient<IQualificationValidator, PriorQualificationValidator>();
            var usiSettings = new OurUsiSettings();
            configuration.GetSection(nameof(OurUsiSettings)).Bind(usiSettings);
            if (usiSettings.USIVerifyDisabled)
                services.AddTransient<IUSIVerify, USIVerifyDisabled>();
            else
                services.AddTransient<IUSIVerify, USIVerify>();
        }
    }
}