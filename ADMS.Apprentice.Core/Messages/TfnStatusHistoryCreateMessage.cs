using System;
using System.ComponentModel.DataAnnotations;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Messages
{
    public class TfnStatusHistoryCreateMessage 
    {
        [Required(ErrorMessage = "Status is required")]
        public TFNStatus Status { get; init; }

        [Required(ErrorMessage = "TFN Id required")]
        public int TfnId { get; set; }
 
        private DateTime? statusDate;
        public DateTime? StatusDate {
            get { return statusDate; } 
            set { statusDate = DateTime.Now;} }
    }
}