using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
    public enum ProfileType
    {
        //TODO: Need to identify code and associated that here
        [Description("Apprentice")]
        APPR,

        [Description("Initial Assessment")]
        INIT,

        [Description("Gateway")]
        GWAY
    }
}