using System;
using ADMS.Apprentice.Core.Entities;
using ADMS.Infrastructure.Cache.OutputCache.Time;

namespace ADMS.Apprentice.Core.Models.TFN
{
    public record TFNStatsModel
    {

        int ApprenticeId;
        string ApprenticeName;
        DateTime DateOfBirth;
        DateTime TfnSubmittedDateTime;
        DateTime TfnCreatedDateTime;
        string TfnVerificationStatus;
        string NumberOfDaysSinceTheMismatch;

        public static explicit operator TFNStatsModel(ApprenticeTFN t) => new TFNStatsModel
        {
            ApprenticeId = t.ApprenticeId,
            //ApprenticeName = ,
            //DateOfBirth =,
            TfnSubmittedDateTime = t.StatusDate,
            TfnCreatedDateTime = t.CreatedOn.GetValueOrDefault(),
            TfnVerificationStatus = t.StatusCode.ToString()
        };
    }
}
