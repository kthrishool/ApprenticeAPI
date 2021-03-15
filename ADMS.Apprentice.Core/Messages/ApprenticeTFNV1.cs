using System;
using System.ComponentModel.DataAnnotations;
using ADMS.Apprentice.Core.Entities;

// ReSharper disable InconsistentNaming

namespace ADMS.Apprentice.Core.Messages
{
    public record ApprenticeTFNV1 
    {
        [Required(ErrorMessage = "ApprenticeId is required")]
        public int ApprenticeId { get; set; }

        [Required(ErrorMessage = "TFN is required")]
        public long TaxFileNumber { get; set; }

        public TFNStatus StatusCode { get; set; }
        public DateTime StatusDate { get; set; }
        public string StatusReasonCode { get; set; }
        public Guid MessageQueueCorrelationId { get; set; }


    }
}