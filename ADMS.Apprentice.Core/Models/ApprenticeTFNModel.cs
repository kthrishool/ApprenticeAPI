using Adms.Shared.Attributes;
using Adms.Shared.Helpers;
using ADMS.Apprentice.Core.Entities;
using System;

namespace ADMS.Apprentice.Core.Models
{
    public record ApprenticeTFNModel 
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }
        public string TFN { get; set; }
        public TFNStatus Status { get; set; }


        public ApprenticeTFNModel() { }

        public ApprenticeTFNModel(ApprenticeTFN Tfn) 
        {
            Id = Tfn.Id;
            ApprenticeId = Tfn.ApprenticeId;
            Status = Tfn.StatusCode;
            TFN = Tfn.TaxFileNumber;

        }

        public ApprenticeTFNModel(ApprenticeTFN Tfn, IApiUrlBuilder apiUrlBuilder, string currentUrl) : this(Tfn)
        {
            var uri = new Uri(currentUrl);
        }
    }

}