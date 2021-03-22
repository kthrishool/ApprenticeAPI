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

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMemberHierarchy.Global
// ReSharper disable UnusedMember.Global

namespace ADMS.Apprentice.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
            DependencyInjectionConfiguration.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            ILoggerFactory loggerFactory,
            IServiceProvider svp)
        {
            app.UseDocumentation(env, loggerFactory, svp);
            app.UseInfrastructure(env, loggerFactory, svp);
        }

    }
}