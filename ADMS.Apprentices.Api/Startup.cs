using System;
using System.IO;
using System.Linq;
using ADMS.Apprentices.Api.Configuration;
using ADMS.Apprentices.Api.HttpClients;
using Adms.Shared.DependencyInjection;
using Au.Gov.Infrastructure;
using Au.Gov.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Adms.Shared.Extensions;
using Au.Gov.Infrastructure.Authorisation;
using Adms.Shared.Swagger;
using Microsoft.Extensions.Options;
using ADMS.Apprentices.Core;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMemberHierarchy.Global
// ReSharper disable UnusedMember.Global

namespace ADMS.Apprentices.Api
{
    /// <summary>
    /// Application startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // The internal infrastructure is not hardened for internet use. Never deploy internal apis to an internet facing url.
            ModuleProvider = new AdmsInternalApiInfrastructureModules(Configuration)
                    .Remove(typeof(IOpenApiModuleBuilder)) //remove openapi
                ;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <remarks />
        public BaseInfrastructureModules ModuleProvider { get; set; }

        /// <summary>
        /// Configures services
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddInfrastructure(new List<IApplicationModelConvention> { new ApiExplorerGetsOnlyConvention() });
            //services.AddDocumentation("ADMS.Apprentices.Api.XML");
            //services.AddControllers();

            //services.AddMvcCore().AddNewtonsoftJson(options =>
            //{
            //    JsonSerialisationConfiguration.Configure(options.SerializerSettings);
            //    options.SerializerSettings.Converters.Add(new SortedListConverter());
            //}).AddXmlSerializerFormatters();
            services.AddInfrastructure(ModuleProvider, Configuration);

            SwaggerConfiguration.ConfigureServices(services, GetSwaggerSettings());

            SettingsConfiguration.Configure(services, Configuration);
            HttpClientConfiguration.Configure(services, Configuration);
            DependencyInjectionConfiguration.ConfigureServices(services, Configuration);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(
            IApplicationBuilder app,
            IHostApplicationLifetime lifetime,
            IWebHostEnvironment env,
            IOptions<OurEnvironmentSettings> ourEnvironmentSettings)
        {
            //app.UseRequestBuffering();

            SwaggerConfiguration.Configure(app, GetSwaggerSettings(ourEnvironmentSettings));
            //swagger documentation
            //app.UseDocumentation(env, loggerFactory, svp);
            //app.UseInfrastructure(env, loggerFactory, svp);
            app.UseInfrastructure(lifetime, env, ModuleProvider);
        }
        private static SwaggerSettings GetSwaggerSettings(IOptions<OurEnvironmentSettings> ourEnvironmentSettings = null)
        {
            return new()
            {
                Path = ourEnvironmentSettings?.Value.SwaggerPath,
                Prefix = ourEnvironmentSettings?.Value.SwaggerPrefix,
                ApiInfo = new[]
                {
                    new OpenApiInfo
                    {
                        Version = "default",
                        Title = "ADMS Apprentices API",
                        Description = "API documentation for the ADMS Apprentices API (current version)."
                    },
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "ADMS Apprentices API",
                        Description = "API documentation for the ADMS Apprentices API (version 1)."
                    }
                },
                XmlDocumentationFilenames = new[] { "Adms.Apprentices.Api.xml" }
            };
        }
    }
}
