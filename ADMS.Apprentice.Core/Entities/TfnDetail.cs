﻿using Adms.Shared;
using ADMS.Services.Infrastructure.Model.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ADMS.Apprentice.Core.Entities
{
    public class TfnDetail : IAmAnAggregateRoot<int>, ITimestampEnabled, IAuditable
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }
        public string TFN { get; set; }
        public TFNStatus Status { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }

        [JsonIgnore] 
        public virtual ICollection<TfnStatusHistory> TfnStatusHistories { get; }

        public TfnDetail()
        {
            TfnStatusHistories = new Collection<TfnStatusHistory>();
        }

    }
}