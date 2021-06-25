using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ADMS.Infrastructure.Adfs.OAuth2.ClientCredentials;
using ADMS.Apprentices.Core;
using Microsoft.Extensions.Options;

namespace ADMS.Apprentices.Api.HttpClients
{
    /// <summary>
    /// Generate and adds authorization token to outgoing HttpClient requests
    /// </summary>
    public class UsiAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IOptions<OurUsiSettings> ourUsiSettings;

        /// <summary>Constructor</summary>
        public UsiAuthorizationMessageHandler(IOptions<OurUsiSettings> ourUsiSettings)
        {
            this.ourUsiSettings = ourUsiSettings;
        }
        /// <summary>
        /// SendAsync
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancelToken)
        {
            // create a configuration context for USI ...
            ITokenAgentContext context = new TokenAgentContext
            {
                AdfsDomainName = ourUsiSettings.Value.AdfsDomainName,
                ClientId = ourUsiSettings.Value.ClientID,
                Resource = ourUsiSettings.Value.Resource,
                SigningCertficateThumbprint = ourUsiSettings.Value.SigningCertficateThumbprint,
            };

            // create a httpclient implementation using the above configuration context
            ITokenAgent tokenAgent = TokenAgentFactory.Create(TokenAgents.HttpClient, context);

            // use the TokenAgent to get a token
            Token token = tokenAgent.GetToken();
            string accessToken = token.AccessToken;

            HttpRequestHeaders headers = request.Headers; 
            AuthenticationHeaderValue authHeader = headers.Authorization;
            headers.Authorization = new AuthenticationHeaderValue(authHeader.Scheme, accessToken.ToString());           
            headers.Add("usi.gov.au-orgcode", ourUsiSettings.Value.OrganisationId);

            return await base.SendAsync(request, cancelToken);
        }
    }
}
