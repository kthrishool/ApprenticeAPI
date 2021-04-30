using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
    public enum ProfileType
    {
        [Description("Apprentice")]
        APPR,

        [Description("Initial Assessment")]
        INIT,

        [Description("Gateway")]
        GWAY
    }
}