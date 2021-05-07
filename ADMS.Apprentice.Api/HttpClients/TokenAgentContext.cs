using ADMS.Infrastructure.Adfs.OAuth2.ClientCredentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Api.HttpClients
{
    class TokenAgentContext : ITokenAgentContext
    {
        public string AdfsDomainName { get; set; }
        public string ClientId { get; set; }
        public string Resource { get; set; }
        public string SigningCertficateThumbprint { get; set; }
        public string OAuth2TokenEndPoint => $"https://{AdfsDomainName}/adfs/oauth2/token";
    }
}