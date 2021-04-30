using System;
using System.Collections.Generic;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using ADMS.Apprentice.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ADMS.Apprentice.Api.HttpClients
{
    /// <summary>
    /// Configuration for the HTTP client
    /// </summary>
    public static class HttpClientConfiguration
    {
        /// <summary>
        /// Configures the HTTP client
        /// </summary>
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<AuthorizationMessageHandler>();
            IConfigurationSection ourHttpClientSettingsSection = configuration.GetSection(nameof(OurHttpClientSettings));
            services.Configure<OurHttpClientSettings>(ourHttpClientSettingsSection);
            OurHttpClientSettings settings = ourHttpClientSettingsSection.Get<OurHttpClientSettings>();
            services
                .AddHttpClient("referenceData", c => { c.BaseAddress = new Uri(settings.ReferenceDataEndpointBaseUrl); })
                .AddTypedClient(RestService.For<IReferenceDataClient>)
                .AddHttpMessageHandler<AuthorizationMessageHandler>();
        }
    }
}
