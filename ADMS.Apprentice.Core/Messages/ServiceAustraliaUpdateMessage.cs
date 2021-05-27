﻿using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.Core.Messages
{
    public class ServiceAustraliaUpdateMessage
    {
        [MaxLength(10, ErrorMessage = "CRN Exceeds 10 Characters")]
        [Required]
        public string CustomerReferenceNumber { get; set; }
    }
}