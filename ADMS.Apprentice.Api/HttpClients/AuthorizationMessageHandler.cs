using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ADMS.Apprentice.Api.HttpClients
{
    /// <summary>
    /// Adds authorization token from incoming requests to outgoing HttpClient requests
    /// </summary>
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthorizationMessageHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancelToken)
        {
            HttpRequestHeaders headers = request.Headers;
            AuthenticationHeaderValue authHeader = headers.Authorization;
            if (authHeader != null)
            {
                string auth = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (auth != null)
                {
                    string jwt = auth.Split(' ', StringSplitOptions.TrimEntries).LastOrDefault();
                    headers.Authorization = new AuthenticationHeaderValue(authHeader.Scheme, jwt);
                }
            }
            return await base.SendAsync(request, cancelToken);
        }
    }
}
