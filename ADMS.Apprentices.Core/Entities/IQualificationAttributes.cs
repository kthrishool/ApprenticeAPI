using System;

namespace ADMS.Apprentices.Core.Entities
{
    public interface IQualificationAttributes
    {
        int ApprenticeId { get; set; }

        string QualificationCode { get; set; }
        string QualificationDescription { get; set; }
        string QualificationLevel { get; set; }
        string QualificationANZSCOCode { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }
}