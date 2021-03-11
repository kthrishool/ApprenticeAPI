using Adms.Shared.Attributes;
using Adms.Shared.Helpers;
using ADMS.Apprentice.Core.Entities;
using System;

namespace ADMS.Apprentice.Core.Models
{
    public record TfnDetailModel 
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }
        public string TFN { get; set; }
        public TFNStatus Status { get; set; }


        public TfnDetailModel() { }

        public TfnDetailModel(TfnDetail Tfn) 
        {
            Id = Tfn.Id;
            ApprenticeId = Tfn.ApprenticeId;
            Status = Tfn.Status;
            TFN = Tfn.TFN;

        }

        public TfnDetailModel(TfnDetail Tfn, IApiUrlBuilder apiUrlBuilder, string currentUrl) : this(Tfn)
        {
            var uri = new Uri(currentUrl);
        }
    }

}