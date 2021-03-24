using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
    public enum AddressType
    {
        //TODO: Need to identify code and associated that here
        [Description("Residential Address")]
        RESD,

        [Description("Postal Address")]
        POST
    }
}