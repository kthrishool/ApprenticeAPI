using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentices.Core
{
    public class OurUsiSettings
    {
        public string ClientID { get; set; }
        public string Resource { get; set; }
        public string SigningCertficateThumbprint { get; set; }
        public string OrganisationId { get; set; }
        public string AdfsDomainName { get; set; }
        public bool USIVerifyDisabled { get; set; }
    }
}