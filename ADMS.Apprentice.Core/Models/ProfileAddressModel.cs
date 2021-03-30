using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.Core.Models
{
    public record ProfileAddressModel
    {        
        public string StreetAddress1 { get; set; }        
        public string StreetAddress2 { get; set; }        
        public string StreetAddress3 { get; set; }        
        public string Locality { get; set; }        
        public string StateCode { get; set; }        
        public string Postcode { get; set; }        
        public string SingleLineAddress { get; set; }
    }
}