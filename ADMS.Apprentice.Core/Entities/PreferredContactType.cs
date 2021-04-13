using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
    public enum PreferredContactType
    {
        //TODO: Need to identify code and associated that here
        [Description("Phone")]
        Phone,

        [Description("Mobile")]
        Mobile,

        [Description("Email")]
        Email,

        [Description("Mail")]
        Mail,

        [Description("SMS")]
        SMS
    }
}