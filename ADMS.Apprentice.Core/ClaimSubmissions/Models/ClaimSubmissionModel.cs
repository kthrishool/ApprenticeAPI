using ADMS.Apprentice.Core.ClaimSubmissions.Entities;

namespace ADMS.Apprentice.Core.ClaimSubmissions.Models
{
    public record ClaimSubmissionModel : ClaimSubmissionListModel
    {
        public ClaimSubmissionModel(ClaimSubmission claimSubmission) : base(claimSubmission)
        {
        }
    }
}