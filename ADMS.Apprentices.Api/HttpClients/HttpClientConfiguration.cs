using System;
using System.Net;
using System.Net.Http;
using ADMS.Apprentices.Core;
using ADMS.Apprentices.Core.HttpClients.ReferenceDataApi;
using ADMS.Apprentices.Core.HttpClients.USI;
using Au.Gov.Infrastructure.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ADMS.Apprentices.Api.HttpClients
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
            services.AddTransient<ApiClientDelegatingHandler>();
            services.AddTransient<UsiAuthorizationMessageHandler>();

            var settings = new OurHttpClientSettings();
            configuration.GetSection(nameof(OurHttpClientSettings)).Bind(settings);

            services
                .AddHttpClient("referenceData", c => { c.BaseAddress = new Uri(settings.ReferenceDataEndpointBaseUrl); })
                .AddTypedClient(RestService.For<IReferenceDataClient>)
                .AddHttpMessageHandler<ApiClientDelegatingHandler>();

            services
                .AddHttpClient("usi", c => { c.BaseAddress = new Uri(settings.UsiEndpointBaseUrl); })
                .AddTypedClient(RestService.For<IUSIClient>)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    UseProxy = true,
                    AllowAutoRedirect = false,
                    Proxy = new WebProxy(settings.ProxyUrl, true) {UseDefaultCredentials = true},
                })
                .AddHttpMessageHandler<UsiAuthorizationMessageHandler>();
        }
    }
}