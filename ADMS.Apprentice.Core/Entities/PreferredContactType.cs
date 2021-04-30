using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
    public enum PreferredContactType
    {        
        [Description("Phone")]
        PHONE,

        [Description("Mobile")]
        MOBILE,

        [Description("Email")]
        EMAIL,

        [Description("Mail")]
        MAIL,

        [Description("SMS")]
        SMS
    }
}