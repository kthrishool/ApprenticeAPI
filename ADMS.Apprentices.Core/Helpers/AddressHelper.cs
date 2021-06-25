using System.Diagnostics.CodeAnalysis;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Helpers
{
    // function will be removed soon 
    [ExcludeFromCodeCoverage]
    public static class AddressHelper
    {
        public static void ExtractAddress(IAddressAttributes fromaddress, IAddressAttributes toAddress)
        {
            toAddress.SingleLineAddress = fromaddress.SingleLineAddress.Sanitise();
            toAddress.StreetAddress1 = fromaddress.StreetAddress1.Sanitise();
            toAddress.StreetAddress2 = fromaddress.StreetAddress2.Sanitise();
            toAddress.StreetAddress3 = fromaddress.StreetAddress3.Sanitise();
            toAddress.Locality = fromaddress.Locality.Sanitise();
            toAddress.StateCode = fromaddress.StateCode.Sanitise();
            toAddress.Postcode = fromaddress.Postcode.Sanitise();
        }
    }
}