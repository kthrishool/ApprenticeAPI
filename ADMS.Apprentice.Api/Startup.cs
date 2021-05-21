using System;
using System.Collections.Generic;
using ADMS.Apprentice.Api.Configuration;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Configuration;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared.Paging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ADMS.Apprentice.Api.HttpClients;
using Adms.Shared.Extensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMemberHierarchy.Global
// ReSharper disable UnusedMember.Global

namespace ADMS.Apprentice.Api
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
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures services
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure(new List<IApplicationModelConvention> { new ApiExplorerGetsOnlyConvention() });
            services.AddDocumentation("ADMS.Apprentice.Api.XML");
            services.AddControllers();

            services.AddMvcCore().AddNewtonsoftJson(options =>
            {
                JsonSerialisationConfiguration.Configure(options.SerializerSettings);
                options.SerializerSettings.Converters.Add(new SortedListConverter());
            }).AddXmlSerializerFormatters();

            SettingsConfiguration.Configure(services, Configuration);
            HttpClientConfiguration.Configure(services, Configuration);
            DependencyInjectionConfiguration.ConfigureServices(services);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            ILoggerFactory loggerFactory,
            IServiceProvider svp)
        {

            app.UseRequestBuffering();

            app.UseDocumentation(env, loggerFactory, svp);
            app.UseInfrastructure(env, loggerFactory, svp);
        }

    }

}