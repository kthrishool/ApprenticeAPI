using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Models
{
    public record ApprenticeTFNModel 
    {
        public int Id;
        public int ApprenticeId;
        public string TaxFileNumber;
        public TFNStatus Status;
        public string StatusReason;


        public ApprenticeTFNModel() { }

        public ApprenticeTFNModel(ApprenticeTFN Tfn) 
        {
            Id = Tfn.Id;
            ApprenticeId = Tfn.ApprenticeId;
            Status = Tfn.StatusCode;
            TaxFileNumber = Tfn.TaxFileNumber;
            StatusReason = Tfn.StatusReasonCode;
        }
    }
}