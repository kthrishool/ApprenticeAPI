using System.ComponentModel;

namespace ADMS.Apprentices.Core.Entities
{
    public enum AddressType
    {        
        [Description("Residential Address")]
        RESD,

        [Description("Postal Address")]
        POST
    }
}