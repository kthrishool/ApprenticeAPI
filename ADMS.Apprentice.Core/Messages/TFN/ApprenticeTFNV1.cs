﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Entities;

// ReSharper disable InconsistentNaming

namespace ADMS.Apprentice.Core.Messages.TFN
{
    public record ApprenticeTFNV1 
    {
        [Required(ErrorMessage = "ApprenticeId is required")]
        public int ApprenticeId;

        [Required(ErrorMessage = "TFN is required")]
        public long TaxFileNumber;
    }
}