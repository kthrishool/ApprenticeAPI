using System;
using ADMS.Services.Infrastructure.Model.Interface;

namespace ADMS.Apprentice.Core.Entities
{
    public class TfnStatusHistory: ITimestampEnabled, IAuditable
    {
        public int Id { get; set; }
        public int TfnDetailId { get; set; }
        public TFNStatus Status { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }

        public virtual TfnDetail Tfn { get; set; }
    }
}