using System;

namespace ADMS.Apprentices.Core.Messages.TFN
{
    public record TFNStatsV1(
       int ApprenticeId,
       string ApprenticeName,
        DateTime DateOfBirth,
        DateTime TfnStatusDateTime,
        DateTime? TfnCreatedDateTime,
        string TfnVerificationStatus,
        string NumberOfDaysSinceTheMismatch
    );
}
