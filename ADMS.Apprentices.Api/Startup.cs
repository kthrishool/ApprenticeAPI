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

            //swagger documentation
            var path = AppDomain.CurrentDomain.BaseDirectory;
            string xmlCommentFileName = "ADMS.Apprentices.Api.XML";
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Apprentices API", Version = "v1"});
                c.ResolveConflictingActions(ad => ad.First());
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(xmlCommentFileName))
                {
                    if (File.Exists($"{path}/{xmlCommentFileName}"))
                    {
                        c.IncludeXmlComments($"{path}/{xmlCommentFileName}");
                    }
                }
            });

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
            ILoggerFactory loggerFactory,
            IServiceProvider svp)
        {
            //app.UseRequestBuffering();

            //swagger documentation
            SwaggerOptions swaggerOptions = new SwaggerOptions();
            swaggerOptions.RouteTemplate = "swagger/docs/{documentName}";
            app.UseSwagger(c => c = swaggerOptions);
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger"; // serve the UI at root 
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            });

            //app.UseDocumentation(env, loggerFactory, svp);
            //app.UseInfrastructure(env, loggerFactory, svp);
            app.UseInfrastructure(lifetime, env, ModuleProvider);
        }
    }
}
