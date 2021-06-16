using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
    public enum ProfileType
    {
        [Description("Australian Apprentice")]
        APPR,

        [Description("Initial Assessment")]
        INIT,

        [Description("Apprentice Seeker")]
        SEEK
    }
}