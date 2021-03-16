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
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;

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

            //services.Configure<MvcOptions>(options =>
            //{
            //    var xmlInputFormatting = new IBatchCollectionXmlSerializer(options);
            //    var jsonInputFormatting = new JsonSerialisationConfiguration

            //    options.InputFormatters.Clear();
            //    options.InputFormatters.Add(jsonInputFormatting);
            //    options.InputFormatters.Add(xmlInputFormatting);
            //});

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
    public class IBatchCollectionXmlSerializer : XmlSerializerInputFormatter
    {
        public IBatchCollectionXmlSerializer(MvcOptions options) : base(options)
        {
        }

        protected override XmlSerializer CreateSerializer(Type type)
        {
var                serializer = base.CreateSerializer(type);
            return serializer;
        }
    }
}