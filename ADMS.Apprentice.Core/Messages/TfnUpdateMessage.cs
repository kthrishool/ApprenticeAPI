using System;
using System.ComponentModel.DataAnnotations;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Messages
{
    public class TfnUpdateMessage
    {
        [Required(ErrorMessage = "Status is required")]
        public TfnStatus Status { get; init; }
    }
}