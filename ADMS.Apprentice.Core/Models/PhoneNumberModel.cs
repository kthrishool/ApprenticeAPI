﻿using System;

namespace ADMS.Apprentice.Core.Models
{
    public class PhoneNumberModel
    {
        public int Id { get; set; }
        public string PhoneTypeCode { get; set; }
        public string PhoneNumber { get; set; }
        public Boolean? PreferredPhoneFlag { get; set; }
    }
}