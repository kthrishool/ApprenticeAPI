using System.ComponentModel;

namespace ADMS.Apprentices.Core.Entities
{
    public enum GenderType
    {
        [Description("Male")]
        M,

        [Description("Female")]
        F,

        [Description("Unspecified")]
        X
    }
}