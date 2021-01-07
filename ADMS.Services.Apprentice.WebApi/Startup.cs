using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Employment.Services.Infrastructure.WebApi;
using Employment.Services.Infrastructure.WebApi.Documentation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ADMS.Services.Apprentice.WebApi
{
    /// <remarks />
    public class Startup 
    {
        public Startup (IConfiguration configuration)
        {
            if (configuration!=null)
                Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure(new List<IApplicationModelConvention>() { new ApiExplorerGetsOnlyConvention() });
            services.AddDocumentation("Employment.Services.Adms.XML");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IServiceProvider svp)
        {
            app.UseDocumentation(env, loggerFactory, svp);
            app.UseInfrastructure(env, loggerFactory, svp);
        }
    }

}
