namespace ADMS.Apprentice.Core.ClaimSubmissions.Entities
{
    public enum ClaimSubmissionStatus
    {
        Created,
        AwaitingEmployerApproval,
        AwaitingApprenticeApproval,
        AwaitingNetworkProviderApproval,
        Submitted,
        Deleted,
        Expired
    }
}