using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
    public enum AddressType
    {        
        [Description("Residential Address")]
        RESD,

        [Description("Postal Address")]
        POST
    }
}