namespace ADMS.Apprentices.Api.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Au.Gov.Infrastructure.Authorisation;
    /// <summary>
    /// Configure authorisation policies for the API endpoints.
    /// </summary>
    public class AuthorisationConfiguration 
    {
        ///<summary>AUTH_Apprentice_view policy</summary>
        public const string  AUTH_Apprentice_View = "AUTH_Apprentice_View";

        ///<summary>AUTH_Apprentice_Management policy</summary>
        public const string  AUTH_Apprentice_Management = "AUTH_Apprentice_Management";

        ///<summary>AUTH_Apprentice_Activate policy</summary>
        public const string  AUTH_Apprentice_Activiate = "AUTH_Apprentice_Activiate";
        
        ///<summary>AUTH_Apprentice_Merge policy</summary>
        public const string  AUTH_Apprentice_Merge = "AUTH_Apprentice_Merge";

        ///<summary>AUTH_TSL_Management policy</summary>
        public const string  AUTH_Apprentice_TSL_Management = "AUTH_TSL_Management";

        ///<summary>AUTH_TSL_view policy</summary>
        public const string  AUTH_Apprentice_TSL_View = "AUTH_TSL_View";
        
        ///<summary>AUTH_Apprentice_Gateway policy</summary>
        public const string  AUTH_Apprentice_Gateway = "AUTH_Apprentice_Gateway";
        
        ///<summary>Authorisation for internal only</summary>
        public const string AUTH_ITAdmin = "AUTH_ITAdmin";
        
        ///<summary>Authorisation deny all</summary>
        public const string AUTH_DENY_ALL = "c27fc48d8b77afd827edeefccfecb4da"; // MD5sum of Deny everyone 🙂
    }
}
