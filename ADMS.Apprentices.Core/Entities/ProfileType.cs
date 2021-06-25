using System.ComponentModel;

namespace ADMS.Apprentices.Core.Entities
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