using System.ComponentModel;

namespace ADMS.Apprentices.Core.Entities
{
    public enum PhoneType
    {        
        PHONE1,        
        PHONE2,
        //**NOT REMOVING SO THAT EXISTING MOBILE AND LANDLINE VALIDATIONS AND UNIT TESTINGS WILL NOT FAIL***** 
        LANDLINE,
        MOBILE
    }
}