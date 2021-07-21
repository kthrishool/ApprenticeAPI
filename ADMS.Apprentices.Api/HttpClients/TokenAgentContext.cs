using Adms.Shared.ClientCredentials;

namespace ADMS.Apprentices.Api.HttpClients
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