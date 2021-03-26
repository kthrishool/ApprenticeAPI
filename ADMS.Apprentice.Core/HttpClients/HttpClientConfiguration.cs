using System;
using System.Collections.Generic;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ADMS.Apprentice.Core.HttpClients
{
    public static class HttpClientConfiguration
    {
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
