using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.Core.Messages
{
    public record ProfileAddressMessage
    {
        [MaxLength(80, ErrorMessage = "Street Address Line 1 cannot exceed 80 characters in length")]
        public string StreetAddress1 { get; set; }

        [MaxLength(80, ErrorMessage = "Street Address Line 2 cannot exceed 80 characters in length")]
        public string StreetAddress2 { get; set; }

        [MaxLength(80, ErrorMessage = "Street Address Line 3 cannot exceed 80 characters in length")]
        public string StreetAddress3 { get; set; }

        [MaxLength(40, ErrorMessage = "Suburb cannot exceed 40 characters in length")]
        public string Locality { get; set; }

        [MaxLength(40, ErrorMessage = "State cannot exceed 40 characters in length")]
        public string StateCode { get; set; }

        [MaxLength(4, ErrorMessage = "Postcode cannot exceed 4 characters in length")]
        public string Postcode { get; set; }

        [MaxLength(375, ErrorMessage = "Address cannot exceed 375 characters in length")]
        public string SingleLineAddress { get; set; }
    }
}