using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
    public enum PreferredContactType
    {
        //TODO: Need to identify code and associated that here
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