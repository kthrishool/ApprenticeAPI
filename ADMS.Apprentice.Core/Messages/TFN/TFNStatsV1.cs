using System;

namespace ADMS.Apprentice.Core.Messages.TFN
{
    public record TFNStatsV1(
       int ApprenticeId,
       string ApprenticeName,
        DateTime? DateOfBirth,
        DateTime TfnSubmittedDateTime,
        DateTime? TfnCreatedDateTime,
        string TfnVerificationStatus,
        string NumberOfDaysSinceTheMismatch
    );
}
