using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Models
{
    public record ProfileModel : ProfileListModel
    {
        public ProfileModel(Profile profile) : base(profile)
        {
        }
    }
}