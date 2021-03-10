using Adms.Shared.Attributes;
using Adms.Shared.Helpers;
using ADMS.Apprentice.Core.Entities;
using System;

namespace ADMS.Apprentice.Core.Models
{
    public partial class TfnDetailModel : TfnDetailBase
    {

        public TfnDetailModel() { }

        public TfnDetailModel(TfnDetail Tfn) : base(Tfn)
        { }

        public TfnDetailModel(TfnDetail Tfn, IApiUrlBuilder apiUrlBuilder, string currentUrl) : this(Tfn)
        {
            var uri = new Uri(currentUrl);
        }
    }

    [ModelXml("Tfn", CollectionParentName = "TfnList")]
    public class TfnDetailBase
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }
        public string TFN { get; set; }
        public TfnStatus Status { get; set; }

        public TfnDetailBase() { }

        public TfnDetailBase(TfnDetail Tfn)
        {
            Id = Tfn.Id;
            ApprenticeId = Tfn.ApprenticeId;
            Status = Tfn.Status;
            TFN = Tfn.TFN;

        }
    }
}